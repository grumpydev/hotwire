using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Hotwire.Shared.Interfaces;
using Microsoft.Practices.Unity;
using Hotwire.Shared.Services;

namespace Hotwire.Shared
{
    public static class BootStrapper
    {
        private static object ConfigurationLock = new object();
        private static bool Configured = false;

        /// <summary>
        /// Unity container
        /// </summary>
        public static IUnityContainer Container {get; private set;}

        public static void ConfigureServices()
        {
            if (Configured)
                return;

            lock (ConfigurationLock)
            {
                if (!Configured)
                {
                    InitialiseContainer();
                    Configured = true;
                }
            }
        }

        private static void InitialiseContainer()
        {
            var bootstrapper = new UnityBootstrapper();
            Container = bootstrapper.Container;

            Container.RegisterType<IConfigurationService, ConfigurationService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IEncryptionService, EncryptionService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ISystemInformationService, SystemInformationService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IAlertService, AlertService>(new ContainerControlledLifetimeManager());
        }
    }
}
