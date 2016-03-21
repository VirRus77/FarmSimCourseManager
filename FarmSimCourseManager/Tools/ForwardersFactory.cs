using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
#if !SILVERLIGHT
using System.Reflection.Emit;
#else
using System.Linq.Expressions;
#endif

namespace FarmSimCourseManager.Tools
{
    // The forwarder-generating code is in a separate class because it does not depend on type T.
    // Based on Daniel Grunwald FastSmartWeakEvent, Copyright (c) 2008
    public static class ForwardersFactory
    {
        public static Delegate CreateForwarder(MethodInfo method, params Type[] argsTypes)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            var forwarder = ForwardersCache.Get(method);
            if (forwarder != null)
                return forwarder;

            var declaringType = method.DeclaringType;
            Debug.Assert(declaringType != null);

            if (declaringType.HasCustomAttributes<CompilerGeneratedAttribute>(false))
                throw new ArgumentException("Cannot create weak event to anonymous method with closure.", "method");

            var parameters = method.GetParameters();
            if (parameters.Length != argsTypes.Length)
                throw new ArgumentException(string.Format("Delegate should take {0} parameters.", argsTypes.Length), "method");
            for (var i = 0; i < parameters.Length; i++)
            {
                var argType = argsTypes[i];
                if (argType != typeof(object) && !argType.IsAssignableFrom(parameters[i].ParameterType))
                    throw new ArgumentException(string.Format("The {0}'th delegate parameter must be derived from type '{1}'", i, argType), "method");
            }

            var getTarget = typeof(WeakReference).GetMethod("get_Target");
            Debug.Assert(getTarget != null);

            // Создать тип делегата по переданным типам аргументов.
            var sb = new StringBuilder();
            sb.AppendFormat("System.Func`{0}[", argsTypes.Length + 2);
            sb.AppendFormat("[{0}],", typeof(WeakReference).AssemblyQualifiedName);
            foreach (var argType in argsTypes)
                sb.AppendFormat("[{0}],", argType.AssemblyQualifiedName);
            sb.AppendFormat("[{0}]", typeof(bool).AssemblyQualifiedName);
            sb.AppendFormat("], {0}", typeof(Func<>).Assembly.FullName);
            var delegateType = Type.GetType(sb.ToString());
            if (delegateType == null)
            {
                throw new ArgumentException(string.Format("Can't create delegate type 'Func<WeakReference, {0}, bool>'.",
                    string.Join(", ", argsTypes.Select(t => t.Name).ToArray())), "argsTypes");
            }

#if !SILVERLIGHT
            var forwarderParameters = new List<Type> { typeof(WeakReference) };
            forwarderParameters.AddRange(argsTypes);
            var dm = new DynamicMethod("FastSmartWeakEvent", typeof(bool), forwarderParameters.ToArray(), declaringType);

            var il = dm.GetILGenerator();

            if (!method.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Callvirt, getTarget);
                il.Emit(OpCodes.Dup);
                var label = il.DefineLabel();
                il.Emit(OpCodes.Brtrue, label);
                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Ret);
                il.MarkLabel(label);
                // The castclass here is required for the generated code to be verifiable.
                // We can leave it out because we know this cast will always succeed
                // (the instance/method pair was taken from a delegate).
                // Unverifiable code is fine because private reflection is only allowed under FullTrust
                // anyways.
                //il.Emit(OpCodes.Castclass, declaringType);
            }
            for (var i = 0; i < parameters.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        il.Emit(OpCodes.Ldarg_1);
                        break;
                    case 1:
                        il.Emit(OpCodes.Ldarg_2);
                        break;
                    case 2:
                        il.Emit(OpCodes.Ldarg_3);
                        break;
                    default:
                        il.Emit(OpCodes.Ldarg, i + 1);
                        break;
                }
                var parameterType = parameters[i].ParameterType;
                if (parameterType != typeof(object) && !parameterType.IsAssignableFrom(argsTypes[i]))
                {
                    // This castclass here is required to prevent creating a hole in the .NET type system.
                    // See Program.TypeSafetyProblem in the 'SmartWeakEventBenchmark' to see the effect when
                    // this cast is not used.
                    // You can remove this cast if you trust add FastSmartWeakEvent.Raise callers to do
                    // the right thing, but the small performance increase (about 5%) usually isn't worth the risk.
                    il.Emit(OpCodes.Castclass, parameterType);
                }
            }

            il.Emit(OpCodes.Call, method);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Ret);

            var fd = dm.CreateDelegate(delegateType);
#else
            var rParam = Expression.Parameter(typeof(WeakReference));
            var lambdaParams = new List<ParameterExpression> { rParam };
            var callParams = new List<Expression>();
            for (var i = 0; i < argsTypes.Length; i++)
            {
                var argType = argsTypes[i];
                var parameterType = parameters[i].ParameterType;
                var param = Expression.Parameter(argType);
                lambdaParams.Add(param);
                callParams.Add(parameterType != typeof(object) && !parameterType.IsAssignableFrom(argType)
                    ? (Expression)Expression.Convert(param, parameterType)
                    : param);
            }

            Expression body;
            var exitPoint = Expression.Label(typeof(bool));
            if (!method.IsStatic)
            {
                var targetEx = Expression.Property(rParam, getTarget);
                body = Expression.Block(
                    Expression.IfThen(
                        Expression.ReferenceEqual(targetEx, Expression.Constant(null)),
                        Expression.Return(exitPoint, Expression.Constant(true))),
                    Expression.Call(Expression.Convert(targetEx, declaringType), method, callParams),
                    Expression.Return(exitPoint, Expression.Constant(false)),
                    Expression.Label(exitPoint, Expression.Constant(false)));
            }
            else
            {
                body = Expression.Block(
                    Expression.Call(method, callParams),
                    Expression.Return(exitPoint, Expression.Constant(false)),
                    Expression.Label(exitPoint, Expression.Constant(false)));
            }

            var fd = Expression.Lambda(delegateType, body, lambdaParams)
                .Compile();
#endif
            ForwardersCache.Set(method, fd);
            return fd;
        }
    }
}