using System.Collections.Generic;
using System.Globalization;
using FarmSimCourseManager.CourseFileData;
using FarmSimCourseManager.Tools;

namespace FarmSimCourseManager.Model
{
    public class FolderNode : ObservableObject, INode<Folder>
    {
        private readonly IList<Course> _courses;
        private readonly IList<Waypoint> _waypoints;
        private bool? _isChecked;
        private IList<INode> _childs;

        public FolderNode(Folder folder, IList<Course> courses)
        {
            Value = folder;
            _courses = courses;
            IsCollapsed = true;
            CreateChilds();
        }

        #region Implementation of INode

        public string Name { get { return Value.Name; } }

        public IList<INode> Childs
        {
            get { return _childs; }
            private set
            {
                if (_childs == value)
                    return;
                _childs = value;
                NotifyPropertyChanged(() => Childs);
            }
        }

        public bool IsCollapsed { get; set; }

        public bool? IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked == value)
                    return;
                _isChecked = value;
                Childs.ForEach(v => v.IsChecked = value);
                NotifyPropertyChanged(() => IsChecked);
            }
        }

        #endregion

        #region Implementation of INode<Folder>

        public Folder Value { get; private set; }

        #endregion

        private void CreateChilds()
        {
            var nodeList = new List<INode>();
            foreach (var course in _courses)
            {
                nodeList.Add(new CourseNode(course));
            }
            Childs = null;
            Childs = nodeList;
        }
    }

    public class CourseNode : ObservableObject, INode<Course>
    {
        private bool? _isChecked;

        public CourseNode(Course course)
        {
            Value = course;
            IsCollapsed = true;
            CreateChilds();
        }

        #region Implementation of INode

        public string Name { get { return Value.Name; } }
        public IList<INode> Childs { get; private set; }
        public bool IsCollapsed { get; set; }

        public bool? IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked == value)
                    return;
                _isChecked = value;
                NotifyPropertyChanged(() => IsChecked);
            }
        }

        #endregion

        #region Implementation of INode<Course>

        public Course Value { get; private set; }

        #endregion

        private void CreateChilds()
        {
            var nodeList = new List<INode>();
            foreach (var waypoint in Value.Waypoints)
            {
                nodeList.Add(new WaypointNode(waypoint));
            }
            Childs = null;
            Childs = nodeList;
        }
    }

    internal class WaypointNode : INode<Waypoint>
    {
        private static NumberFormatInfo NumberFormatInfo;

        static WaypointNode()
        {
            NumberFormatInfo = new NumberFormatInfo
            {
                NumberDecimalSeparator = "."
            };
        }

        public WaypointNode(Waypoint waypoint)
        {
            Value = waypoint;
            Childs = new INode[0];
            IsCollapsed = true;
            Name = string.Format("{0} {1}", Value.Position.X.ToString("0.00", NumberFormatInfo), Value.Position.Y.ToString("0.00", NumberFormatInfo));
        }

        #region Implementation of INode

        public string Name { get; private set; }
        public IList<INode> Childs { get; private set; }
        public bool IsCollapsed { get; set; }
        public bool? IsChecked { get; set; }

        #endregion

        #region Implementation of INode<Waypoint>

        public Waypoint Value { get; private set; }

        #endregion
    }
}
