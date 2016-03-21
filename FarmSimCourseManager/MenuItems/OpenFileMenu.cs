using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using System.Windows.Input;
using FarmSimCourseManager.Contracts;
using FarmSimCourseManager.CourseFileData;
using FarmSimCourseManager.Model;
using FarmSimCourseManager.Tools;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;

namespace FarmSimCourseManager.MenuItems
{
    [Export(typeof(IMenu))]
    [Export(typeof(IOpenMenu))]
    public class OpenFileMenu : IOpenMenu
    {
        private IContent _content;
        private readonly OpenFileDialog _openFileDialog;

        [ImportingConstructor]
        public OpenFileMenu(IContent content)
        {
            _content = content;

            Name = "Открыть файл маршрутов";
            Command = new RelayCommand(OnExecute);

            _openFileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "Course file|courseplay.xml|Xml files|*.xml",
                Multiselect = false,
                Title = "Открыть файл путей",
                InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games\\FarmingSimulator2015")
            };
        }



        #region Implementation of IMenu

        public string Name { get; private set; }

        public string Description { get; private set; }

        public ICommand Command { get; private set; }

        private void OnExecute()
        {
            _openFileDialog.FileName = null;
            var rezult = _openFileDialog.ShowDialog();

            if (!rezult.HasValue || !rezult.Value)
                return;

            var fileName = _openFileDialog.FileName;
            _content.OpenFilePath = fileName;
            _content.CourseFileModel = CourseFileModel.Load(CourseFileManger.OpenFile(fileName));
        }

        #endregion
    }

    [Export(typeof(IMenu))]
    [Export(typeof(ISaveMenu))]
    public class SaveFileMenu : ISaveMenu
    {
        private readonly IContent _content;
        private readonly ISettings _settings;

        [ImportingConstructor]
        public SaveFileMenu(IContent content, ISettings settings)
        {
            _content = content;
            _settings = settings;

            _content.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName != NameHelper<IContent>.Name(v => v.OpenFilePath))
                    return;
                RaiseCanExecuteChanged();
            };

            Name = "Сохранить файл маршрутов";
            Command = new RelayCommand(OnExecute, () => !string.IsNullOrEmpty(_content.OpenFilePath));
            RaiseCanExecuteChanged();
        }

        #region Implementation of IMenu

        public string Name { get; private set; }
        public string Description { get; private set; }
        public ICommand Command { get; private set; }

        #endregion

        private void OnExecute()
        {
            if (_content.CourseFileModel == null)
                throw new Exception("Не открыто не одного файла.");
            if (string.IsNullOrEmpty(_content.OpenFilePath))
                throw new Exception("Нет пути сохранения открыто файла.");
            CourseFileManger.SaveFile(_content.OpenFilePath, _content.CourseFileModel.Save(), _settings.IsCreateBackup, _settings.CountBackups);
        }

        private void RaiseCanExecuteChanged()
        {
            ((RelayCommand)Command).RaiseCanExecuteChanged();
        }
    }

    [Export(typeof(IMenu))]
    [Export(typeof(ISaveAsMenu))]
    public class SaveAsFileMenu : ISaveAsMenu
    {
        private readonly IContent _content;
        private readonly SaveFileDialog _saveFileDialog;


        [ImportingConstructor]
        public SaveAsFileMenu(IContent content)
        {
            _content = content;

            _content.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName != NameHelper<IContent>.Name(v => v.OpenFilePath))
                    return;
                RaiseCanExecuteChanged();
            };

            Name = "Сохранить файл маршрутов как...";
            Command = new RelayCommand(OnExecute, () => !string.IsNullOrEmpty(_content.OpenFilePath));

            _saveFileDialog = new SaveFileDialog
            {
                OverwritePrompt = true,
                CheckPathExists = true,
                Filter = "Course file|courseplay.xml|Xml files|*.xml",
                Title = "Сохранить файл маршрутов",
                InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games\\FarmingSimulator2015"),
            };

            RaiseCanExecuteChanged();
        }

        [Import]
        public IContent Content { get; set; }

        #region Implementation of IMenu

        public string Name { get; private set; }
        public string Description { get; private set; }
        public ICommand Command { get; private set; }

        #endregion

        private void OnExecute()
        {
            if (_content.CourseFileModel == null)
                throw new Exception("Не открыто не одного файла.");
            if (!string.IsNullOrEmpty(_content.OpenFilePath))
            {
                _saveFileDialog.InitialDirectory = Path.GetDirectoryName(_content.OpenFilePath);
                _saveFileDialog.FileName = Path.GetFileName(_content.OpenFilePath);
            }

            var rezult = _saveFileDialog.ShowDialog();
            if (!rezult.HasValue || !rezult.Value)
                return;

            CourseFileManger.SaveAsFile(_saveFileDialog.FileName, _content.CourseFileModel.Save());
        }

        private void RaiseCanExecuteChanged()
        {
            ((RelayCommand)Command).RaiseCanExecuteChanged();
        }
    }

    [Export(typeof(IMenu))]
    [Export(typeof(ILoadMapMenu))]
    public class LoadMapMenu : ILoadMapMenu
    {
        private readonly IContent _content;
        private readonly OpenFileDialog _openFileDialog;

        [ImportingConstructor]
        public LoadMapMenu(IContent content)
        {
            _content = content;

            Name = "Открыть файл карты";
            Command = new RelayCommand(OnExecute);

            _openFileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "FarmSim map image|pda_map_H.dds|Image files|*.dds;*.png;*.bmp;*.jpg;*.jpeg",
                Multiselect = false,
                Title = "Открыть файл карты",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer)
            };

            RaiseCanExecuteChanged();
        }

        [Import]
        public IContent Content { get; set; }

        #region Implementation of IMenu

        public string Name { get; private set; }
        public string Description { get; private set; }
        public ICommand Command { get; private set; }

        #endregion

        private void OnExecute()
        {
            _openFileDialog.FileName = null;
            var rezult = _openFileDialog.ShowDialog();

            if (!rezult.HasValue || !rezult.Value)
                return;

            var fileName = _openFileDialog.FileName;
            _content.MapFilePath = fileName;
        }

        private void RaiseCanExecuteChanged()
        {
            ((RelayCommand)Command).RaiseCanExecuteChanged();
        }
    }
    
    [Export(typeof(IMenu))]
    [Export(typeof(ISettingsMenu))]
    public class SettingsMenu : ISettingsMenu
    {
        [ImportingConstructor]
        public SettingsMenu()
        {
            Name = "Настройки";
            Command = new RelayCommand(OnExecute);
            RaiseCanExecuteChanged();
        }

        [Import]
        public IServiceLocator ServiceLocator { get; set; }

        #region Implementation of IMenu

        public string Name { get; private set; }
        public string Description { get; private set; }
        public ICommand Command { get; private set; }

        #endregion

        private void OnExecute()
        {
            var window = ServiceLocator.GetInstance<ISettingsWindow>() as Window;

            if (window != null) 
                window.ShowDialog();
        }

        private void RaiseCanExecuteChanged()
        {
            ((RelayCommand)Command).RaiseCanExecuteChanged();
        }
    }
}