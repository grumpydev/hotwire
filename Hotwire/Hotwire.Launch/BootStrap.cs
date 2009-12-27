using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Practices.Unity;
using Hotwire.Launch.ViewModels;
using Hotwire.Launch.Views;

namespace Hotwire.Launch
{
    class BootStrap
    {
        static TaskTray _taskTray;

        [STAThread]
        static void Main()
        {
            Hotwire.Shared.BootStrapper.ConfigureServices();
            IUnityContainer container = Hotwire.Shared.BootStrapper.Container;

            container.RegisterType<TaskTray>();
            container.RegisterType<ConfigurationViewModel>();
            container.RegisterType<ConfigurationView>();
            container.RegisterType<AboutBoxView>();

            Application app = new Application();

            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            _taskTray = container.Resolve<TaskTray>();
            app.Run();
        }
    }
}
