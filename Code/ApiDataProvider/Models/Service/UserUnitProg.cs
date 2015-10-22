using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Objects;

namespace DataProvider.Models.Service
{
    public class UserUnitProg:DbModel
    {
        public static int GetUserId(string sid)
        {
            if (String.IsNullOrEmpty(sid)) return -1;
            SqlParameter pAction = new SqlParameter() { ParameterName = "action", Value = "getUserBySid", SqlDbType = SqlDbType.NVarChar };
            SqlParameter pAdSid = new SqlParameter() { ParameterName = "user_sid", Value = sid, DbType = DbType.AnsiString };

            DataTable dt = Db.UnitProg.ExecuteQueryStoredProcedure("ui_users", pAction, pAdSid);

            int id = -1;
            if (dt.Rows.Count > 0)
            {
                id = Db.DbHelper.GetValueIntOrDefault(dt.Rows[0], "id_user");
            }

            return id;
        }
    }
}