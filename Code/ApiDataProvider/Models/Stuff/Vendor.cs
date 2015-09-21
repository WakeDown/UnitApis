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
    public class Vendor : DbModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Vendor()
        {
        }

        public Vendor(DataRow row)
        {
            FillSelf(row);
        }

        public Vendor(int id)
        {
            SqlParameter pId = new SqlParameter()
            {
                ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int
            };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_Vendor", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueIntOrDefault(row,"id");
            Name = Db.DbHelper.GetValueStringOrEmpty(row, "name");
        }

        public static IEnumerable<Vendor> GetList()
        {
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_Vendor");
            var lst = new List<Vendor>();
            foreach (DataRow row in dt.Rows)
            {
                var Vendor = new Vendor(row);
                lst.Add(Vendor);
            }
            return lst;
        }

        public void Save()
        {

            SqlParameter pId = new SqlParameter()
            {
                ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int
            };
            SqlParameter pName = new SqlParameter()
            {
                ParameterName = "name", SqlValue = Name, SqlDbType = SqlDbType.NVarChar
            };
            SqlParameter pCreatorAdSid = new SqlParameter()
            {
                ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar
            };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("save_Vendor", pId, pName, pCreatorAdSid);
            if (dt.Rows.Count > 0)
            {
                int id;
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;
            }
        }

        public static void Close(int id, string deleter)
        {
            SqlParameter pDeleter = new SqlParameter()
            {
                ParameterName = "deleter_sid", SqlValue = deleter, SqlDbType = SqlDbType.VarChar
            };
            SqlParameter pId = new SqlParameter()
            {
                ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int
            };
            Db.Stuff.ExecuteStoredProcedure("close_vendor", pId, pDeleter);
           
            
        }
    }
}