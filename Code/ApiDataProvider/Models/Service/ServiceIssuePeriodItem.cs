using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataProvider.Models.Service
{
    public class ServiceIssuePeriodItem
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ServiceIssuePeriodItem()
        {
        }
        public ServiceIssuePeriodItem(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}