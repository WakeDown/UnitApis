using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Objects;

namespace DataProvider.Models.SpeCalc
{
    public class QueState : DbModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SysName { get; set; }
        public int OrderNum { get; set; }

        public QueState() { }

        public QueState(int id)
        {
            GetQueState(id: id);
        }

        public QueState(DataRow row)
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueInt(row["id"]);
            Name = row["name"].ToString();
            SysName = row["sys_name"].ToString();
            OrderNum = Db.DbHelper.GetValueInt(row["order_num"]);
        }

        

        public new static IEnumerable<QueState> GetList()
        {
            var dt = Db.SpeCalc.ExecuteQueryStoredProcedure("get_question_state");

            var lst = new List<QueState>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new QueState(row);
                lst.Add(model);
            }

            return lst;
        }

        public QueState GetFirstState()
        {
            return GetQueState("NEW");
        }

        public QueState GetSentState()
        {
            return GetQueState("SENT");
        }

        public QueState GetProcessState()
        {
            return GetQueState("PROCESS");
        }

        public QueState GetAnsweredState()
        {
            return GetQueState("GETANSWER");
        }

        public QueState GetAprovedState()
        {
            return GetQueState("APROVE");
        }

        public QueState GetQueState(string sysName = null, int? id = null)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            SqlParameter pSysName = new SqlParameter() { ParameterName = "sys_name", SqlValue = sysName, SqlDbType = SqlDbType.NVarChar };
            var dt = Db.SpeCalc.ExecuteQueryStoredProcedure("get_question_state", pSysName, pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
            return this;
        }
    }
}