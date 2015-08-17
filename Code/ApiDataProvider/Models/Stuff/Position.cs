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
    public class Position:DbModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int EmpCount { get; set; }
        //public Employee Creator { get; set; }
        public string NameRod { get; set; }
        public string NameDat { get; set; }

        public Position()
        {
        }

        public Position(DataRow row)
        {
            FillSelf(row);
        }

        public Position(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_position", pId);
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
            NameRod = row["name_rod"].ToString();
            NameDat = row["name_dat"].ToString();
            EmpCount = Db.DbHelper.GetValueInt(row["emp_count"]);
        }

        public static IEnumerable<Position> GetList()
        {
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_position");
            var lst = new List<Position>();
            foreach (DataRow row in dt.Rows)
            {
                var pos = new Position(row);
                lst.Add(pos);
            }
            return lst;
        }

        public void Save()
        {
            //if (Creator == null) Creator = new Employee();
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            SqlParameter pName = new SqlParameter() { ParameterName = "name", SqlValue = Name, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pNameRod = new SqlParameter() { ParameterName = "name_rod", SqlValue = NameRod, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pNameDat = new SqlParameter() { ParameterName = "name_dat", SqlValue = NameDat, SqlDbType = SqlDbType.NVarChar };

            var dt = Db.Stuff.ExecuteQueryStoredProcedure("save_position", pId, pName, pCreatorAdSid, pNameRod, pNameDat);
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
            int count = (int)Db.Stuff.ExecuteScalar("get_position_link_count", pId);

            if (count == 0)
            {
                SqlParameter pIdPos = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
                Db.Stuff.ExecuteStoredProcedure("close_position", pIdPos);
            }
            else
            {
                throw new Exception("Невозможно удалить должность так как есть привязка к сотрудникам!");
            }
        }
    }

    
}