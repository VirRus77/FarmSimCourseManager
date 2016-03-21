using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace FarmSimCourseManager.Tools.Converters
{
    public class MoveConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                return -(int)value * 0.5d;
            }
            if (value is float)
            {
                return -(float)value * 0.5d;
            }
            if (value is double)
            {
                return -(double)value * 0.5d;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class AntiScaleConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return value;
            try
            {
                var convertValue = System.Convert.ToDouble(value);
                var convertParameter = System.Convert.ToDouble(parameter);
                return convertParameter / convertValue;
            }
            catch (Exception)
            {
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
