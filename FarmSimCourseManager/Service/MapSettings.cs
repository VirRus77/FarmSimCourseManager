using System;
using System.Drawing;
using System.Xml.Serialization;
using FarmSimCourseManager.Contracts;
using FarmSimCourseManager.Tools;

namespace FarmSimCourseManager.Service
{
    [Serializable]
    public class MapSettings : ObservableObject, IMapSettings
    {
        private ImageBinary _imageStop;
        private ImageBinary _imageStart;
        private Color _courseLineColor;
        private int _courseLineThickness;
        private Color _pointLineColor;
        private int _pointLineThickness;
        private int _pointCircleRadius;

        public MapSettings()
        {
            CourseLineColor = Color.Red;
            CourseLineThickness = 1;

            PointLineColor = Color.Blue;
            PointLineThickness = 1;
            PointCircleRadius = 3;
        }

        #region Implementation of IMapSettings

        [XmlElement]
        public ImageBinary ImageStop
        {
            get { return _imageStop; }
            set
            {
                if (_imageStop == value)
                    return;
                _imageStop = value;
                NotifyPropertyChanged(() => ImageStop);
            }
        }

        [XmlElement]
        public ImageBinary ImageStart
        {
            get { return _imageStart; }
            set
            {
                if (_imageStart == value)
                    return;
                _imageStart = value;
                NotifyPropertyChanged(() => ImageStart);
            }
        }

        [XmlAttribute("CourseLineColor")]
        public string CourseLineColorXml
        {
            get { return XmlColor.Convert(CourseLineColor); }
            set { CourseLineColor = XmlColor.Convert(value); }
        }

        [XmlIgnore]
        public Color CourseLineColor
        {
            get { return _courseLineColor; }
            set
            {
                if (_courseLineColor == value)
                    return;
                _courseLineColor = value;
                NotifyPropertyChanged(() => CourseLineColor);
            }
        }

        [XmlAttribute]
        public int CourseLineThickness
        {
            get { return _courseLineThickness; }
            set
            {
                if (_courseLineThickness == value)
                    return;
                _courseLineThickness = value;
                NotifyPropertyChanged(() => CourseLineThickness);
            }
        }

        [XmlAttribute("PointLineColor")]
        public string PointLineColorXml
        {
            get { return XmlColor.Convert(PointLineColor); }
            set { PointLineColor = XmlColor.Convert(value); }
        }

        [XmlIgnore]
        public Color PointLineColor
        {
            get { return _pointLineColor; }
            set
            {
                if (_pointLineColor == value)
                    return;
                _pointLineColor = value;
                NotifyPropertyChanged(() => PointLineColor);
            }
        }

        [XmlAttribute]
        public int PointLineThickness
        {
            get { return _pointLineThickness; }
            set
            {
                if (_pointLineThickness == value)
                    return;
                _pointLineThickness = value;
                NotifyPropertyChanged(() => PointLineThickness);
            }
        }

        [XmlAttribute]
        public int PointCircleRadius
        {
            get { return _pointCircleRadius; }
            set
            {
                if (_pointCircleRadius == value)
                    return;
                _pointCircleRadius = value;
                NotifyPropertyChanged(() => PointCircleRadius);
            }
        }

        #endregion
    }
}