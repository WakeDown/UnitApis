using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Objects;

namespace DataProvider.Models.Stuff
{
    public class EmployeeSm
    {
        public int Id { get; set; }
        public string AdSid { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        public EmployeeSm()
        {
        }
        
        public EmployeeSm(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_employee_sm", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
        }

        public EmployeeSm(string sid)
        {
            if (String.IsNullOrEmpty(sid)) return;
            SqlParameter pSid = new SqlParameter() { ParameterName = "ad_sid", SqlValue = sid, SqlDbType = SqlDbType.VarChar };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_employee_sm", pSid);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
            else
            {
                var adUser = AdHelper.GetUserBySid(sid);
                FillSelf(adUser); 
            }
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            AdSid = Db.DbHelper.GetValueString(row, "ad_sid");
            DisplayName = Db.DbHelper.GetValueString(row, "display_name");
            FullName = Db.DbHelper.GetValueString(row, "full_name");
            Email = Db.DbHelper.GetValueString(row, "email");
        }

        private void FillSelf(EmployeeSm user)
        {
            Id = user.Id;
            AdSid = user.AdSid;
            DisplayName = user.DisplayName;
            FullName = user.FullName;
            Email = user.Email;
        }
    }
}