using System;
using System.Reflection;

namespace FarmSimCourseManager.Tools
{
    public class EventEntry<T1> : EventEntryBase, IEventEntry<T1>
    {
        private readonly Func<WeakReference, T1, bool> _forwarder;

        public EventEntry(WeakReference targetReference, MethodInfo targetMethod, Func<WeakReference, T1, bool> forwarder)
            : base(targetReference, targetMethod)
        {
            _forwarder = forwarder;
        }


        public bool Invoke(T1 a1)
        {
            return _forwarder(TargetReference, a1);
        }
    }

    public class EventEntry<T1, T2> : EventEntryBase, IEventEntry<T1, T2>
    {
        private readonly Func<WeakReference, T1, T2, bool> _forwarder;

        public EventEntry(WeakReference targetReference, MethodInfo targetMethod, Func<WeakReference, T1, T2, bool> forwarder)
            : base(targetReference, targetMethod)
        {
            _forwarder = forwarder;
        }


        public bool Invoke(T1 a1, T2 a2)
        {
            return _forwarder(TargetReference, a1, a2);
        }
    }
}