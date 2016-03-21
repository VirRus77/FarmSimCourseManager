// Copyright (c) 2008 Daniel Grunwald
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace FarmSimCourseManager.Tools
{
    /// <summary>
    /// A class for managing a weak event.
    /// </summary>
    public sealed class WeakEvent<T>
        where T : class
    {
        private readonly List<IEventEntry<object, EventArgs>> _eventEntries;

        static WeakEvent()
        {
            if (!typeof(T).IsSubclassOf(typeof(Delegate)))
                throw new ArgumentException("T must be a delegate type");
            var invoke = typeof(T).GetMethod("Invoke");
            if (invoke == null)
                throw new ArgumentException("T must be a delegate type");
            var parameters = invoke.GetParameters();
            if (parameters.Length != 2)
                throw new ArgumentException("T must be a delegate type taking 2 parameters");
            if (parameters[0].ParameterType != typeof(object))
                throw new ArgumentException("The first delegate parameter must be of type 'object'");
            if (!typeof(EventArgs).IsAssignableFrom(parameters[1].ParameterType))
                throw new ArgumentException("The second delegate parameter must be derived from type 'EventArgs'");
            if (invoke.ReturnType != typeof(void))
                throw new ArgumentException("The delegate return type must be void.");
        }

        public WeakEvent()
        {
            _eventEntries = new List<IEventEntry<object, EventArgs>>();
        }


        public int SubscribersCount
        {
            get { return _eventEntries.Count; }
        }


        public void Add(T eh)
        {
            if (eh == null)
                return;
            var d = (Delegate)(object)eh;
            if (_eventEntries.Count == _eventEntries.Capacity)
                RemoveDeadEntries();
            var targetMethod = d.Method;
            var targetInstance = d.Target;
            var target = targetInstance != null ? new WeakReference(targetInstance) : null;
            _eventEntries.Add(CreateEventEntry<object, EventArgs>(target, targetMethod));
        }

        public void Remove(T eh)
        {
            if (eh == null)
                return;
            var d = (Delegate)(object)eh;
            var targetInstance = d.Target;
            var targetMethod = d.Method;
            for (var i = _eventEntries.Count - 1; i >= 0; i--)
            {
                var entry = _eventEntries[i];
                if (entry.TargetReference != null)
                {
                    var target = entry.TargetReference.Target;
                    if (target == null)
                    {
                        _eventEntries.RemoveAt(i);
                    }
                    else if (target == targetInstance && entry.TargetMethod == targetMethod)
                    {
                        _eventEntries.RemoveAt(i);
                        break;
                    }
                }
                else
                {
                    if (targetInstance == null && entry.TargetMethod == targetMethod)
                    {
                        _eventEntries.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void Raise(object sender, EventArgs e)
        {
            var needsCleanup = false;
            foreach (var ee in _eventEntries.ToArray())
            {
                if (ee.Invoke(sender, e))
                    needsCleanup = true;
            }
            if (needsCleanup)
                RemoveDeadEntries();
        }

        private static IEventEntry<T1, T2> CreateEventEntry<T1, T2>(WeakReference target, MethodInfo targetMethod)
        {
            return new EventEntry<T1, T2>(target, targetMethod, (Func<WeakReference, T1, T2, bool>)ForwardersFactory.CreateForwarder(targetMethod, typeof(T1), typeof(T2)));
        }

        private void RemoveDeadEntries()
        {
#if !SILVERLIGHT
            _eventEntries.RemoveAll(ee => ee.TargetReference != null && !ee.TargetReference.IsAlive);
#else
            for (var i = _eventEntries.Count - 1; i >= 0; i--)
            {
                var ee = _eventEntries[i];
                var targetReference = ee.TargetReference;
                if (targetReference != null && !targetReference.IsAlive)
                    _eventEntries.RemoveAt(i);
            }
#endif
        }
    }
}