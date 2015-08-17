using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Models.SpeCalc;
using DataProvider.Models.Stuff;

namespace DataProvider.Models.SpeCalc
{
    public class HistoryQueState
    {
        public QueState State { get; set; }
        public Employee Creator { get; set; }
        public DateTime DateCreate { get; set; }

        public static IEnumerable<HistoryQueState> GetStateHistory(int idQuestion)
        {
            var list = new List<HistoryQueState>();
            SqlParameter pIdQuestion = new SqlParameter() { ParameterName = "id_question", SqlValue = idQuestion, SqlDbType = SqlDbType.Int };

            var dt = Db.SpeCalc.ExecuteQueryStoredProcedure("get_que_state_history", pIdQuestion);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new HistoryQueState() { State = new QueState(Db.DbHelper.GetValueInt(row["id_que_state"])), Creator = new Employee(row["creator_sid"].ToString()), DateCreate = Db.DbHelper.GetValueDateTime(row["dattim1"])});
                }
            }

            return list;
        }
    }
}