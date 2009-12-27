using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hotwire.Shared.Interfaces;

namespace Hotwire.Shared.Services
{
    public class SystemInformationService : ISystemInformationService
    {
        public string DriveSerial { get; private set; }

        public string MacAddress { get; private set; }

        public string CpuId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the SystemInformation class.
        /// </summary>
        public SystemInformationService()
        {
            DriveSerial = SystemInformationHelper.GetVolumeSerial("C");
            MacAddress = SystemInformationHelper.GetMACAddress();
            CpuId = SystemInformationHelper.GetCPUId();
        }
    }
}
