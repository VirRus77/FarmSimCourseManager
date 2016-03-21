using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using FarmSimCourseManager.Contracts;
using FarmSimCourseManager.Model;
using FarmSimCourseManager.Tools;

namespace FarmSimCourseManager.ViewModel
{
    [Export(typeof(CourseTreeViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class CourseTreeViewModel : ObservableObject
    {
        private readonly IContent _content;

        [ImportingConstructor]
        public CourseTreeViewModel(IContent content)
        {
            _content = content;

            _content.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName != NameHelper<IContent>.Name(v => v.CourseFileModel))
                    return;
                NotifyPropertyChanged(() => RootFolder);
            };
        }

        public FolderModel RootFolder
        {
            get
            {
                return _content.CourseFileModel == null
                    ? null
                    : _content.CourseFileModel.Folder;
            }
        }
    }
}
