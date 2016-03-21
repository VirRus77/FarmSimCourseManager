using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AndBurn.DDSReader;
using FarmSimCourseManager.Contracts;
using FarmSimCourseManager.Model;
using FarmSimCourseManager.Tools;
using Brushes = System.Windows.Media.Brushes;
using Size = System.Windows.Size;

namespace FarmSimCourseManager.ViewModel
{
    [Export(typeof(MapControlViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MapControlViewModel : ObservableObject
    {
        private readonly IContent _content;
        private CourseFileModel _courseFileModel;
        private Bitmap _mapImage;

        [ImportingConstructor]
        public MapControlViewModel(IContent content)
        {
            _content = content;

            _content.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == NameHelper<IContent>.Name(v => v.MapFilePath))
                {
                    Bitmap bmp;

                    var extension = Path.GetExtension(_content.MapFilePath);
                    if (extension == null)
                        throw new NullReferenceException("File extension");

                    switch (extension.ToLower())
                    {
                        case ".dds":
                            var image = new DDSImage(new FileStream(_content.MapFilePath, FileMode.Open));
                            bmp = image.BitmapImage;
                            break;
                        case ".png":
                        case ".bmp":
                        case ".jpg":
                        case ".jpeg":
                            bmp = new Bitmap(_content.MapFilePath);
                            break;
                        default:
                            throw new NotSupportedException(string.Format("Не подъдерживаемый формат \"{0}\" файла.", extension));
                    }

                    MapImage = bmp;
                }
                if (args.PropertyName == NameHelper<IContent>.Name(v => v.CourseFileModel))
                {
                    //TestInBitmap();
                    CourseFileModel = _content.CourseFileModel;
                    NotifyPropertyChanged(() => Courses);
                }
            };
        }

        [Import]
        public ISettings Settings { get; set; }

        public CourseFileModel CourseFileModel
        {
            get { return _courseFileModel; }
            set
            {
                if (_courseFileModel == value)
                    return;
                if (_courseFileModel != null)
                    _courseFileModel.Courses.ForEach(v => v.PropertyChanged -= CoursePropertyChanged);
                _courseFileModel = value;
                if (_courseFileModel != null)
                    _courseFileModel.Courses.ForEach(v => v.PropertyChanged += CoursePropertyChanged);
                NotifyPropertyChanged(() => CourseFileModel);
            }
        }

        //private void TestInBitmap()
        //{
        //    var dateTime = DateTime.Now;

        //    var canvas = new Canvas();

        //    var path = new System.Windows.Shapes.Path();
        //    path.Stroke = Brushes.Red;
        //    path.StrokeThickness = 1;
        //    path.RenderTransform = new MatrixTransform
        //    {
        //        Matrix = new Matrix
        //        {
        //            OffsetX = 1000,
        //            OffsetY = 1000
        //        }
        //    };
        //    var pathGeometry = new PathGeometry();
        //    var pathFigure = new PathFigure();
        //    pathGeometry.Figures.Add(pathFigure);
        //    path.Data = pathGeometry;

        //    System.Windows.Point? point = null;
        //    foreach (var courseModel in _content.CourseFileModel.Courses)
        //    {
        //        if (courseModel.Waypoints.Count <= 1)
        //            continue;

        //        if (point != null)
        //        {
        //            pathFigure.Segments.Add(new LineSegment(courseModel.Waypoints.First().Position, false));
        //        }
        //        point = courseModel.Waypoints.First().Position;

        //        foreach (var waypointModel in courseModel.Waypoints.Skip(1))
        //        {
        //            pathFigure.Segments.Add(new LineSegment(waypointModel.Position, true));
        //            point = waypointModel.Position;
        //        }
        //    }

        //    canvas.Width = 2000;
        //    canvas.Height = 2000;

        //    canvas.Children.Add(path);

        //    canvas.Measure(new Size(canvas.Width, canvas.Height));
        //    canvas.Arrange(new Rect(0,0,canvas.Width, canvas.Height));

        //    var dpi = 96;

        //    var rtb = new RenderTargetBitmap(
        //        (int)canvas.Width, //width 
        //        (int)canvas.Height, //height 
        //        dpi, //dpi x 
        //        dpi, //dpi y 
        //        PixelFormats.Pbgra32 // pixelformat 
        //        );
        //    rtb.Render(canvas);

        //    var enc = new System.Windows.Media.Imaging.PngBitmapEncoder();
        //    enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(rtb));
        //    using (var stm = System.IO.File.Create("E:\\Temp\\FS2015\\test.png"))
        //    {
        //        enc.Save(stm);
        //    }

        //    var t = DateTime.Now - dateTime;
        //}

        public Bitmap MapImage
        {
            get { return _mapImage; }
            private set
            {
                if (_mapImage == value)
                    return;
                if (_mapImage != null)
                    _mapImage.Dispose();
                _mapImage = value;
                NotifyPropertyChanged(() => MapImage);
            }
        }


        public List<CourseModel> Courses { get { return _courseFileModel == null ? null : _courseFileModel.Courses.Where(v => v.IsChecked.HasValue && v.IsChecked.Value).ToList(); } }

        private void CoursePropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName != NameHelper<CourseModel>.Name(v => v.IsChecked))
                return;
            NotifyPropertyChanged(() => Courses);
        }
    }
}
