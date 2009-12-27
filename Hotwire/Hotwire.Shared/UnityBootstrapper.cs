using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;

namespace Hotwire.Shared
{
    public class UnityBootstrapper
    {
        public IUnityContainer Container { get; private set; }

        /// <summary>
        /// Initializes a new instance of the UnityBootstrapper class.
        /// </summary>
        public UnityBootstrapper()
        {
            this.Container = new UnityContainer();

            Container.RegisterType<IServiceLocator, UnityServiceLocatorAdapter>(new ContainerControlledLifetimeManager());

            ServiceLocator.SetLocatorProvider(() => this.Container.Resolve<IServiceLocator>());
        }

    }
}
