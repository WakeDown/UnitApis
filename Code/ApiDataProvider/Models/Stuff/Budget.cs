using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Objects;
using DataProvider._TMPLTS;

namespace DataProvider.Models.Stuff
{
    public class Budget:DbModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Descr { get; set; }
        public int? EmpCount { get; set; }
        public int? IdParent { get; set; }
        public Budget Parent { get; set; }
        //public int PeopleCount { get; set; }
        //public int Level { get; set; }

        public Budget() { }

        public Budget(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_budget", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
        }

        public Budget(DataRow row)
            : this()
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            Name = Db.DbHelper.GetValueString(row, "name");
            Descr = Db.DbHelper.GetValueString(row, "descr");
            EmpCount = Db.DbHelper.GetValueIntOrNull(row, "emp_count");
            IdParent = Db.DbHelper.GetValueIntOrNull(row, "id_parent");
            if (IdParent.HasValue)
            { Parent = new Budget(IdParent.Value);}
            else { Parent = new Budget(); }
        }

        public void Save()
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            SqlParameter pName = new SqlParameter() { ParameterName = "name", SqlValue = Name, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pDescr = new SqlParameter() { ParameterName = "descr", SqlValue = Descr, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pIdParent = new SqlParameter() { ParameterName = "id_parent", SqlValue = IdParent, SqlDbType = SqlDbType.Int };

            var dt = Db.Stuff.ExecuteQueryStoredProcedure("save_budget", pId, pName, pDescr, pCreatorAdSid, pIdParent);
            int id = 0;
            if (dt.Rows.Count > 0)
            {
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;
            }
        }

        public static IEnumerable<Budget> GetList()
        {
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_budget_list");

            var lst = new List<Budget>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new Budget(row);
                lst.Add(model);
            }

            return lst.OrderBy(x=>x.Parent.Name).ThenBy(x=>x.Name);
        }

        public static void Close(int id, string deleterSid)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            SqlParameter pDeleterSid = new SqlParameter() { ParameterName = "deleter_sid", SqlValue = deleterSid, SqlDbType = SqlDbType.VarChar };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("close_budget", pId, pDeleterSid);
        }
    }
}