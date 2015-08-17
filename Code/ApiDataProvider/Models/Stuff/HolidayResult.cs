using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataProvider.Models.Stuff
{
    public class HolidayResult
    {
        public bool SendDelivery { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public bool IsSundayOnly { get; set; }
    }
}