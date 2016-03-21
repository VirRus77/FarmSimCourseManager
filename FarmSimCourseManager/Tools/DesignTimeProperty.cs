using System.ComponentModel;
using System.Windows;

namespace FarmSimCourseManager.Tools
{
    /// <summary>
    /// Класс установки значений для дизайна
    /// </summary>
    public static class DesignTimeProperty
    {
        static bool? _inDesignMode;

        /// <summary>
        /// Indicates whether or not the framework is in design-time mode. (Caliburn.Micro implementation)
        /// </summary>
        public static bool InDesignMode
        {
            get
            {
                if (_inDesignMode == null)
                {
                    var prop = DesignerProperties.IsInDesignModeProperty;
                    _inDesignMode = (bool)DependencyPropertyDescriptor
                        .FromProperty(prop, typeof(FrameworkElement))
                        .Metadata
                        .DefaultValue;

                    if (!_inDesignMode.GetValueOrDefault(false)
                        && System.Diagnostics.Process.GetCurrentProcess()
                        .ProcessName.StartsWith("devenv", System.StringComparison.Ordinal))
                    {
                        _inDesignMode = true;
                    }
                }

                return _inDesignMode.GetValueOrDefault(false);
            }
        }

        #region Background

        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.RegisterAttached(
            "Background", typeof(System.Windows.Media.Brush), typeof(DesignTimeProperty),
            new PropertyMetadata(BackgroundChanged));

        public static System.Windows.Media.Brush GetBackground(DependencyObject dependencyObject)
        {
            return (System.Windows.Media.Brush)dependencyObject.GetValue(BackgroundProperty);
        }
        public static void SetBackground(DependencyObject dependencyObject, System.Windows.Media.Brush value)
        {
            dependencyObject.SetValue(BackgroundProperty, value);
        }

        private static void BackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!InDesignMode)
                return;

            var propertyInfo = d.GetType().GetProperty("Background");
            if (propertyInfo == null)
                return;

            propertyInfo.SetValue(d, e.NewValue, null);
        }

        #endregion

        #region MinHeight

        public static readonly DependencyProperty MinHeightProperty = DependencyProperty.RegisterAttached(
            "MinHeight", typeof(double), typeof(DesignTimeProperty),
            new PropertyMetadata(MinHeightChanged));

        public static double GetMinHeight(DependencyObject dependencyObject)
        {
            return (double)dependencyObject.GetValue(MinHeightProperty);
        }
        public static void SetMinHeight(DependencyObject dependencyObject, double value)
        {
            dependencyObject.SetValue(MinHeightProperty, value);
        }

        private static void MinHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!InDesignMode)
                return;

            var propertyInfo = d.GetType().GetProperty("MinHeight");
            if (propertyInfo == null)
                return;

            propertyInfo.SetValue(d, e.NewValue, null);
        }

        #endregion

        #region MinWidth

        public static readonly DependencyProperty MinWidthProperty = DependencyProperty.RegisterAttached(
            "MinWidth", typeof(double), typeof(DesignTimeProperty),
            new PropertyMetadata(MinWidthChanged));

        public static double GetMinWidth(DependencyObject dependencyObject)
        {
            return (double)dependencyObject.GetValue(MinWidthProperty);
        }
        public static void SetMinWidth(DependencyObject dependencyObject, double value)
        {
            dependencyObject.SetValue(MinWidthProperty, value);
        }

        private static void MinWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!InDesignMode)
                return;

            var propertyInfo = d.GetType().GetProperty("MinWidth");
            if (propertyInfo == null)
                return;

            propertyInfo.SetValue(d, e.NewValue, null);
        }

        #endregion
    }
}
