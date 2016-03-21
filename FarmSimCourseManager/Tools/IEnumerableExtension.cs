using System;
using System.Collections;
using System.Collections.Generic;

namespace FarmSimCourseManager.Tools
{
    // ReSharper disable once InconsistentNaming
    public static class IEnumerableExtension
    {
        public static void ForEach(this IEnumerable enumerable, Action<object> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            foreach (var value in enumerable)
            {
                action(value);
            }
        }

        public static void ForEach<T>(this IEnumerable enumerable, Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            foreach (T value in enumerable)
            {
                action(value);
            }
        }

        public static void ForEach<T>(this IEnumerable enumerable, Action<T, int> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            var index = 0;
            foreach (T value in enumerable)
            {
                action(value, index++);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            foreach (var value in enumerable)
            {
                action(value);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            var index = 0;
            foreach (var value in enumerable)
            {
                action(value, index++);
            }
        }

        public static IEnumerable<V> SelectRecursive<T, V>(this IEnumerable<T> enumerable, Func<T, IEnumerable<T>> child, Func<T, V> select)
        {
            if (enumerable == null)
                yield break;
            foreach (var value in enumerable)
            {
                yield return select(value);
                foreach (var valueV in child(value).SelectRecursive(child, select))
                {
                    yield return valueV;
                }
            }
        }

        //public static T SingleOrDefault<T>(this IEnumerable<T> enumerable, T defaultValue)
        //    where T : class
        //{
        //    var value = enumerable.SingleOrDefault(default(T));
        //    return value ?? defaultValue;
        //}
    }
}
