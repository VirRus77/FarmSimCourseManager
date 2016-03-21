using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmSimCourseManager.Tools
{
    /// <summary>
    /// Инструментальный класс работы с типами.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Проверить наличие атрибутов заданного типа.
        /// </summary>
        /// <typeparam name="T">тип атрибута</typeparam>
        /// <param name="type">тип, атрибуты которого проверяются</param>
        /// <param name="inherit">флаг учёта атрибутов, наследованных от родителей</param>
        /// <returns>признак наличия атрибутов</returns>
        public static bool HasCustomAttributes<T>(this Type type, bool inherit)
        {
            return type.GetCustomAttributes(typeof(T), inherit).Length != 0;
        }

        /// <summary>
        /// Получить набор атрибутов заданного типа.
        /// </summary>
        /// <typeparam name="T">тип атрибута</typeparam>
        /// <param name="type">тип, атрибуты которого считываются</param>
        /// <param name="inherit">флаг чтения атрибутов, наследованных от родителей</param>
        /// <returns>набор атрибутов типа</returns>
        public static IEnumerable<T> GetCustomAttributes<T>(this Type type, bool inherit)
        {
            return type.GetCustomAttributes(typeof(T), inherit).Cast<T>();
        }

        /// <summary>
        /// Перечислить иерархию наследования.
        /// </summary>
        /// <param name="type">тип для перечисления иерархии</param>
        /// <returns>набор базовых типов вверх по иерархии, начиная с указанного</returns>
        public static IEnumerable<Type> GetInheritancHierarchy(this Type type)
        {
            for (var current = type; current != null; current = current.BaseType)
                yield return current;
        }
    }
}
