using System.ComponentModel.Composition;
using System.Windows;
using FarmSimCourseManager.Contracts;
using FarmSimCourseManager.ViewModel;

namespace FarmSimCourseManager.View
{
    [Export(typeof(SettingsWindow))]
    [Export(typeof(ISettingsWindow))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class SettingsWindow : Window, ISettingsWindow 
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        [Import]
        public SettingsWindowViewModel ViewModel
        {
            get { return (SettingsWindowViewModel)DataContext; }
            set { DataContext = value; }
        }
    }
}
