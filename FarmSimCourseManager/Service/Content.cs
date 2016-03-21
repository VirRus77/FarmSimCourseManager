using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using FarmSimCourseManager.Contracts;
using FarmSimCourseManager.Model;
using FarmSimCourseManager.Tools;

namespace FarmSimCourseManager.Service
{
    [Export(typeof(IContent))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class Content : ObservableObject, IContent
    {
        private string _openFilePath;
        private CourseFileModel _courseFile;
        private IList<INode> _treeNodes;
        private string _mapFilePath;

        public string OpenFilePath
        {
            get { return _openFilePath; }
            set
            {
                _openFilePath = value;
                NotifyPropertyChanged(() => OpenFilePath);
            }
        }

        public CourseFileModel CourseFileModel
        {
            get { return _courseFile; }
            set
            {
                if (_courseFile == value)
                    return;
                _courseFile = value;
                NotifyPropertyChanged(() => CourseFileModel);
            }
        }

        [Import]
        public ISettings Settings { get; set; }

        //public IList<INode> TreeNodes
        //{
        //    get { return _treeNodes; }
        //    private set
        //    {
        //        if (_treeNodes == value)
        //            return;
        //        _treeNodes = value;
        //        NotifyPropertyChanged(() => TreeNodes);
        //    }
        //}

        public string MapFilePath
        {
            get { return _mapFilePath; }
            set
            {
                if (_mapFilePath == value)
                    return;
                _mapFilePath = value;
                NotifyPropertyChanged(() => MapFilePath);
            }
        }

        //private void FillNodes()
        //{
        //    var treeList = new List<INode>();
        //    var courseList = CourseFile.Courses.OrderBy(v => v.Id).ToList();
        //    foreach (var folder in CourseFile.Folders.OrderBy(v => v.Id))
        //    {
        //        var node = new FolderNode(folder, courseList.Where(v => v.Parent == folder.Id).ToList());
        //        courseList = courseList.Where(v => v.Parent != folder.Id).ToList();
        //        treeList.Add(node);
        //    }
        //    foreach (var course in courseList)
        //    {
        //        var node = new CourseNode(course);
        //        treeList.Add(node);
        //    }
        //    TreeNodes = null;
        //    TreeNodes = treeList;
        //}
    }
}
