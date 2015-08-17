using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HolidayWorkEmailListener.Objects;
using Newtonsoft.Json;
using HolidayWorkEmailListener.Objects;

namespace StuffDelivery.Models
{
    public class HolidayWork:DbModel
    {
        public static HolidayResult CheckTodayIsPreHoliday()
        {
            DateTime date = DateTime.Now;//new DateTime(2015, 7, 24);// new DateTime(2015, 4,30);// 
            Uri uri = new Uri(String.Format("{0}/HolidayWork/CheckIsPreHoliday?date={1:yyyy-MM-dd}", OdataServiceUri, date.Date));
            string jsonString = GetJson(uri);

            HolidayResult result = JsonConvert.DeserializeObject<HolidayResult>(jsonString);
            return result;
        }
    }
}
