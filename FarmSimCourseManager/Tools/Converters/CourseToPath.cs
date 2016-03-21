using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using FarmSimCourseManager.Model;

namespace FarmSimCourseManager.Tools.Converters
{
    public class CoursePositionItemToPathConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var coursePositionItem = value as CoursePositionItem;
            if (coursePositionItem == null || coursePositionItem.Value.Waypoints.Count <= 1)
                return null;

            var course = coursePositionItem.Value;

            var path = new Path();
#if !true
            var geometryGroup = new PathGeometry();
            var pathFigure = new PathFigure();
            pathFigure.StartPoint = course.Waypoints.First().Position;

            pathFigure.Segments.Add(new PolyLineSegment(course.Waypoints.Skip(1).Select(v => v.Position), true));
            geometryGroup.Figures.Add(pathFigure);
            path.Data = geometryGroup;
#else
            var geometryGroup = new GeometryGroup();

            //foreach (var waypoint in course.Waypoints.Skip(1))
            //{
            //    pathFigure.Segments.Add(new LineSegment(waypoint.Position, true));
            //}

            var previewWaypoint = course.Waypoints.First();
            foreach (var waypoint in course.Waypoints.Skip(1))
            {
                var line = new LineGeometry(previewWaypoint.Position, waypoint.Position);
                previewWaypoint = waypoint;
                geometryGroup.Children.Add(line);
            }
            path.Data = geometryGroup;
#endif
            return path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}