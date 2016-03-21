using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using FarmSimCourseManager.Tools.CanvasEx;

namespace FarmSimCourseManager.Tools.Controls
{
    public partial class BrowserControl : Canvas
    {
        #region Вспомогательные классы

        /// <summary>
        /// Инструмент для отработки нажатия.
        /// </summary>
        private class ClickInstrument : IInstrument
        {
            private readonly Point _downPoint;
            private readonly DateTime _downTime;
            private readonly WeakEvent<MouseButtonEventHandler> _mouseClickEvent;

            public ClickInstrument(Point downPoint, WeakEvent<MouseButtonEventHandler> mouseClickEvent)
            {
                _downPoint = downPoint;
                _mouseClickEvent = mouseClickEvent;
                _downTime = DateTime.Now;
            }

            public void Dispose()
            { }


            public void OnUp(Point upPoint, MouseButtonEventArgs mouseButtonEventArgs)
            {
                var posDelta = upPoint - _downPoint;
                var timeSpan = DateTime.Now - _downTime;
                if (Math.Abs(posDelta.X) < 5 && Math.Abs(posDelta.Y) < 5 && timeSpan.TotalSeconds < 3)
                    _mouseClickEvent.Raise(this, mouseButtonEventArgs);
            }

            public void OnMove(Point movePoint, MouseEventArgs mouseEventArgs)
            { }
        }

        /// <summary>
        /// Инструмент для сдвига видимой области.
        /// </summary>
        private class PanInstrument : IInstrument
        {
            private Point _prevPoint;
            private readonly UIElement _container;

            public PanInstrument(Point prevPoint, UIElement container)
            {
                _prevPoint = prevPoint;
                _container = container;
            }

            public void Dispose()
            { }


            public void OnUp(Point upPoint, MouseButtonEventArgs mouseButtonEventArgs)
            { }

            public void OnMove(Point movePoint, MouseEventArgs mouseEventArgs)
            {
                var delta = movePoint - _prevPoint;
                _prevPoint = movePoint;
                var top = GetTop(_container);
                if (double.IsNaN(top))
                    top = 0;
                var left = GetLeft(_container);
                if (double.IsNaN(left))
                    left = 0;
                SetTop(_container, top + delta.Y);
                SetLeft(_container, left + delta.X);
            }
        }

        #endregion

        #region DependencyProperty

        public static readonly DependencyProperty MovedScaleElementProperty =
            DependencyPropertyHelper<BrowserControl>.Register(o => o.MovedScaleElement, MovedElementPropertyChanged);
        public static readonly DependencyProperty MovedElementProperty =
            DependencyPropertyHelper<BrowserControl>.Register(o => o.MovedElement, MovedElementPropertyChanged);
        public static readonly DependencyProperty ZoomPowProperty =
            DependencyPropertyHelper<BrowserControl>.Register(o => o.ZoomPow, defaultValue: 2.0);
        public static readonly DependencyProperty ScaleProperty =
            DependencyPropertyHelper<BrowserControl>.Register(o => o.Scale, defaultValue: 2.0);
        public static readonly DependencyProperty ScaleTargetProperty =
            DependencyPropertyHelper<BrowserControl>.Register(o => o.ScaleTarget, OnScaleTargetPropertyChanged);
        private static readonly DependencyPropertyKey CursorXPropertyKey =
            DependencyProperty.RegisterReadOnly("CursorX", typeof(double), typeof(BrowserControl), new PropertyMetadata(default(double)));
        private static readonly DependencyProperty CursorXProperty =
            CursorXPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey CursorYPropertyKey =
            DependencyProperty.RegisterReadOnly("CursorY", typeof(double), typeof(BrowserControl), new PropertyMetadata(default(double)));
        private static readonly DependencyProperty CursorYProperty =
            CursorYPropertyKey.DependencyProperty;
        public static readonly RoutedEvent MouseClickEvent =
            EventManager.RegisterRoutedEvent("MouseClickCommand", RoutingStrategy.Direct, typeof(MouseButtonEventHandler), typeof(BrowserControl));
        public static readonly DependencyProperty InstrumentsProperty =
            DependencyPropertyHelper<BrowserControl>.Register(o => o.Instruments, OnInstrumentsChanged);
        public static readonly DependencyProperty ScaleOnResizeProperty =
            DependencyProperty.Register("ScaleOnResize", typeof(bool), typeof(BrowserControl), new PropertyMetadata(default(bool)));
        public static readonly DependencyProperty ZoomCenterCommandProperty =
            DependencyPropertyHelper<BrowserControl>.Register(v => v.ZoomCenterCommand);
        public static readonly DependencyProperty MaxZoomInProperty =
            DependencyPropertyHelper<BrowserControl>.Register(o => o.MaxZoomIn, defaultValue: 2);
        
        #endregion

        private readonly WeakEvent<MouseButtonEventHandler> _mouseClickEvent;
        private readonly IList<IInstrument> _instruments;
        private double _scaleFactor;

        private Popup _popup;

        public BrowserControl()
        {
            InitializeComponent();
            if (DesignTimeProperty.InDesignMode)
                return;
            _mouseClickEvent = new WeakEvent<MouseButtonEventHandler>();
            MouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseLeftButtonUp += OnMouseLeftButtonUp;
            MouseMove += OnMouseMove;
            LostMouseCapture += OnLostMouseCapture;
            MouseWheel += OnMouseWheel;
            SizeChanged += (sender, args) =>
            {
                if (ScaleOnResize)
                    ScaleInCenter();
            };

            //_popup = new Popup();
            //IsManipulationEnabled = true;
            //ManipulationDelta += OnManipulationDelta;

            _scaleFactor = 0;
            _instruments = new List<IInstrument>();
            UpdateScale();

            ZoomInCommand = new RelayCommand(() => ZoomIn());
            ZoomOutCommand = new RelayCommand(() => ZoomOut());
            ZoomCenterCommand = new RelayCommand(() => ScaleInCenter());
        }

        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }

        public ICommand ZoomCenterCommand
        {
            get { return (ICommand)GetValue(ZoomCenterCommandProperty); }
            set { SetValue(ZoomCenterCommandProperty, value); }
        }

        public bool ScaleOnResize
        {
            get { return (bool)GetValue(ScaleOnResizeProperty); }
            set { SetValue(ScaleOnResizeProperty, value); }
        }

        public FrameworkElement MovedScaleElement
        {
            get { return (FrameworkElement)GetValue(MovedScaleElementProperty); }
            set { SetValue(MovedScaleElementProperty, value); }
        }

        public FrameworkElement MovedElement
        {
            get { return (FrameworkElement)GetValue(MovedElementProperty); }
            set { SetValue(MovedElementProperty, value); }
        }

        public double ZoomPow
        {
            get { return (double)GetValue(ZoomPowProperty); }
            set { SetValue(ZoomPowProperty, value); }
        }

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public IScaleTarget ScaleTarget
        {
            get { return (IScaleTarget)GetValue(ScaleTargetProperty); }
            set { SetValue(ScaleTargetProperty, value); }
        }

        public double CursorX
        {
            get { return (double)GetValue(CursorXProperty); }
        }

        public double CursorY
        {
            get { return (double)GetValue(CursorYProperty); }
        }

        public int MaxZoomIn
        {
            get { return (int)GetValue(MaxZoomInProperty); }
            set { SetValue(MaxZoomInProperty, value); }
        }

        public InstrumentActivator Instruments
        {
            get { return (InstrumentActivator)GetValue(InstrumentsProperty); }
            set { SetValue(InstrumentsProperty, value); }
        }

        public event MouseButtonEventHandler MouseClick
        {
            add { _mouseClickEvent.Add(value); }
            remove { _mouseClickEvent.Remove(value); }
        }


        private static void MovedElementPropertyChanged(BrowserControl me, FrameworkElement oldValue, FrameworkElement newValue)
        {
            if (oldValue != null)
                oldValue.SizeChanged -= me.control_SizeChanged;
            if (newValue != null)
                newValue.SizeChanged += me.control_SizeChanged;
        }

        private static void OnScaleTargetPropertyChanged(BrowserControl me, IScaleTarget oldValue, IScaleTarget newValue)
        {
            if (newValue != null)
                newValue.SetScale(me.Scale);
        }

        private static void OnInstrumentsChanged(BrowserControl me, InstrumentActivator oldValue, InstrumentActivator newValue)
        {
            if (oldValue != null)
                oldValue.Started -= me.OnInstrumentStarted;
            if (newValue != null)
                newValue.Started += me.OnInstrumentStarted;
        }

        private void control_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScaleInCenter();
        }

        public void ScaleInCenter()
        {
            var elements = new List<FrameworkElement>();
            if (MovedScaleElement != null)
                elements.Add(MovedScaleElement);
            if (MovedElement != null)
                elements.Add(MovedElement);
            var size = elements.GetMaxActualSize();
            if (size.IsEmpty)
                return;

            var newScale = Math.Min(ActualWidth / size.Width, ActualHeight / size.Height);
            _scaleFactor = Math.Log(newScale, ZoomPow);
            UpdateScale();
            SetCenter(new Point(size.Width / 2, size.Height / 2), true);
        }

        public void SetCenter(Point point, bool useScale = false)
        {
            var delta = new Vector(point.X, point.Y);
            if (useScale)
                delta *= Math.Pow(ZoomPow, _scaleFactor);
            var center = new Vector(ActualWidth / 2d, ActualHeight / 2d) - delta;
            SetTop(Container, center.Y);
            SetLeft(Container, center.X);
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs mouseWheelEventArgs)
        {
            var value = mouseWheelEventArgs.Delta / Mouse.MouseWheelDeltaForOneLine;
            var mapPoint = (Vector)Mouse.GetPosition(this);

            Zoom(mapPoint, value);
        }

        private void UpdateScale()
        {
            var scale = Math.Pow(ZoomPow, _scaleFactor);
            ScaleContainer.LayoutTransform = new ScaleTransform(scale, scale);
            Scale = scale;
            var scaleTarget = ScaleTarget;
            if (scaleTarget != null)
                scaleTarget.SetScale(scale);
        }

        private void OnLostMouseCapture(object sender, MouseEventArgs mouseEventArgs)
        {
            StopInstruments();
        }

        private void OnInstrumentStarted(IInstrument instrument)
        {
            if (_instruments.Count == 0)
                Mouse.Capture(this, CaptureMode.SubTree);
            _instruments.Add(instrument);
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Mouse.Capture(this, CaptureMode.SubTree);
            var downPoint = Mouse.GetPosition(this);
            if (_instruments.Count != 0)
                return;

            _instruments.Add(new ClickInstrument(downPoint, _mouseClickEvent));
            _instruments.Add(new PanInstrument(downPoint, Container));
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (_instruments.Count == 0)
                return;
            var upPoint = Mouse.GetPosition(this);
            foreach (var instrument in _instruments)
                instrument.OnUp(upPoint, mouseButtonEventArgs);
            StopInstruments();
            ReleaseMouseCapture();
        }

        private void StopInstruments()
        {
            foreach (var instrument in _instruments)
                instrument.Dispose();
            _instruments.Clear();
        }

        public void CenterSelectedFeature(Path path)
        {
            var cc = ScaleContainer;

            // Габаритные размеры выбранного объекта
            path.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var bound = path.Data.Bounds;
            var x = bound.X + bound.Width / 2.0;
            var y = bound.Y + bound.Height / 2.0;
            var delta = new Vector(x, y);

            var left = x - bound.Width;
            var right = x + bound.Width;
            var top = y - bound.Height;
            var botton = y + bound.Height;
            var newScale = Math.Min(ActualWidth / (right - left), ActualHeight / (botton - top));
            _scaleFactor = Math.Log(newScale, ZoomPow);
            UpdateScale();

            delta *= Math.Pow(ZoomPow, _scaleFactor);

            var center = new Vector(ActualWidth / 2d, ActualHeight / 2d) - delta;
            SetTop(Container, center.Y);
            SetLeft(Container, center.X);
        }

        public void ScaleFeatureToCenter(Path path)
        {
            // Габаритные размеры и положение плана карты относительно окна просмотра
            var parentW = ActualWidth;
            var parentH = ActualHeight;

            var uiElement = Container;
            var top = GetTop(uiElement);
            if (double.IsNaN(top))
                top = 0;
            var left = GetLeft(uiElement);
            if (double.IsNaN(left))
                left = 0;

            var cc = ScaleContainer;
            var scaleW = cc.RenderSize.Width / cc.DesiredSize.Width;
            var scaleH = cc.RenderSize.Height / cc.DesiredSize.Height;

            // Габаритные размеры выбранного объекта
            path.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var bound = path.Data.Bounds;

            try
            {
                // Проверяем попадание выделенного объекта в видимую часть плана
                var pathRect = new Rect(bound.X / scaleW + left, bound.Y / scaleH + top, bound.Width / scaleW, bound.Height / scaleH);
                var uiElementRect = new Rect(0, 0, parentW, parentH);

                // Если объект находится вне поля зрения - сдвинуть план, чтобы показать выбранный объект
                if (uiElementRect.Contains(pathRect) == false)
                {
                    var x = (bound.X + bound.Width);
                    var y = (bound.Y + bound.Height);
                    SetCenter(new Point(x, y), true);
                }
            }
            catch (Exception) { }
        }

        private void OnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            // TODO: перенести туда, где изменяется.
            var mousePosition = Mouse.GetPosition(this);

            var uiElement = Container;
            var top = GetTop(uiElement);
            if (double.IsNaN(top))
                top = 0;
            var left = GetLeft(uiElement);
            if (double.IsNaN(left))
                left = 0;

            SetValue(CursorXPropertyKey, (mousePosition.X - left) / Scale);
            SetValue(CursorYPropertyKey, (mousePosition.Y - top) / Scale);

            foreach (var instrument in _instruments)
                instrument.OnMove(mousePosition, mouseEventArgs);
        }

        private void ZoomIn()
        {
            Zoom(new Vector(ActualWidth / 2d, ActualHeight / 2d), 1);
        }

        private void ZoomOut()
        {
            Zoom(new Vector(ActualWidth / 2d, ActualHeight / 2d), -1);
        }

        private void Zoom(Vector canvasPoint, double scaleFactorDelta)
        {
            var value = scaleFactorDelta;

            if (_scaleFactor + value > MaxZoomIn)
                return;

            var uiElement = Container;
            var top = GetTop(uiElement);
            if (double.IsNaN(top))
                top = 0;
            var left = GetLeft(uiElement);
            if (double.IsNaN(left))
                left = 0;
            var LTi = new Vector(left, top);
            var pointImage = canvasPoint - LTi;
            pointImage /= Math.Pow(ZoomPow, _scaleFactor);

            _scaleFactor += value;

            pointImage *= Math.Pow(ZoomPow, _scaleFactor);

            UpdateScale();
            SetTop(uiElement, canvasPoint.Y - pointImage.Y);
            SetLeft(uiElement, canvasPoint.X - pointImage.X);
        }

        private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs manipulationDeltaEventArgs)
        {
            var delta = manipulationDeltaEventArgs.DeltaManipulation;
            var sb = new StringBuilder();

            sb.AppendLine("Rotation:    " + delta.Rotation);
            sb.AppendLine("Translation: " + delta.Translation.ToString());
            sb.AppendLine("Expansion:   " + delta.Expansion.ToString());
            sb.AppendLine("Scale:       " + delta.Scale.ToString());

            _popup.DataContext = sb.ToString();

            //if (Math.Abs(delta.Scale.Length) < 0.001d)
            //    return;

            //var scale = Scale * delta.Scale.Length;
            //_scaleFactor = Math.Log(scale, ZoomPow);
            //UpdateScale();
            ////ScaleContainer.LayoutTransform = new ScaleTransform(scale, scale);
        }
    }
}