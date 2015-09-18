using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StuffDelivery.Models
{
   public class HolidayResult
    {
        public bool SendDelivery { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public bool IsSundayOnly { get; set; }
    }
}
