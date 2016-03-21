using System;
using System.ComponentModel;
using System.Linq.Expressions;
using JetBrains.Annotations;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Threading;
#endif

namespace FarmSimCourseManager.Tools
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
#if SILVERLIGHT
        private readonly Dispatcher _dispatcher;

        protected ObservableObject()
        {
            _dispatcher = Deployment.Current.Dispatcher;
        }


        public Dispatcher Dispatcher
        {
            get { return _dispatcher; }
        }
#endif

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void NotifyPropertyChanged<TProp>(Expression<Func<TProp>> expression, bool isDeep = false)
        {
            var e = PropertyChanged;
            if (e == null)
                return;

            var eventArgs = new PropertyChangedEventArgs(NameHelper.Name(expression, isDeep));
#if !SILVERLIGHT
            e(this, eventArgs);
#else
            if (_dispatcher.CheckAccess())
                e(this, eventArgs);
            else
                _dispatcher.BeginInvoke(() => e(this, eventArgs));
#endif
        }
    }
}