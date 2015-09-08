using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataProvider.Controllers.Stuff;

namespace DataProvider.Models.Stuff
{
    public class ItBudgetReportItem
    {
        public string BudgetName { get; set; }
        public string DepartmentName { get; set; }
        public int PeopleCount { get; set; }
        public double CostSum { get; set; }
        public IEnumerable<ItBudgetReportItemPeople> Peoples { get; set; }
        public int Level { get; set; }

        public ItBudgetReportItem()
        {
            
        }
    }
}