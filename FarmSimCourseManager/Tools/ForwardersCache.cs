using System;
using System.Collections.Generic;
using System.Reflection;

namespace FarmSimCourseManager.Tools
{
    internal static class ForwardersCache
    {
        private static readonly IDictionary<MethodInfo, Delegate> Forwarders;

        static ForwardersCache()
        {
            Forwarders = new Dictionary<MethodInfo, Delegate>();
        }


        public static Delegate Get(MethodInfo method)
        {
            lock (Forwarders)
            {
                Delegate d;
                if (Forwarders.TryGetValue(method, out d))
                    return d;
            }
            return null;
        }

        public static void Set(MethodInfo method, Delegate d)
        {
            lock (Forwarders)
            {
                Forwarders[method] = d;
            }
        }
    }
}
