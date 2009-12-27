using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace Hotwire.Shared
{
    public static class SystemInformationHelper
    {
        /// <summary>
        /// return Volume Serial Number from hard drive
        /// </summary>
        /// <param name="strDriveLetter">[optional] Drive letter</param>
        /// <returns>[string] VolumeSerialNumber</returns>
        public static string GetVolumeSerial(string strDriveLetter)
        {
            if (strDriveLetter == "" || strDriveLetter == null) strDriveLetter = "C";
            using (ManagementObject disk = new ManagementObject(String.Format("win32_logicaldisk.deviceid=\"{0}:\"", strDriveLetter)))
            {
                disk.Get();
                return disk["VolumeSerialNumber"].ToString();
            }
        }

        /// <summary>
        /// Returns MAC Address from first Network Card in Computer
        /// </summary>
        /// <returns>[string] MAC Address</returns>
        public static string GetMACAddress()
        {
            using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
            {
                ManagementObjectCollection moc = mc.GetInstances();
                string MACAddress = String.Empty;
                foreach (ManagementObject mo in moc)
                {
                    if (MACAddress == String.Empty)
                        // only return MAC Address from first card
                        if ((bool)mo["IPEnabled"] == true)
                            MACAddress = mo["MacAddress"].ToString();
                    mo.Dispose();
                }
                MACAddress = MACAddress.Replace(":", "");
                return MACAddress;
            }
        }

        /// <summary>
        /// Return processorId from first CPU in machine
        /// </summary>
        /// <returns>[string] ProcessorId</returns>
        public static string GetCPUId()
        {
            string cpuInfo = String.Empty;
            string temp = String.Empty;
            using (ManagementClass mc = new ManagementClass("Win32_Processor"))
            {
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                    if (cpuInfo == String.Empty)
                        cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
            }
            return cpuInfo;
        }
    }
}
