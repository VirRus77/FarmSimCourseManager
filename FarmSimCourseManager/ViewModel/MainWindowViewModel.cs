using System.ComponentModel.Composition;
using FarmSimCourseManager.Contracts;
using FarmSimCourseManager.Tools;

namespace FarmSimCourseManager.ViewModel
{
    [Export(typeof(MainWindowViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MainWindowViewModel : ObservableObject
    {
        //[ImportingConstructor]
        //public MainWindowViewModel([ImportMany] IEnumerable<IMenu> menuItems)
        //{
        //    var menuItemList = menuItems.ToList();

        //    OpenMenu = menuItemList.OfType<IOpenMenu>().Single();
        //}

        [Import]
        public IOpenMenu OpenMenu { get; private set; }

        [Import]
        public ISaveMenu SaveMenu { get; private set; }

        [Import]
        public ISaveAsMenu SaveAsMenu { get; private set; }

        [Import]
        public ILoadMapMenu LoadMapMenu { get; private set; }

        [Import]
        public ISettingsMenu SettingsMenu { get; private set; }
    }
}
