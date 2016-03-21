using System;
using System.Windows;
using System.Windows.Input;

namespace FarmSimCourseManager.Tools.Controls
{
    /// <summary>
    /// Интерфейс инструмента.
    /// </summary>
    public interface IInstrument : IDisposable
    {
        void OnUp(Point upPoint, MouseButtonEventArgs mouseButtonEventArgs);
        void OnMove(Point movePoint, MouseEventArgs mouseEventArgs);
    }
}
