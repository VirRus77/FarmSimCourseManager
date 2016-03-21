using System.Windows.Input;

namespace FarmSimCourseManager.Contracts
{
    public interface IMenu
    {
        /// <summary>
        /// Название команды
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Описание команды
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Выполнение при нажатии
        /// </summary>
        ICommand Command { get; }
    }

    public interface IOpenMenu : IMenu
    { }

    public interface ISaveMenu : IMenu
    { }

    public interface ISaveAsMenu : IMenu
    { }

    public interface ILoadMapMenu : IMenu
    { }

    public interface ISettingsMenu : IMenu
    { }

    public interface ISettingsWindow 
    { }
}
