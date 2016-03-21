using System.ComponentModel.Composition;
using System.Windows.Controls;
using FarmSimCourseManager.Contracts;
using FarmSimCourseManager.ViewModel;

namespace FarmSimCourseManager.View
{
    [Export(typeof(ITreeView))]
    [Export(typeof(CourseTree))]
    public partial class CourseTree : UserControl, ITreeView
    {
        public CourseTree()
        {
            InitializeComponent();
        }

        [Import]
        public CourseTreeViewModel ViewModel
        {
            get { return (CourseTreeViewModel)DataContext; }
            set { DataContext = value; }
        }
    }
}
