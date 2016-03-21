using System;
using System.Windows;

namespace FarmSimCourseManager.Tools
{
    /// <summary>
    /// Базовый интерфейс комопонента, выводимого на план.
    /// </summary>
    public interface IPlanItem : IDisposable
    {
        /// <summary>
        /// Местоположение.
        /// </summary>
        Point Location { get; set; }
        /// <summary>
        /// Масштаб.
        /// </summary>
        double Scale { get; set; }
    }
}
