using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FarmSimCourseManager.Contracts;
using FarmSimCourseManager.Model;

namespace FarmSimCourseManager.Tools.Controls
{
    public class DrawElementMaps : FrameworkElement
    {
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale",
                typeof(double),
                typeof(DrawElementMaps),
                new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke",
                typeof(Color),
                typeof(DrawElementMaps),
                new FrameworkPropertyMetadata(Colors.Red, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness",
                typeof(double),
                typeof(DrawElementMaps),
                new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));

        public new static readonly DependencyProperty IsVisibleProperty =
            DependencyPropertyHelper<DrawElementMaps>.Register(
                v => v.IsVisible,
                (control, oldValue, newValue) =>
                {
                    control.Visibility = newValue ? Visibility.Visible : Visibility.Hidden;
                },
                true);

        public static readonly DependencyProperty MapSettingsProperty =
            DependencyPropertyHelper<DrawElementMaps>.Register(v => v.MapSettings, (control, oldValue, newValue) => control.OnSetMapSettings(control, oldValue, newValue));

        public new bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        public IMapSettings MapSettings
        {
            get { return (IMapSettings)GetValue(MapSettingsProperty); }
            set { SetValue(MapSettingsProperty, value); }
        }

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public Color Stroke
        {
            get { return (Color)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        protected virtual void OnSetMapSettings(DrawElementMaps control, IMapSettings oldValue, IMapSettings newValue)
        { }

        protected static Color Convert(System.Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }

    public class DrawCourse : DrawElementMaps
    {
        public static readonly DependencyProperty CourseModelProperty =
            DependencyProperty.Register("CourseModel",
                typeof(CourseModel),
                typeof(DrawCourse),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        #region Overrides of DrawElementMaps

        protected override void OnSetMapSettings(DrawElementMaps control, IMapSettings oldValue, IMapSettings newValue)
        {
            if (newValue == null)
                return;

            Stroke = Convert(newValue.CourseLineColor);
            StrokeThickness = newValue.CourseLineThickness;
        }

        #endregion

        public CourseModel CourseModel
        {
            get { return (CourseModel)GetValue(CourseModelProperty); }
            set { SetValue(CourseModelProperty, value); }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (CourseModel == null || CourseModel.Waypoints.Count <= 1)
                return;

            Console.WriteLine("OnRender");
#if !true
            var g = new StreamGeometry();
            var sgc = g.Open();

            Point? point = null;
            foreach (var courseModel in CourseFileModel.Courses)
            {
                point = null;
                if (courseModel.Waypoints.Count <= 1)
                    continue;

                //if (point != null)
                //{
                //    point = courseModel.Waypoints.First().Position;
                //    sgc.BeginFigure(point.Value, false, false);
                //    //pathFigure.Segments.Add(new LineSegment(courseModel.Waypoints.First().Position, false));
                //}
                //point = courseModel.Waypoints.First().Position;
                point = courseModel.Waypoints.First().Position;
                sgc.BeginFigure(point.Value, false, false);

                foreach (var waypointModel in courseModel.Waypoints.Skip(1))
                {
                    sgc.LineTo(waypointModel.Position, true, false);
                    //pathFigure.Segments.Add(new LineSegment(waypointModel.Position, true));
                    //point = waypointModel.Position;
                }
            }
            sgc.Close();
            var pen = new Pen(new SolidColorBrush(Stroke), StrokeThickness);
            drawingContext.DrawGeometry(null, pen, g);
            //Pen seriePen = new Pen(new SolidColorBrush(Stroke), 1.0);
            //bool firstPoint = true;
            //foreach (CurveValuePointVM pointVm in serieVm.Points.Cast<CurveValuePointVM>())
            //{
            //    if (pointVm.XValue < xMin || pointVm.XValue > xMax) continue;

            //    double x = basePoint.X + (pointVm.XValue - xMin) * xSizePerValue;
            //    double y = basePoint.Y - (pointVm.Value - yMin) * ySizePerValue;
            //    Point coord = new Point(x, y);

            //    if (firstPoint)
            //    {
            //        firstPoint = false;
            //        sgc.BeginFigure(coord, false, false);
            //    }
            //    else
            //    {
            //        sgc.LineTo(coord, true, false);
            //    }
            //}

            //sgc.Close();
            //dc.DrawGeometry(null, seriePen, g);
#else
            var pathGeometry = new PathGeometry();
            var pathFigure = new PathFigure()
            {
                StartPoint = CourseModel.Waypoints.First().Position,
                IsFilled = false,
                IsClosed = false
            };

            pathGeometry.Figures.Add(pathFigure);

            foreach (var waypointModel in CourseModel.Waypoints.Skip(1))
            {
                var lineSegment = new LineSegment(waypointModel.Position, true);
                lineSegment.Freeze();
                pathFigure.Segments.Add(lineSegment);
            }

            pathFigure.Freeze();

            var brush = new SolidColorBrush(Stroke);
            brush.Freeze();
            var pen = new Pen(brush, StrokeThickness / Scale);
            pathGeometry.Freeze();
            pen.Freeze();

            drawingContext.DrawGeometry(null, pen, pathGeometry);
#endif
        }
    }

    public class DrawMap : FrameworkElement
    {
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale",
                typeof(double),
                typeof(DrawMap),
                new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness",
                typeof(double),
                typeof(DrawMap),
                new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke",
                typeof(Color),
                typeof(DrawMap),
                new FrameworkPropertyMetadata(Colors.Red, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty CourseFileModelProperty =
            DependencyProperty.Register("CourseFileModel",
                typeof(CourseFileModel),
                typeof(DrawMap),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
        public Color Stroke
        {
            get { return (Color)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }
        public CourseFileModel CourseFileModel
        {
            get { return (CourseFileModel)GetValue(CourseFileModelProperty); }
            set { SetValue(CourseFileModelProperty, value); }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (CourseFileModel == null)
                return;
#if !true
            var g = new StreamGeometry();
            var sgc = g.Open();

            Point? point = null;
            foreach (var courseModel in CourseFileModel.Courses)
            {
                point = null;
                if (courseModel.Waypoints.Count <= 1)
                    continue;

                //if (point != null)
                //{
                //    point = courseModel.Waypoints.First().Position;
                //    sgc.BeginFigure(point.Value, false, false);
                //    //pathFigure.Segments.Add(new LineSegment(courseModel.Waypoints.First().Position, false));
                //}
                //point = courseModel.Waypoints.First().Position;
                point = courseModel.Waypoints.First().Position;
                sgc.BeginFigure(point.Value, false, false);

                foreach (var waypointModel in courseModel.Waypoints.Skip(1))
                {
                    sgc.LineTo(waypointModel.Position, true, false);
                    //pathFigure.Segments.Add(new LineSegment(waypointModel.Position, true));
                    //point = waypointModel.Position;
                }
            }
            sgc.Close();
            var pen = new Pen(new SolidColorBrush(Stroke), StrokeThickness);
            drawingContext.DrawGeometry(null, pen, g);
            //Pen seriePen = new Pen(new SolidColorBrush(Stroke), 1.0);
            //bool firstPoint = true;
            //foreach (CurveValuePointVM pointVm in serieVm.Points.Cast<CurveValuePointVM>())
            //{
            //    if (pointVm.XValue < xMin || pointVm.XValue > xMax) continue;

            //    double x = basePoint.X + (pointVm.XValue - xMin) * xSizePerValue;
            //    double y = basePoint.Y - (pointVm.Value - yMin) * ySizePerValue;
            //    Point coord = new Point(x, y);

            //    if (firstPoint)
            //    {
            //        firstPoint = false;
            //        sgc.BeginFigure(coord, false, false);
            //    }
            //    else
            //    {
            //        sgc.LineTo(coord, true, false);
            //    }
            //}

            //sgc.Close();
            //dc.DrawGeometry(null, seriePen, g);
#else
            //Console.WriteLine("{0} OnRender", DateTime.Now);
            var pathGeometry = new PathGeometry();
            var pathFigure = new PathFigure();
            pathGeometry.Figures.Add(pathFigure);

            Point? point = null;
            foreach (var courseModel in CourseFileModel.Courses)
            {
                if (courseModel.Waypoints.Count <= 1)
                    continue;

                if (point != null)
                {
                    pathFigure.Segments.Add(new LineSegment(courseModel.Waypoints.First().Position, false));
                }
                else
                {
                    pathFigure.StartPoint = courseModel.Waypoints.First().Position;
                }

                point = courseModel.Waypoints.First().Position;

                foreach (var waypointModel in courseModel.Waypoints.Skip(1))
                {
                    var lineSegment = new LineSegment(waypointModel.Position, true);
                    lineSegment.Freeze();
                    pathFigure.Segments.Add(lineSegment);
                    point = waypointModel.Position;
                }
            }

            pathFigure.Freeze();

            var solidColorBrush = new SolidColorBrush(Stroke);
            solidColorBrush.Freeze();

            var pen = new Pen(solidColorBrush, StrokeThickness);
            pen.Freeze();
            pathGeometry.Freeze();

            drawingContext.DrawGeometry(null, pen, pathGeometry);
#endif
        }
    }

    public class DrawTest : DrawMap
    {
        /// <summary>
        /// Тормозит
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            //OnRender1(drawingContext);
            //return;

            if (CourseFileModel == null
                || CourseFileModel.Courses.Count == 0
                || !CourseFileModel.Courses.Any(v => v.Waypoints.Count > 1))
            {
                return;
            }

            // Точки маршрутов
            var coursePoints = CourseFileModel.Courses
                .Select(courseModel => courseModel.Waypoints
                    .Select(waypointModel => waypointModel.Position)
                    .ToList())
                .ToList();
            var points = coursePoints.SelectMany(v => v).ToList();
            var rect = new Rect(
                        new Point(points.Min(v => v.X), points.Min(v => v.Y)),
                        new Point(points.Max(v => v.X), points.Max(v => v.Y)));

            var matrix = new Matrix();
            matrix.Scale(Scale, Scale);

            rect.Transform(matrix);

            var mapPath = GetMap(matrix);

            var sizeWindow = 512;
            var dpi = 96;

            var endX = IndexRB(rect.X + rect.Width, sizeWindow);
            var endY = IndexRB(rect.Y + rect.Height, sizeWindow);
            var startY = IndexLT(rect.Y, sizeWindow);

            for (var x = IndexLT(rect.X, sizeWindow); x < endX; x++)
            {
                for (var y = startY; y < endY; y++)
                {
                    var rtb = new RenderTargetBitmap(sizeWindow, sizeWindow, dpi, dpi, PixelFormats.Pbgra32);

                    mapPath.RenderTransform = new TranslateTransform(-1 * x * sizeWindow, -1 * y * sizeWindow);
                    mapPath.Measure(new Size(sizeWindow, sizeWindow));
                    mapPath.Arrange(new Rect(0, 0, sizeWindow, sizeWindow));

                    rtb.Render(mapPath);
                    rtb.Freeze();

                    drawingContext.DrawImage(rtb, new Rect(x * sizeWindow, y * sizeWindow, sizeWindow, sizeWindow));
                    drawingContext.DrawRectangle(null, new Pen(Brushes.Aqua, 1), new Rect(x * sizeWindow, y * sizeWindow, sizeWindow, sizeWindow));
                }
            }
        }

        private void OnRender1(DrawingContext drawingContext)
        {
            if (CourseFileModel == null
                || CourseFileModel.Courses.Count == 0
                || !CourseFileModel.Courses.Any(v => v.Waypoints.Count > 1))
            {
                return;
            }

            var coursePoints = CourseFileModel.Courses
                .Select(courseModel => courseModel.Waypoints
                    .Select(waypointModel => waypointModel.Position)
                    .ToList())
                .ToList();

            var rect = new Rect(
                new Point(coursePoints.SelectMany(v => v).Min(v => v.X),
                    coursePoints.SelectMany(v => v).Min(v => v.Y)),
                new Point(coursePoints.SelectMany(v => v).Max(v => v.X),
                    coursePoints.SelectMany(v => v).Max(v => v.Y)));
            //wb = new WriteableBitmap((int)ActualWidth, (int)ActualHeight, 96, 96, PixelFormats.Pbgra32, null);

            var dpi = 96;

            //var bmp = new WriteableBitmap(
            //    2000,
            //    2000,
            //    //(int) ((size.Width + 0.9d).Floor()),
            //    //(int) ((size.Height + 0.9d).Floor()),
            //    dpi,
            //    dpi,
            //    PixelFormats.Pbgra32,
            //    null);
            //bmp.Lock();

            var rtb = new RenderTargetBitmap(2000, 2000, dpi, dpi, PixelFormats.Pbgra32);

            var matrix = new Matrix()
            {
                OffsetX = 1000,
                OffsetY = 1000
            };

            var path = new Path();
            var visual = GetMap(matrix);
            visual.Measure(new Size(rect.Width, rect.Height));
            visual.Arrange(new Rect(0, 0, rect.Width, rect.Height));

            rtb.Render(visual);

            //rtb.CopyPixels(new Int32Rect(0, 0, rtb.PixelWidth, rtb.PixelHeight),
            //bmp.BackBuffer,
            //bmp.BackBufferStride * bmp.PixelHeight, bmp.BackBufferStride);

            //bmp.AddDirtyRect(new Int32Rect(0, 0, 2000, 2000));
            //bmp.Unlock();

            drawingContext.DrawImage(rtb, new Rect(-1000, -1000, 2000, 2000));
        }

        Path GetMap(Matrix matrix)
        {
            var pathGeometry = new PathGeometry();
            var pathFigure = new PathFigure();
            pathGeometry.Figures.Add(pathFigure);

            Point? point = null;
            foreach (var courseModel in CourseFileModel.Courses)
            {
                if (courseModel.Waypoints.Count <= 1)
                    continue;

                if (point != null)
                {
                    pathFigure.Segments.Add(new LineSegment(matrix.Transform(courseModel.Waypoints.First().Position),
                        false));
                }
                else
                {
                    pathFigure.StartPoint = matrix.Transform(courseModel.Waypoints.First().Position);
                }
                point = matrix.Transform(courseModel.Waypoints.First().Position);

                foreach (var waypointModel in courseModel.Waypoints.Skip(1))
                {
                    pathFigure.Segments.Add(new LineSegment(matrix.Transform(waypointModel.Position), true));
                    point = matrix.Transform(waypointModel.Position);
                }
            }

            var solidColorBrush = new SolidColorBrush(Stroke);
            solidColorBrush.Freeze();

            return new Path()
            {
                Stroke = solidColorBrush,
                StrokeThickness = StrokeThickness,
                Data = pathGeometry
            };
        }

        private static int IndexLT(double point, int size)
        {
            var index = (int)(point / size);
            return point < 0 ? index - 1 : index;
        }
        private static int IndexRB(double point, int size)
        {
            var index = (int)(point / size);
            return point > 0 ? index + 1 : index;
        }
    }

    public class DrawPointCourse : DrawElementMaps
    {
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius",
                typeof(double),
                typeof(DrawPointCourse),
                new FrameworkPropertyMetadata(3d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty CourseModelProperty =
            DependencyProperty.Register("CourseModel",
                typeof(CourseModel),
                typeof(DrawPointCourse),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public CourseModel CourseModel
        {
            get { return (CourseModel)GetValue(CourseModelProperty); }
            set { SetValue(CourseModelProperty, value); }
        }

        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (CourseModel == null || CourseModel.Waypoints.Count <= 1)
                return;

            var brush = new SolidColorBrush(Stroke);
            brush.Freeze();
            var pen = new Pen(brush, StrokeThickness / Scale);
            pen.Freeze();

            foreach (var point in CourseModel.Waypoints)
            {
                drawingContext.DrawEllipse(null, pen, point.Position, Radius / Scale, Radius / Scale);
            }
            //var pathGeometry = new PathGeometry();
            //var pathFigure = new PathFigure();
            //pathGeometry.Figures.Add(pathFigure);

            //var points = CourseFileModel.Courses.SelectMany(v => v.Waypoints).Select(v => v.Position).ToList();
            //if (points.Count == 0)
            //    return;

            ////Point? point = waypointModels.First().Position;
            ////Point? point = null;
            ////pathFigure.Segments.Add(new ArcSegment(waypointModel.Position, new Size(Radius * 2, Radius * 2), Math.PI, true, SweepDirection.Clockwise, true));
            ////pathGeometry.Figures.Add(pathFigure);
            //var diametr = 2 * Radius;
            //foreach (var pointModel in points)
            //{
            //    //if (point != null)
            //    var startCircle = new Point(pointModel.X - Radius, pointModel.Y);
            //    pathFigure.Segments.Add(new LineSegment(startCircle, false));
            //    pathFigure.Segments.Add(new ArcSegment(new Point(startCircle.X + diametr, startCircle.Y), new Size(Radius * 2, Radius * 2), 0, true, SweepDirection.Counterclockwise, true));
            //    pathFigure.Segments.Add(new ArcSegment(startCircle, new Size(Radius * 2, Radius * 2), 0, true, SweepDirection.Counterclockwise, true));

            //    //var pathFigure = new PathFigure();
            //    //pathFigure.Segments.Add(new ArcSegment(waypointModel.Position, new Size(Radius * 2, Radius * 2), Math.PI, true, SweepDirection.Clockwise, true));
            //    //pathGeometry.Figures.Add(pathFigure);
            //    //if (courseModel.Waypoints.Count <= 1)
            //    //    continue;

            //    //if (point != null)
            //    //{
            //    //    pathFigure.Segments.Add(new LineSegment(courseModel.Waypoints.First().Position, false));
            //    //}
            //    //point = courseModel.Waypoints.First().Position;

            //    //foreach (var waypointModel in courseModel.Waypoints.Skip(1))
            //    //{
            //    //    pathFigure.Segments.Add(new LineSegment(waypointModel.Position, true));
            //    //    point = waypointModel.Position;
            //    //}
            //}
            //var pen = new Pen(new SolidColorBrush(Stroke), StrokeThickness);
            //drawingContext.DrawGeometry(null, pen, pathGeometry);
        }

        #region Overrides of DrawElementMaps

        protected override void OnSetMapSettings(DrawElementMaps control, IMapSettings oldValue, IMapSettings newValue)
        {
            if (newValue == null)
                return;

            var binding = new Binding
            {
                Source = newValue,
                Path = new PropertyPath(NameHelper<IMapSettings>.Name(v => v.PointLineColor)),
                Converter = new DrawingColorToMediaColorConverter(),
                Mode = BindingMode.OneWay
            };
            SetBinding(StrokeProperty, binding);
            //Stroke = Convert(newValue.PointLineColor);
            StrokeThickness = newValue.PointLineThickness;
            Radius = newValue.PointCircleRadius;
        }

        #endregion
    }

    public class DrawingColorToMediaColorConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is System.Drawing.Color ? Convert((System.Drawing.Color)value) : (object)null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is Color) ? (object)null : Convert((Color)value);
        }

        #endregion

        public static Color Convert(System.Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        protected static System.Drawing.Color Convert(Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
