using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Windows.Data;
#if !SILVERLIGHT

#endif

namespace FarmSimCourseManager.Tools
{
    /// <summary>
    /// Инструментальный класс для получения имени свойства текущего класса, используемого в выражении.
    /// </summary>
    public static class NameHelper
    {
#if !SILVERLIGHT
        private const string IndexerName = Binding.IndexerName;
#else
        private const string IndexerName = "Item[]";
#endif

        /// <summary>
        /// Получить имя свойства текущего класса.
        /// </summary>
        public static string Name<TProp>(Expression<Func<TProp>> expression, bool isDeep = false)
        {
            return GetMemberName(expression.Body, isDeep);
        }

        /// <summary>
        /// Получить имя свойства текущего класса.
        /// </summary>
        public static string Name(Expression<Action> expression, bool isDeep = false)
        {
            return GetMemberName(expression.Body, isDeep);
        }

        internal static string GetMemberName(Expression expression, bool isDeep)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression)expression;
                    var memberName = memberExpression.Member.Name;
                    if (!isDeep)
                        return memberName;
                    var superPath = GetMemberName(memberExpression.Expression, true);
                    return !string.IsNullOrEmpty(superPath) ? superPath + "." + memberName : memberName;
                case ExpressionType.Call:
                    var callExpression = (MethodCallExpression)expression;
                    var method = callExpression.Method;
                    var methodName = method.Name;
                    return method.IsSpecialName && methodName == "get_Item" ? IndexerName : methodName;
                case ExpressionType.Convert:
                    var unaryExpression = (UnaryExpression)expression;
                    return GetMemberName(unaryExpression.Operand, isDeep);
                case ExpressionType.Parameter:
                case ExpressionType.Constant: //Change
                    return string.Empty;
                default:
                    throw new ArgumentException("The expression is not a member access or method call expression");
            }
        }
    }

    /// <summary>
    /// Инструментальный класс для получения имени свойства заданного класса, используемого в выражении.
    /// </summary>
    /// <typeparam name="TSource">тип, имя свойства которого надо получить</typeparam>
    public static class NameHelper<TSource>
    {
        #region Вспомогательный класс

        private class MembersCollector : ExpressionVisitor
        {
#if !SILVERLIGHT
            private const string IndexerName = Binding.IndexerName;
#else
            private const string IndexerName = "Item[]";
#endif

            private readonly Type _type;
            private readonly bool _isDeep;
            private readonly ICollection<string> _members;
            private readonly ICollection<Expression> _nodesToSkip;

            public MembersCollector(Type type, bool isDeep)
            {
                _type = !IsNullableType(type) ? type : type.GetGenericArguments()[0];
                _isDeep = isDeep;
                _members = new HashSet<string>();
                _nodesToSkip = new HashSet<Expression>();
            }


            public ICollection<string> Members
            {
                get { return _members; }
            }


            protected override Expression VisitMember(MemberExpression node)
            {
                var parentExpression = node.Expression;
                if (!_nodesToSkip.Contains(node) && parentExpression != null)
                {
                    var superPath = new List<string>();
                    if (!_isDeep)
                    {
                        if (parentExpression.Type == _type)
                        {
                            if (!IsNullableType(parentExpression.Type))
                            {
                                var memberName = node.Member.Name;
                                _members.Add(memberName);
                            }
                            else
                            {
                                CollectPath(parentExpression, superPath);
                                superPath.Reverse();
                                if (superPath.Count > 0)
                                    _members.Add(superPath[superPath.Count - 1]);
                            }
                        }
                    }
                    else
                    {
                        var typeMatch = CollectPath(parentExpression, superPath);
                        superPath.Reverse();
                        _nodesToSkip.Add(parentExpression);
                        if (typeMatch)
                        {
                            if (!IsNullableType(parentExpression.Type))
                            {
                                var memberName = node.Member.Name;
                                _members.Add(superPath.Count != 0 ? string.Join(".", superPath.ToArray()) + "." + memberName : memberName);
                            }
                            else if (superPath.Count > 0)
                            {
                                _members.Add(string.Join(".", superPath.ToArray()));
                            }
                        }
                    }
                }
                return base.VisitMember(node);
            }

            private static bool IsNullableType(Type type)
            {
                return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
            }

            private bool CollectPath(Expression expression, IList<string> path)
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        var memberExpression = (MemberExpression)expression;
                        var member = memberExpression.Member;
                        var memberName = member.Name;
                        path.Add(memberName);
                        var parentExpression = memberExpression.Expression;
                        _nodesToSkip.Add(parentExpression);
                        return CollectPath(parentExpression, path);
                    case ExpressionType.Call:
                        var callExpression = (MethodCallExpression)expression;
                        var method = callExpression.Method;
                        var methodName = method.Name;
                        path.Add(method.IsSpecialName && methodName == "get_Item" ? IndexerName : methodName);
                        return method.DeclaringType == _type;
                    case ExpressionType.Convert:
                        var unaryExpression = (UnaryExpression)expression;
                        return CollectPath(unaryExpression.Operand, path);
                    case ExpressionType.Parameter:
                    case ExpressionType.Constant: //Change
                        return expression.Type == _type;
                    default:
                        throw new ArgumentException("The expression is not a member access or method call expression");
                }
            }
        }

        #endregion

        /// <summary>
        /// Получить имя свойства заданного класса.
        /// </summary>
        public static string Name<TProp>(Expression<Func<TSource, TProp>> expression, bool isDeep = false)
        {
            return NameHelper.GetMemberName(expression.Body, isDeep);
        }

        /// <summary>
        /// Получить имя свойства заданного класса.
        /// </summary>
        public static string Name(Expression<Action<TSource>> expression, bool isDeep = false)
        {
            return NameHelper.GetMemberName(expression.Body, isDeep);
        }

        /// <summary>
        /// Получить набор имён свойств заданного типа, использованных в выражении.
        /// </summary>
        public static ICollection<string> MemberNames<TProp>(Expression<Func<TSource, TProp>> expression, bool isDeep = false)
        {
            var collector = new MembersCollector(typeof(TSource), isDeep);
            collector.Visit(expression);
            return collector.Members;
        }
    }
}
