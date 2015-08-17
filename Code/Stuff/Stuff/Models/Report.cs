using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Stuff.Objects;

namespace Stuff.Models
{
    public class Report:DbModel
    {
        public static IEnumerable<ItBudgetReportItem> GetItBudgetList()
        {
            Uri uri = new Uri(String.Format("{0}/StuffReport/GetItBudgetList", OdataServiceUri));
            string jsonString = GetJson(uri);

            var model = JsonConvert.DeserializeObject<IEnumerable<ItBudgetReportItem>>(jsonString);

            return model;
        }
    }
}