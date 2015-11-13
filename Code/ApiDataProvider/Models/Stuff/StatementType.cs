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
    public class StatementType:DbModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderNum { get; set; }

        public StatementType() { }

        public StatementType(string sysName)
        {
            SqlParameter pSysName = new SqlParameter() { ParameterName = "sys_name", SqlValue = sysName, SqlDbType = SqlDbType.NVarChar };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("statement_type_get", pSysName);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
        }

        public StatementType(DataRow row)
            : this()
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            Name = Db.DbHelper.GetValueString(row, "name");
            OrderNum = Db.DbHelper.GetValueIntOrDefault(row, "order_num");
        }

        public static IEnumerable<StatementType> GetList(string groupSysName = null)
        {
            SqlParameter pGroupSysName = new SqlParameter() { ParameterName = "group_sys_name", SqlValue = groupSysName, SqlDbType = SqlDbType.VarChar };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("statement_type_get_list", pGroupSysName);

            var lst = new List<StatementType>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new StatementType(row);
                lst.Add(model);
            }

            return lst;
        }
    }
}