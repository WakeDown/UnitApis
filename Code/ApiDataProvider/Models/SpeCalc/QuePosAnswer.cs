using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Models.Stuff;
using DataProvider.Objects;

namespace DataProvider.Models.SpeCalc
{
    public class QuePosAnswer : DbModel
    {
        public int Id { get; set; }
        public QuePosition QuePosition { get; set; }
        public Employee Answerer { get; set; }
        public string Descr { get; set; }
        public Employee Creator { get; set; }


        public QuePosAnswer() { }

        public QuePosAnswer(DataRow row)
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueInt(row["id"]);
            QuePosition = new QuePosition() { Id = Db.DbHelper.GetValueInt(row["id_que_position"]), Question = new Question() { Id = Db.DbHelper.GetValueInt(row["id_question"]) } };//new QuePosition(Db.DbHelper.GetValueInt(row["id_que_position"]));//
            Answerer = new Employee() { AdSid = row["answerer_sid"].ToString() };
            Descr = row["descr"].ToString();
        }

        public QuePosAnswer(int id)
        {
            SqlParameter pId = new SqlParameter() {ParameterName = "id", SqlValue = id,SqlDbType = SqlDbType.Int };
            var dt = Db.SpeCalc.ExecuteQueryStoredProcedure("get_que_pos_answer", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
        }

        public void Save()
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            SqlParameter pQuestionPositionId = new SqlParameter() { ParameterName = "id_que_position", SqlValue = QuePosition.Id, SqlDbType = SqlDbType.Int };
            SqlParameter pAnswererAdSid = new SqlParameter() { ParameterName = "answerer_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pDescr = new SqlParameter() { ParameterName = "descr", SqlValue = Descr, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };

            var dt = Db.SpeCalc.ExecuteQueryStoredProcedure("save_que_pos_answer", pId, pQuestionPositionId, pAnswererAdSid, pDescr, pCreatorAdSid);
            if (dt.Rows.Count > 0)
            {
                int id;
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;
            }
        }

        public static IEnumerable<QuePosAnswer> GetList(int idQuePosition)
        {
            SqlParameter pIdQuePosition = new SqlParameter() { ParameterName = "id_que_position", SqlValue = idQuePosition, SqlDbType = SqlDbType.Int };
            var dt = Db.SpeCalc.ExecuteQueryStoredProcedure("get_que_pos_answer", pIdQuePosition);

            var lst = new List<QuePosAnswer>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new QuePosAnswer(row);
                lst.Add(model);
            }

            return lst;
        }
        
        public static void Close(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.SpeCalc.ExecuteQueryStoredProcedure("close_que_pos_answer", pId);
        } 
    }
}