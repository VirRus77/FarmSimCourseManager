using System.ComponentModel.Composition;
using FarmSimCourseManager.Contracts;
using FarmSimCourseManager.Tools;

namespace FarmSimCourseManager.ViewModel
{
    [Export(typeof(SettingsWindowViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SettingsWindowViewModel : ObservableObject
    {
        [Import]
        public ISettings Settings { get; set; }
    }
}
