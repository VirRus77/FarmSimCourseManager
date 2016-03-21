using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using FarmSimCourseManager.CourseFileData;
using FarmSimCourseManager.Model;

namespace FarmSimCourseManager.Tools.Converters
{
    public class CourseToPositionItemConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var course = value as CourseModel;
            if (course != null)
                return new CoursePositionItem(course);

            var list = value as IList;
            if (list == null)
                return null;

            return list.OfType<CourseModel>().Select(v => new CoursePositionItem(v)).ToList();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class CourseToPointItemConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var course = value as CourseModel;
            if (course != null)
                return course.Waypoints.Select(waypoint => new WaypointPositionItem(waypoint));

            var list = value as IList;
            if (list == null)
                return null;

            return list.OfType<CourseModel>().SelectMany(v => v.Waypoints.Select(waypoint => new WaypointPositionItem(waypoint))).ToList();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}