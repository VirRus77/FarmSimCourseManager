using System.ComponentModel.Composition;
using System.Windows;
using FarmSimCourseManager.Contracts;
using FarmSimCourseManager.ViewModel;
using Microsoft.Practices.Prism.Regions;

namespace FarmSimCourseManager.View
{
    [Export(typeof(MainWindow))]
    public partial class MainWindow : Window
    {
        [ImportingConstructor]
        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();
            RegionManager = regionManager;
            RegionManager.RegisterViewWithRegion(WellKnownRegions.TreeViewRegion, typeof(ITreeView));
            RegionManager.RegisterViewWithRegion(WellKnownRegions.MapRegion, typeof(IMapView));
        }

        public IRegionManager RegionManager { get; set; }

        [Import]
        public MainWindowViewModel ViewModel
        {
            get { return (MainWindowViewModel)DataContext; }
            set { DataContext = value; }
        }
    }
}