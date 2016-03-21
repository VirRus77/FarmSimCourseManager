using System;
using System.Linq.Expressions;
using System.Windows;

namespace FarmSimCourseManager.Tools
{
    /// <summary>
    /// Инструментальный класс для регистрации зависимых свойств.
    /// </summary>
    /// <typeparam name="TSource">тип, в рамках которого производится регистрация</typeparam>
    public static class DependencyPropertyHelper<TSource>
        where TSource : DependencyObject
    {
        public delegate void NotifyPropertyChanged<T1, T2>(T1 control, T2 oldValue, T2 newValue);
#if !SILVERLIGHT
        /// <summary>
        /// Зарегистрировать зависимое свойство.
        /// </summary>
        /// <typeparam name="TProp">тип свойства</typeparam>
        /// <param name="expression">выражение, содержащее доступ к обёртке свойства для получения его имени</param>
        /// <param name="onPropertyChanged">обработчик события изменения свойства, или null, если не нужен</param>
        /// <param name="defaultValue">значение свойства по-умолчанию</param>
        /// <param name="onCoerce">обработчик, подправляющий значение свойства перед установкой, или null, если не нужен</param>
        /// <param name="onValidate">обработчик, проверяющий корректность значения свойства, или null, если не нужен</param>
        /// <returns>зарегистрированное зависимое свойство</returns>
        public static DependencyProperty Register<TProp>(Expression<Func<TSource, TProp>> expression, NotifyPropertyChanged<TSource, TProp> onPropertyChanged = null,
            TProp defaultValue = default(TProp), Func<TSource, TProp, TProp> onCoerce = null, Func<TProp, bool> onValidate = null)
        {
            var propertyChangedCallback = onPropertyChanged != null ? (o, args) => onPropertyChanged((TSource)o, (TProp)args.OldValue, (TProp)args.NewValue) : (PropertyChangedCallback)null;
            var coerceValueCallback = onCoerce != null ? (o, v) => onCoerce((TSource)o, (TProp)v) : (CoerceValueCallback)null;
            var validateValueCallback = onValidate != null ? o => onValidate((TProp)o) : (ValidateValueCallback)null;
            return DependencyProperty.Register(NameHelper<TSource>.Name(expression), typeof(TProp), typeof(TSource),
                new PropertyMetadata(defaultValue, propertyChangedCallback, coerceValueCallback),
                validateValueCallback);
        }
#else
        /// <summary>
        /// Зарегистрировать зависимое свойство.
        /// </summary>
        /// <typeparam name="TProp">тип свойства</typeparam>
        /// <param name="expression">выражение, содержащее доступ к обёртке свойства для получения его имени</param>
        /// <param name="onPropertyChanged">обработчик события изменения свойства, или null, если не нужен</param>
        /// <param name="defaultValue">значение свойства по-умолчанию</param>
        /// <returns>зарегистрированное зависимое свойство</returns>
        public static DependencyProperty Register<TProp>(Expression<Func<TSource, TProp>> expression, Action<TSource, TProp, TProp> onPropertyChanged = null,
            TProp defaultValue = default(TProp))
        {
            var propertyChangedCallback = onPropertyChanged != null ? (o, args) => onPropertyChanged((TSource)o, (TProp)args.OldValue, (TProp)args.NewValue) : (PropertyChangedCallback)null;
            return DependencyProperty.Register(NameHelper<TSource>.Name(expression), typeof(TProp), typeof(TSource),
                new PropertyMetadata(defaultValue, propertyChangedCallback));
        }
#endif

#if !SILVERLIGHT
        /// <summary>
        /// Переопределить обработчик события изменения наследованного свойства.
        /// </summary>
        /// <typeparam name="TProp">тип свойства</typeparam>
        /// <param name="expression">выражение, содержащее доступ к обёртке свойства для получения его типа</param>
        /// <param name="property">наследованное зависимое свойство</param>
        /// <param name="onPropertyChanged">обработчик события изменения свойства</param>
        public static void OverrideFrameworkProperty<TProp>(Expression<Func<TSource, TProp>> expression, DependencyProperty property, Action<TSource, TProp, TProp> onPropertyChanged)
        {
            if (onPropertyChanged == null)
                throw new ArgumentNullException("onPropertyChanged");
            PropertyChangedCallback propertyChangedCallback = (o, args) => onPropertyChanged((TSource)o, (TProp)args.OldValue, (TProp)args.NewValue);
            property.OverrideMetadata(typeof(TSource), new FrameworkPropertyMetadata(propertyChangedCallback));
        }
#endif
    }
}
