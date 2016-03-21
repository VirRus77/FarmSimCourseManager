using System.Drawing;

namespace FarmSimCourseManager.Contracts
{
    public interface ISettings
    {
        /// <summary>
        /// Создавать бэкапы
        /// </summary>
        bool IsCreateBackup { get; set; }

        /// <summary>
        /// Число бэкапов
        /// </summary>
        int CountBackups { get; set; }

        /// <summary>
        /// Настройки карты и отображения
        /// </summary>
        IMapSettings MapSettings { get; set; }
    }

    public interface IMapSettings
    {
        /// <summary>
        /// Картинка точки завершения маршрута
        /// </summary>
        ImageBinary ImageStop { get; set; }

        /// <summary>
        /// Картинка точки начала маршрута
        /// </summary>
        ImageBinary ImageStart { get; set; }

        /// <summary>
        /// Цвет линии маршрута
        /// </summary>
        Color CourseLineColor { get; set; }

        /// <summary>
        /// Толщина линии маршрута
        /// </summary>
        int CourseLineThickness { get; set; }

        /// <summary>
        /// Цвет линии точки
        /// </summary>
        Color PointLineColor { get; set; }

        /// <summary>
        /// Толщина линии точки
        /// </summary>
        int PointLineThickness { get; set; }

        /// <summary>
        /// Радиус точки
        /// </summary>
        int PointCircleRadius { get; set; }
    }
}