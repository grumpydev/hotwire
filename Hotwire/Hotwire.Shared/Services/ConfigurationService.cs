using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hotwire.Shared.Interfaces;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;

namespace Hotwire.Shared.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private static string configurationFileName;

        static ConfigurationService()
        {
            string configurationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Hotwire");
            configurationFileName = Path.Combine(configurationDirectory, "Hotwire.config");

            if (!Directory.Exists(configurationDirectory))
            {
                Directory.CreateDirectory(configurationDirectory);
            }
        }
        #region IConfigurationService Members

        public IEnumerable<Machine> LoadConfiguration()
        {
            if (!File.Exists(configurationFileName))
                return new List<Machine>();

            List<Machine> machines = new List<Machine>();

            using (FileStream fs = new FileStream(configurationFileName, FileMode.Open))
            {
        		XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                DataContractSerializer serializer = new DataContractSerializer(typeof(List<Machine>));
                machines = (List<Machine>) serializer.ReadObject(reader, true);
                reader.Close();
            }

            return machines;
        }

        public bool SaveConfiguration(IEnumerable<Machine> machines)
        {
            using (FileStream writer = new FileStream(configurationFileName, FileMode.Create))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(IEnumerable<Machine>));
                serializer.WriteObject(writer, machines);
                writer.Close();
            }

            return true;
        }

        public bool ConfigurationAppearsValid(IEnumerable<Machine> machines)
        {
            foreach (var machine in machines)
            {
                if (!Uri.IsWellFormedUriString(machine.Url, UriKind.Absolute))
                    return false;
            }

            return true;
        }

        #endregion
    }
}
