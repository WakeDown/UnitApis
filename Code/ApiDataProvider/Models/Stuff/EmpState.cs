using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;

namespace DataProvider.Models.Stuff
{
    public class EmpState
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public EmpState()
        {
        }

        public EmpState(DataRow row)
        {
            FillSelf(row);
        }

        public EmpState(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            //SqlParameter pGetAll = new SqlParameter() { ParameterName = "get_all", SqlValue = 1, SqlDbType = SqlDbType.Bit };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_emp_state", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
        }
        public EmpState(string sysName)
        {
            SqlParameter pSysName = new SqlParameter() { ParameterName = "sys_name", SqlValue = sysName, SqlDbType = SqlDbType.NVarChar };
            //SqlParameter pGetAll = new SqlParameter() { ParameterName = "get_all", SqlValue = 1, SqlDbType = SqlDbType.Bit };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_emp_state", pSysName);
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
        }

        public static IEnumerable<EmpState> GetList()
        {
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_emp_state");
            var lst = new List<EmpState>();
            foreach (DataRow row in dt.Rows)
            {
                var empSt = new EmpState(row);
                lst.Add(empSt);
            }
            return lst;
        }

        public static EmpState GetStuffState()
        {
            return new EmpState("STUFF");
        }

        public static EmpState GetFiredState()
        {
            return new EmpState("FIRED");
        }

        public static EmpState GetDecreeState()
        {
            return new EmpState("DECREE");
        }

        public static EmpState GetNewbieState()
        {
            return new EmpState("NEWBIE");
        }
    }
}