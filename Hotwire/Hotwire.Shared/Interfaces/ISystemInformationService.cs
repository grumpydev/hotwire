using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hotwire.Shared;

namespace Hotwire.Shared.Interfaces
{
    public interface ISystemInformationService
    {
        string DriveSerial { get; }
        string MacAddress { get; }
        string CpuId { get; }
    }
}
