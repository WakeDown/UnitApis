using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Objects;

namespace DataProvider.Models.Stuff
{
    public class EmployeeRestHoliday:DbModel
    {
        public string EmployeeName { get; set; }
        public string EmployeeSid { get; set; }
        public int DurationSum { get; set; }
        public int Residue { get; set; }

        public EmployeeRestHoliday(DataRow row)
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            DurationSum = Db.DbHelper.GetValueIntOrDefault(row, "duration");
            EmployeeSid = Db.DbHelper.GetValueString(row, "employee_sid");
            EmployeeName = Db.DbHelper.GetValueString(row, "emlpoyee_name");
            Residue = Db.DbHelper.GetValueIntOrDefault(row, "residue");
        }
    }
}