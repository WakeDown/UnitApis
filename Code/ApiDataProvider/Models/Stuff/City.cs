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
    public class City : DbModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int EmpCount { get; set; }

        public City()
        {
        }

        public City(DataRow row)
        {
            FillSelf(row);
        }

        public City(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_city", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
        }

        public City(string sysName)
        {
            SqlParameter pSysName = new SqlParameter() { ParameterName = "sys_name", SqlValue = sysName, SqlDbType = SqlDbType.NVarChar };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_city", pSysName);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueInt(row["id"]);
            Name = row["name"].ToString();
            EmpCount = Db.DbHelper.GetValueInt(row["emp_count"]);
        }

        public static IEnumerable<City> GetList()
        {
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_city");
            var lst = new List<City>();
            foreach (DataRow row in dt.Rows)
            {
                var city = new City(row);
                lst.Add(city);
            }
            return lst;
        }

        public void Save()
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            SqlParameter pName = new SqlParameter() { ParameterName = "name", SqlValue = Name, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };

            var dt = Db.Stuff.ExecuteQueryStoredProcedure("save_city", pId, pName, pCreatorAdSid);
            if (dt.Rows.Count > 0)
            {
                int id;
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;
            }
        }

        public static void Close(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };

            int count = (int)Db.Stuff.ExecuteScalar("get_city_link_count", pId);

            if (count == 0)
            {
                SqlParameter pIdOrg = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
                Db.Stuff.ExecuteStoredProcedure("close_city", pIdOrg);
            }
            else
            {
                throw new Exception("Невозможно удалить город так как есть привязка к сотрудникам!");
            }
        }
    }
}