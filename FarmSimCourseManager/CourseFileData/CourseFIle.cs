using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Xml.Serialization;

namespace FarmSimCourseManager.CourseFileData
{
    [Serializable]
    [XmlRoot("XML")]
    public class CourseFile : CourseHeader
    {
        /// <summary>
        /// Папки
        /// </summary>
        [XmlArray(elementName: "folders")]
        [XmlArrayItem(ElementName = "folder")]
        public List<Folder> Folders { get; set; }

        /// <summary>
        /// Маршруты
        /// </summary>
        [XmlArray(elementName: "courses")]
        [XmlArrayItem(ElementName = "course")]
        public List<Course> Courses { get; set; }
    }

    [Serializable]
    public class CourseHeader
    {
        /// <summary>
        /// ???
        /// ? - Смещение маршрута относительно подложки "миникарта"
        /// </summary>
        [XmlElement(elementName: "courseplayHud")]
        public CourseplayHud CourseplayHud { get; set; }

        /// <summary>
        /// ???
        /// ? - Настройка сканирования полей
        /// </summary>
        [XmlElement(elementName: "courseplayFields")]
        public CourseplayFields CourseplayFields { get; set; }

        /// <summary>
        /// ???
        /// ? - Оплата труда
        /// </summary>
        [XmlElement(elementName: "courseplayWages")]
        public CourseplayWages CourseplayWages { get; set; }

        /// <summary>
        /// ???
        /// ? - Настройка отображения маршрутов на карте "миникарта"
        /// </summary>
        [XmlElement(elementName: "courseplayIngameMap")]
        public CourseplayInGameMap CourseplayInGameMap { get; set; }

        /// <summary>
        /// ???
        /// ? - Размер записываемых точек
        /// </summary>
        [XmlElement(elementName: "courseManagement")]
        public CourseManagement CourseManagement { get; set; }

        /// <summary>
        /// ???
        /// ? - Настройка карты "контроль маршрута"
        /// </summary>
        [XmlElement(elementName: "course2D")]
        public Course2D Course2D { get; set; }
    }

    /// <summary>
    /// ???
    /// ? - Смещение маршрута относительно подложки "миникарта"
    /// </summary>
    [Serializable]
    public class CourseplayHud
    {
        /// <summary>
        /// ???
        /// ? - Смещение по X
        /// </summary>
        [XmlAttribute(attributeName: "posX")]
        public float PosX { get; set; }

        /// <summary>
        /// ???
        /// ? - Смещение по Y
        /// </summary>
        [XmlAttribute(attributeName: "posY")]
        public float PosY { get; set; }
    }

    /// <summary>
    /// ???
    /// ? - Настройка сканирования полей
    /// </summary>
    [Serializable]
    public class CourseplayFields
    {
        // "true"
        /// <summary>
        /// ???
        /// ? - Автоматическое сканирование полей
        /// </summary>
        [XmlAttribute(attributeName: "automaticScan")]
        public bool AutomaticScan { get; set; }

        // "true"
        /// <summary>
        /// ???
        /// ? - Сканировать только поля принадлежащие игроку
        /// </summary>
        [XmlAttribute(attributeName: "onlyScanOwnedFields")]
        public bool OnlyScanOwnedFields { get; set; }

        // "false"
        /// <summary>
        /// ???
        /// ? - Отладка просканированных полей
        /// </summary>
        [XmlAttribute(attributeName: "debugScannedFields")]
        public bool DebugScannedFields { get; set; }

        // "false"
        /// <summary>
        /// ???
        /// ? - Отладка ручных загруженных полей
        /// </summary>
        [XmlAttribute(attributeName: "debugCustomLoadedFields")]
        public bool DebugCustomLoadedFields { get; set; }

        // "5"
        /// <summary>
        /// ???
        /// ? - Шаг сканирования
        /// </summary>
        [XmlAttribute(attributeName: "scanStep")]
        public int ScanStep { get; set; }
    }

    /// <summary>
    /// ???
    /// ? - Оплата труда
    /// </summary>
    [Serializable]
    public class CourseplayWages
    {
        // "false"
        /// <summary>
        /// Активирована оплата труда.
        /// </summary>
        [XmlAttribute(attributeName: "active")]
        public bool Active { get; set; }

        // "1500"
        /// <summary>
        /// Стоимость оплаты труда
        /// </summary>
        [XmlAttribute(attributeName: "wagePerHour")]
        public int WagePerHour { get; set; }
    }

    /// <summary>
    /// ???
    /// ? - Настройка отображения маршрутов на карте "миникарта"
    /// </summary>
    [Serializable]
    public class CourseplayInGameMap
    {
        // "true"
        /// <summary>
        /// ???
        /// </summary>
        [XmlAttribute(attributeName: "active")]
        public bool Active { get; set; }
        
        // "true"
        /// <summary>
        /// ???
        /// ? - Показывать имена
        /// </summary>
        [XmlAttribute(attributeName: "showName")]
        public bool ShowName { get; set; }
        
        // "true"
        /// <summary>
        /// ???
        /// ? - Показывать маршрут
        /// </summary>
        [XmlAttribute(attributeName: "showCourse")]
        public bool ShowCourse { get; set; }
    }

    /// <summary>
    /// ???
    /// ? - Размер записываемых точек
    /// </summary>
    [Serializable]
    public class CourseManagement
    {
        // "4096"
        /// <summary>
        /// ???
        /// ? - Размер максимально записываемого маршрута
        /// </summary>
        [XmlAttribute(attributeName: "batchWriteSize")]
        public string BatchWriteSize { get; set; }
    }

    /// <summary>
    /// ???
    /// ? - Настройка карты "контроль маршрута"
    /// </summary>
    [Serializable]
    public class Course2D
    {
        // "0.650"
        /// <summary>
        /// ???
        /// ? - Позиция карты "контроль маршрута" на экране
        /// </summary>
        [XmlAttribute(attributeName: "posX")]
        public float PositionX { get; set; }
        
        // "0.350"
        /// <summary>
        /// ???
        /// ? - Позиция карты "контроль маршрута" на экране
        /// </summary>
        [XmlAttribute(attributeName: "posY")]
        public float PositionY { get; set; }
        
        // "0.70"
        /// <summary>
        /// ???
        /// ? - Прозрачность карты "контроль маршрута"
        /// </summary>
        [XmlAttribute(attributeName: "opacity")]
        public float Opacity { get; set; }
    }

    /// <summary>
    /// Папка
    /// </summary>
    [Serializable]
    public class Folder
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [XmlAttribute(attributeName: "id")]
        public int Id { get; set; }

        /// <summary>
        /// Дочерняя папка
        /// </summary>
        [XmlAttribute(attributeName: "parent")]
        public int Parent { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [XmlAttribute(attributeName: "name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// Маршрут
    /// </summary>
    [Serializable]
    public class Course
    {
        // "24--b-p"
        /// <summary>
        /// Название
        /// </summary>
        [XmlAttribute(attributeName: "name")]
        public string Name { get; set; }

        // "3"
        /// <summary>
        /// Идентификатор
        /// </summary>
        [XmlAttribute(attributeName: "id")]
        public int Id { get; set; }

        // "2"
        /// <summary>
        /// Дочерняя папка
        /// </summary>
        [XmlAttribute(attributeName: "parent")]
        public int Parent { get; set; }

        [XmlElement("waypoint")]
        public List<Waypoint> Waypoints { get; set; }
    }

    /// <summary>
    /// Точка маршрута
    /// </summary>
    [Serializable]
    public class Waypoint
    {
        // "142.49 69.33"
        [XmlAttribute(attributeName: "pos")]
        public string PositionXml
        {
            get
            {
                var nfi = new NumberFormatInfo
                {
                    NumberDecimalSeparator = "."
                };
                return string.Format("{0} {1}", Position.X.ToString(nfi), Position.Y.ToString(nfi));
            }
            set
            {
                Position = Point.Parse(value);
            }
        }

        [XmlIgnore]
        public Point Position { get; set; }

        // "1.11"
        /// <summary>
        /// Угол поворота
        /// </summary>
        [XmlAttribute(attributeName: "angle")]
        public float Angle { get; set; }

        // "0"
        /// <summary>
        /// Скорость
        /// </summary>
        [XmlAttribute(attributeName: "speed")]
        public int Speed { get; set; }

        // "1"
        [XmlAttribute(attributeName: "crossing")]
        [DefaultValue(0)]
        public int CrossingXml { get { return Crossing ? 1 : 0; } set { Crossing = value != 0; } }

        /// <summary>
        /// Точка разрыва при построении комплексного пути
        /// </summary>
        [XmlIgnore]
        public bool Crossing { get; set; }

        // "left"
        [XmlAttribute(attributeName: "turn")]
        [DefaultValue(TurnStep.none)]
        public TurnStep Turn { get; set; }

        // "1"
        [XmlAttribute(attributeName: "turnstart")]
        [DefaultValue(0)]
        public int TurnStartXml { get { return TurnStart ? 1 : 0; } set { TurnStart = value != 0; } }

        /// <summary>
        /// Начала шага маневра
        /// </summary>
        [XmlIgnore]
        public bool TurnStart { get; set; }

        // "1"
        [XmlAttribute(attributeName: "turnend")]
        [DefaultValue(0)]
        public int TurnEndXml { get { return TurnEnd ? 1 : 0; } set { TurnEnd = value != 0; } }

        /// <summary>
        /// Окончание шага маневра
        /// </summary>
        [XmlIgnore]
        public bool TurnEnd { get; set; }

        // "1"
        [XmlAttribute(attributeName: "wait")]
        [DefaultValue(0)]
        public int WaitXml { get { return Wait ? 1 : 0; } set { Wait = value != 0; } }

        /// <summary>
        /// Точка остановки
        /// </summary>
        [XmlIgnore]
        public bool Wait { get; set; }

        // "1"
        [XmlAttribute(attributeName: "rev")]
        [DefaultValue(0)]
        public int ReversXml { get { return Revers ? 1 : 0; } set { Revers = value != 0; } }

        /// <summary>
        /// Движение задним ходом
        /// </summary>
        [XmlIgnore]
        public bool Revers { get; set; }


        // "true"
        /// <summary>
        /// Точка сгенерированна модом
        /// </summary>
        [XmlAttribute(attributeName: "generated")]
        [DefaultValue(false)]
        public bool Generated { get; set; }

        // "S"
        /// <summary>
        /// Направление дыижения
        /// </summary>
        [XmlAttribute(attributeName: "dir")]
        [DefaultValue(Direction.none)]
        public Direction Direction { get; set; }

        // "2"
        /// <summary>
        /// ???
        /// </summary>
        [XmlAttribute(attributeName: "ridgemarker")]
        [DefaultValue(0)]
        public int RidgeMarker { get; set; }
    }

    /// <summary>
    /// Поворот
    /// </summary>
    public enum TurnStep
    {
        none,
        right,
        left
    }

    /// <summary>
    /// Направление
    /// </summary>
    public enum Direction
    {
        none,
        N,
        S,
        E,
        W
    }
}
