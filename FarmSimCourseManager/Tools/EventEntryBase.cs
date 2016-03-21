using System;
using System.Reflection;

namespace FarmSimCourseManager.Tools
{
    public class EventEntryBase : IEventEntry
    {
        private readonly WeakReference _targetReference;
        private readonly MethodInfo _targetMethod;

        protected EventEntryBase(WeakReference targetReference, MethodInfo targetMethod)
        {
            _targetReference = targetReference;
            _targetMethod = targetMethod;
        }


        public WeakReference TargetReference
        {
            get { return _targetReference; }
        }

        public MethodInfo TargetMethod
        {
            get { return _targetMethod; }
        }
    }
}