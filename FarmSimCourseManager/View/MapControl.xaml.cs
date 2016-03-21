using System.ComponentModel.Composition;
using System.Windows.Controls;
using FarmSimCourseManager.Contracts;
using FarmSimCourseManager.ViewModel;

namespace FarmSimCourseManager.View
{
    [Export(typeof(IMapView))]
    [Export(typeof(MapControl))]
    public partial class MapControl : UserControl, IMapView
    {
        public MapControl()
        {
            InitializeComponent();
        }

        [Import]
        public MapControlViewModel ViewModel
        {
            get { return (MapControlViewModel)DataContext; }
            set { DataContext = value; }
        }
    }
}
