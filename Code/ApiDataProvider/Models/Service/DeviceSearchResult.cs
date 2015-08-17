using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataProvider.Models.Service
{
    public class DeviceSearchResult
    {
        public IEnumerable<Device> Devices { get; set; }
        public IEnumerable<string> Vendors { get; set; }

        public DeviceSearchResult()
        {
        }

        public DeviceSearchResult(IEnumerable<Device> devices, IEnumerable<string> vendors)
        {
            Devices = devices;
            Vendors = vendors;
        }
    }
}