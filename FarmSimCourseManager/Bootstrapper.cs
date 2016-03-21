using System.ComponentModel.Composition.Hosting;
using System.Windows;
using FarmSimCourseManager.Prism;
using FarmSimCourseManager.View;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.MefExtensions;
using Microsoft.Practices.ServiceLocation;

namespace FarmSimCourseManager
{
    public class Bootstrapper : MefBootstrapper
    {

        #region Overrides of Bootstrapper

        protected override ILoggerFacade CreateLogger()
        {
            return new Log4NetLogger();
        }

        protected override DependencyObject CreateShell()
        {
            return ServiceLocator.Current.GetInstance<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Logger.Log("InitializeShell", Category.Info, Priority.None);

            base.InitializeShell();

            Application.Current.MainWindow = (Window)Shell;
            Application.Current.MainWindow.Show();
        }

        #endregion

        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();
            // Add this assembly to export ModuleTracker
            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));
        }
    }
}