using System;
using System.Reflection;

namespace FarmSimCourseManager.Tools
{
    public interface IEventEntry
    {
        WeakReference TargetReference { get; }
        MethodInfo TargetMethod { get; }
    }

    public interface IEventEntry<T1> : IEventEntry
    {
        bool Invoke(T1 a1);
    }

    public interface IEventEntry<T1, T2> : IEventEntry
    {
        bool Invoke(T1 a1, T2 a2);
    }
}