using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Hotwire.App.Views;
using System.Windows;
using Hotwire.Shared.Interfaces;
using Hotwire.App.ViewModels;

namespace Hotwire.App
{
    class Bootstrap
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Count() != 1)
                return;

            Hotwire.Shared.BootStrapper.ConfigureServices();
            IUnityContainer container = Hotwire.Shared.BootStrapper.Container;

            container.RegisterType<MainView>();
            container.RegisterType<MainViewModel>();

            var configurationService = container.Resolve<IConfigurationService>();
            var machines = configurationService.LoadConfiguration();

            // Verify configuration
            if (!configurationService.ConfigurationAppearsValid(machines))
            {
                container.Resolve<IAlertService>().ShowAlert(@"Your configuration appears invalid, possibly due to changed hardware.", @"Hotwire - Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Get machine
            var machine = container.Resolve<IConfigurationService>().LoadConfiguration().Where(m => m.Identifier == args[0]).FirstOrDefault();
            if (machine == null)
            {
                container.Resolve<IAlertService>().ShowAlert("Unable to load machine", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var currentMachineOverride = new ParameterOverrides();
            currentMachineOverride.Add("currentMachine", machine);
            var viewModel = container.Resolve<MainViewModel>(currentMachineOverride);
            var viewModelOverride = new ParameterOverrides();
            viewModelOverride.Add("viewModel", viewModel);
            
            var mainWindow = container.Resolve<MainView>(viewModelOverride);
            Application app = new Application();
            app.ShutdownMode = ShutdownMode.OnMainWindowClose;
            app.MainWindow = mainWindow;
            mainWindow.Show();
            app.Run();
        }

    }
}
