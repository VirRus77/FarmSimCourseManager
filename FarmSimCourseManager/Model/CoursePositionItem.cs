using System.Windows;
using FarmSimCourseManager.Tools.Controls;

namespace FarmSimCourseManager.Model
{
    public class CoursePositionItem : PositionItem<CourseModel>
    {
        public CoursePositionItem(CourseModel course)
        {
            Value = course;
            Position = new Point(0, 0);
        }
    }
    
    public class WaypointPositionItem : PositionItem<WaypointModel>
    {
        public WaypointPositionItem(WaypointModel waypoint)
        {
            Value = waypoint;
            Position = waypoint.Position;
        }
    }
}
