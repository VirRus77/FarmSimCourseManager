using System.Collections.Generic;
using System.ComponentModel;
using FarmSimCourseManager.CourseFileData;
using FarmSimCourseManager.Model;

namespace FarmSimCourseManager.Contracts
{
    public interface IContent : INotifyPropertyChanged
    {
        /// <summary>
        /// Путь к открытому файлу
        /// </summary>
        string OpenFilePath { get; set; }
        /// <summary>
        /// Контент открытого файла
        /// </summary>
        CourseFileModel CourseFileModel { get; set; }
        /// <summary>
        /// Путь до карты
        /// </summary>
        string MapFilePath { get; set; }
        /// <summary>
        /// Настройки
        /// </summary>
        ISettings Settings { get; set; }
    }
}