using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Objects;
using DataProvider._TMPLTS;

namespace DataProvider.Models
{
    public class MobileUser:DbModel
    {
        public string Sid { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public MobileUser() { }

        //public MobileUser(int id)
        //{
        //    SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
        //    var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_model", pId);
        //    if (dt.Rows.Count > 0)
        //    {
        //        var row = dt.Rows[0];
        //        FillSelf(row);
        //    }
        //}

        public MobileUser(DataRow row)
            : this()
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Sid = Db.DbHelper.GetValueString(row, "sid");
            Login = Db.DbHelper.GetValueString(row, "login");
            Password = Db.DbHelper.GetValueString(row, "password");
        }

        //public void Save()
        //{
        //    SqlParameter pLogin = new SqlParameter() { ParameterName = "login", SqlValue = Login, SqlDbType = SqlDbType.NVarChar };
        //    SqlParameter pPassword = new SqlParameter() { ParameterName = "password", SqlValue = Password, SqlDbType = SqlDbType.NVarChar };
        //    SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };

        //    var dt = Db.Service.ExecuteQueryStoredProcedure("save_model", pLogin, pPassword, pCreatorAdSid);
        //    int id = 0;
        //    if (dt.Rows.Count > 0)
        //    {
        //        int.TryParse(dt.Rows[0]["id"].ToString(), out id);
        //        Id = id;
        //    }
        //}

        public static IEnumerable<MobileUser> GetList()
        {
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_mobile_user_list");

            var lst = new List<MobileUser>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new MobileUser(row);
                lst.Add(model);
            }

            return lst;
        }

        //public static void Close(int id, string deleterSid)
        //{
        //    SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
        //    SqlParameter pDeleterSid = new SqlParameter() { ParameterName = "deleter_sid", SqlValue = deleterSid, SqlDbType = SqlDbType.VarChar };
        //    var dt = Db.Service.ExecuteQueryStoredProcedure("close_model", pId, pDeleterSid);
        //}
    }
}