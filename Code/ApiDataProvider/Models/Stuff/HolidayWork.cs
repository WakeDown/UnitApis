using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;

namespace DataProvider.Models.Stuff
{
    public class HolidayWork
    {
        public static string[] GetConfirms(DateTime date)
        {
            SqlParameter pDate = new SqlParameter() { ParameterName = "date", SqlValue = date, SqlDbType = SqlDbType.Date };

            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_holiday_work_confirms", pDate);

            var list = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(Db.DbHelper.GetValueString(row, "full_name"));
            }

            return list.ToArray();
        }

        public static string SaveConfirm(string fullName)
        {
            SqlParameter pFullName = new SqlParameter() { ParameterName = "full_name", SqlValue = fullName, SqlDbType = SqlDbType.NVarChar };

            var dt = Db.Stuff.ExecuteQueryStoredProcedure("save_holiday_work_confirm", pFullName);
            int id = 0;
            if (dt.Rows.Count > 0)
            {
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
            }

            return Employee.GetEmail(fullName);
        }
    }
}