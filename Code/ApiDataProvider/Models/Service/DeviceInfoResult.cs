﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataProvider.Models.Service
{
    [Serializable]
    public class DeviceInfoResult
    {
        public int? DeviceId { get; set; }
        public string DeviceSerialNum { get; set; }
        public string ContractorStr { get; set; }
        public string ContractStr { get; set; }
        public string DeviceStr { get; set; }
        public string AddressStr { get; set; }
        public string DescrStr { get; set; }
    }
}