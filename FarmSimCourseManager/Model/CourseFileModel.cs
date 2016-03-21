using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;
using FarmSimCourseManager.CourseFileData;
using FarmSimCourseManager.Tools;

namespace FarmSimCourseManager.Model
{
    /// <summary>
    /// Модель файла маршрута
    /// </summary>
    public class CourseFileModel : ObservableObject
    {
        /// <summary>
        /// Не изменяется от загрузки до сохранения
        /// </summary>
        public CourseHeader Header { get; set; }

        /// <summary>
        /// Рутовый директорий
        /// </summary>
        public FolderModel Folder { get; set; }

        /// <summary>
        /// Список маршрутов
        /// </summary>
        public List<CourseModel> Courses { get; set; }

        public CourseFile Save()
        {
            return Save(this);
        }

        public static CourseFileModel Load(CourseFile courseFile)
        {
            var courseFileModel = new CourseFileModel();
            courseFileModel.Header = courseFile;
            courseFileModel.Folder = new FolderModel
            {
                Name = "Root",
                Folders = LoadFolders(courseFile),
                Courses = LoadCourses(courseFile, 0)
            };
            courseFileModel.Folder.Courses.ForEach(course => course.Folder = courseFileModel.Folder);

            courseFileModel.Courses = courseFileModel.Folder
                .Courses
                .Concat(courseFileModel.Folder
                    .Folders
                    .SelectRecursive(folder => folder.Folders, folder => folder.Courses)
                    .SelectMany(v => v)
                    .ToList())
                .ToList();

            courseFileModel.Folder.IsChecked = true;

            return courseFileModel;
        }

        private static List<FolderModel> LoadFolders(CourseFile courseFile, int parent = 0)
        {
            var folderList = courseFile.Folders
                .Where(v => v.Parent == parent)
                .Select(folder => new FolderModel
                {
                    Name = folder.Name,
                    Courses = LoadCourses(courseFile, folder.Id),
                    Folders = LoadFolders(courseFile, folder.Id)
                })
                .ToList();

            folderList.ForEach(folder => folder.Courses.ForEach(v => v.Folder = folder));

            return folderList;
        }

        private static List<CourseModel> LoadCourses(CourseFile courseFile, int folder)
        {
            var coursesList = courseFile.Courses
                .Where(v => v.Parent == folder)
                .Select(course => new CourseModel
                {
                    Name = course.Name,
                    Waypoints = LoadWaypoints(course)
                })
                .ToList();
            coursesList.ForEach(course => course.Lines = LoadLines(course));
            coursesList.ForEach(course => course.Waypoints.ForEach(v => v.Course = course));

            return coursesList;
        }

        private static List<LineModel> LoadLines(CourseModel course)
        {
            var firstPoint = course.Waypoints.First();
            return course.Waypoints
                .Skip(1)
                .Select(v => new LineModel
                {
                    Course = course,
                    StartPoint = firstPoint,
                    EndPoint = firstPoint = v,
                })
                .ToList();
        }

        private static List<WaypointModel> LoadWaypoints(Course course)
        {
            var waypointsList = course.Waypoints
                .Select(v => new WaypointModel
                {
                    Position = v.Position,
                    Angle = v.Angle,
                    Crossing = v.Crossing,
                    Direction = v.Direction,
                    Generated = v.Generated,
                    Revers = v.Revers,
                    RidgeMarker = v.RidgeMarker,
                    Speed = v.Speed,
                    Turn = v.Turn,
                    TurnEnd = v.TurnEnd,
                    TurnStart = v.TurnStart,
                    Wait = v.Wait
                })
                .ToList();

            return waypointsList;
        }

        private static CourseFile Save(CourseFileModel courseFileModel)
        {
            var courseFile = new CourseFile
            {
                CourseplayHud = courseFileModel.Header.CourseplayHud,
                Course2D = courseFileModel.Header.Course2D,
                CourseManagement = courseFileModel.Header.CourseManagement,
                CourseplayFields = courseFileModel.Header.CourseplayFields,
                CourseplayInGameMap = courseFileModel.Header.CourseplayInGameMap,
                CourseplayWages = courseFileModel.Header.CourseplayWages,
            };

            Dictionary<FolderModel, int> indexFolder;
            courseFile.Folders = SaveFolders(courseFileModel, out indexFolder);
            courseFile.Courses = SaveCourses(courseFileModel, indexFolder);
            return courseFile;
        }

        private static List<Folder> SaveFolders(CourseFileModel courseFileModel, out Dictionary<FolderModel, int> dictionary)
        {
            var index = 0;
            var foldersDic = courseFileModel.Folder.Folders.SelectRecursive(v => v.Folders, v => v).ToDictionary(k => k, v => new Folder
            {
                Id = ++index,
                Name = v.Name,
            });

            foldersDic.ForEach(v =>
            {
                var folder = foldersDic.Where(v2 => v2.Key.Folders.Contains(v.Key)).Select(v3 => v3.Value).SingleOrDefault();
                v.Value.Parent = folder == null ? 0 : folder.Id;
            });

            dictionary = foldersDic.ToDictionary(k => k.Key, v => v.Value.Id);

            return foldersDic.Select(v => v.Value).ToList();
        }

        private static List<Course> SaveCourses(CourseFileModel courseFileModel, Dictionary<FolderModel, int> indexFolder)
        {
            var index = 0;
            return courseFileModel.Courses.Select(v => new Course
            {
                Id = ++index,
                Name = v.Name,
                Parent = indexFolder.ContainsKey(v.Folder) ? indexFolder[v.Folder] : 0,
                Waypoints = SaveWaypoints(v.Waypoints)
            }).ToList();
        }

        private static List<Waypoint> SaveWaypoints(List<WaypointModel> waypoints)
        {
            return waypoints
                .Select(v => new Waypoint
                {
                    Position = v.Position,
                    Angle = v.Angle,
                    Crossing = v.Crossing,
                    Direction = v.Direction,
                    Generated = v.Generated,
                    Revers = v.Revers,
                    RidgeMarker = v.RidgeMarker,
                    Speed = v.Speed,
                    Turn = v.Turn,
                    TurnEnd = v.TurnEnd,
                    TurnStart = v.TurnStart,
                    Wait = v.Wait,
                })
                .ToList();
        }
    }

    /// <summary>
    /// Модель директория
    /// </summary>
    public class FolderModel : ObservableObject, INode
    {
        private bool _isCheckChange;
        private bool? _isChecked;
        private IList<FolderModel> _folders;
        private List<CourseModel> _courses;

        public IList<FolderModel> Folders
        {
            get { return _folders; }
            set
            {
                if (_folders == value)
                    return;
                if(_folders != null)
                    _folders.ForEach(v => v.PropertyChanged -= ChildPropertyChanged);
                _folders = value;
                if (_folders != null)
                    _folders.ForEach(v => v.PropertyChanged += ChildPropertyChanged);
                NotifyPropertyChanged(() => Folders);
                NotifyPropertyChanged(() => Childs);
            }
        }

        /// <summary>
        /// Список маршрутов
        /// </summary>
        public List<CourseModel> Courses
        {
            get { return _courses; }
            set
            {
                if (_courses == value)
                    return;
                if (_courses != null)
                    _courses.ForEach(v => v.PropertyChanged -= ChildPropertyChanged);
                _courses = value;
                if (_courses != null)
                    _courses.ForEach(v => v.PropertyChanged += ChildPropertyChanged);
                NotifyPropertyChanged(() => Courses);
                NotifyPropertyChanged(() => Childs);
            }
        }

        /// <summary>
        /// Название директория
        /// </summary>
        public string Name { get; set; }

        public IList<INode> Childs
        {
            get { return Folders.Cast<INode>().Concat(Courses).ToList(); }
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

                if (_isCheckChange)
                    return;

                _isCheckChange = true;

                NotifyPropertyChanged(() => IsChecked);

                if (!IsChecked.HasValue)
                    return;
                Childs.ForEach(v => v.IsChecked = IsChecked);
                Courses.ForEach(v => v.IsChecked = IsChecked);

                _isCheckChange = false;
            }
        }

        private void ChildPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (_isCheckChange || propertyChangedEventArgs.PropertyName != NameHelper<INode>.Name(v => v.IsChecked))
                return;
            _isCheckChange = true;

            var list = Folders.Cast<INode>().Concat(Courses).ToList();

            var isCheck = (bool?)list.All(v => v.IsChecked.HasValue && v.IsChecked.Value);

            if (!isCheck.Value)
            {
                isCheck = list.All(v => v.IsChecked.HasValue && !v.IsChecked.Value) ? false : (bool?)null;
            }

            IsChecked = isCheck;
            _isCheckChange = false;
            NotifyPropertyChanged(() => IsChecked);

        }
    }

    /// <summary>
    /// Модель курса
    /// </summary>
    public class CourseModel : ObservableObject, INode
    {
        private bool? _isChecked;
        private List<WaypointModel> _waypoints;

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }

        public IList<INode> Childs { get { return Waypoints.Cast<INode>().ToList(); } }

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

        /// <summary>
        /// Дочерняя папка
        /// </summary>
        public FolderModel Folder { get; set; }

        /// <summary>
        /// Точки маршрута
        /// </summary>
        public List<WaypointModel> Waypoints
        {
            get { return _waypoints; }
            set
            {
                if (_waypoints == value)
                    return;
                _waypoints = value;
                NotifyPropertyChanged(() => Childs);
            }
        }

        /// <summary>
        /// Линии маршрута
        /// </summary>
        public List<LineModel> Lines { get; set; }
    }

    /// <summary>
    /// Модель точки курса
    /// </summary>
    public class WaypointModel : ObservableObject, INode
    {
        static readonly NumberFormatInfo NumberFormatInfo = new NumberFormatInfo
        {
            NumberDecimalSeparator = "."
        };
        /// <summary>
        /// Дочерний маршрут
        /// </summary>
        public CourseModel Course { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Name
        {
            get
            {
                return string.Format("{0} {1}", Position.X.ToString("0.00", NumberFormatInfo), Position.Y.ToString("0.00", NumberFormatInfo));
            }
        }

        public IList<INode> Childs { get { return new INode[0]; } }

        public bool IsCollapsed { get; set; }

        public bool? IsChecked { get; set; }

        /// <summary>
        /// Координаты точки
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// Угол поворота
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// Скорость
        /// </summary>
        public int Speed { get; set; }

        /// <summary>
        /// Точка разрыва при построении комплексного пути
        /// </summary>
        public bool Crossing { get; set; }

        [DefaultValue(TurnStep.none)]
        public TurnStep Turn { get; set; }

        /// <summary>
        /// Начала шага маневра
        /// </summary>
        [DefaultValue(false)]
        public bool TurnStart { get; set; }

        /// <summary>
        /// Окончание шага маневра
        /// </summary>
        [DefaultValue(false)]
        public bool TurnEnd { get; set; }

        /// <summary>
        /// Точка ожидания
        /// </summary>
        [DefaultValue(false)]
        public bool Wait { get; set; }

        /// <summary>
        /// Движение задним ходом
        /// </summary>
        [DefaultValue(false)]
        public bool Revers { get; set; }


        /// <summary>
        /// Точка сгенерированна модом
        /// </summary>
        [DefaultValue(false)]
        public bool Generated { get; set; }

        /// <summary>
        /// Направление движения
        /// </summary>
        [XmlAttribute(attributeName: "dir")]
        [DefaultValue(Direction.none)]
        public Direction Direction { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        [DefaultValue(0)]
        public int RidgeMarker { get; set; }
    }

    /// <summary>
    /// Модель линии маршрута
    /// </summary>
    public class LineModel : ObservableObject
    {
        public CourseModel Course { get; set; }
        /// <summary>
        /// Начальная точка
        /// </summary>
        public WaypointModel StartPoint { get; set; }
        /// <summary>
        /// Конечная точка
        /// </summary>
        public WaypointModel EndPoint { get; set; }

    }
}