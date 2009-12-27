using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hotwire.Shared.Interfaces
{
    public interface IConfigurationService
    {
        IEnumerable<Machine> LoadConfiguration();
        bool SaveConfiguration(IEnumerable<Machine> machines);
        bool ConfigurationAppearsValid(IEnumerable<Machine> machines);
    }
}
