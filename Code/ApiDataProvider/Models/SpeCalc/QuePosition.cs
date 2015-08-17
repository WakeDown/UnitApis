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
    public class QuePosition : DbModel
    {
        public int Id { get; set; }
        public Question Question { get; set; }
        public Employee User { get; set; }
        public string Descr { get; set; }
        public Employee Creator { get; set; }

        public IEnumerable<QuePosAnswer> QuePosAnswers { get; set; }

        public QuePosition() { }

        private QuePosition(DataRow row)
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueInt(row["id"]);
            Question = new Question() { Id = Db.DbHelper.GetValueInt(row["id_question"]) };
            string adSid = row["user_sid"].ToString();
            User = new Employee(adSid);
            Descr = row["descr"].ToString();
            QuePosAnswers = QuePosAnswer.GetList(Id);
        }

        public QuePosition(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.SpeCalc.ExecuteQueryStoredProcedure("get_question_position", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
        }

        public void Save()
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            SqlParameter pQuestionId = new SqlParameter() { ParameterName = "id_question", SqlValue = Question.Id, SqlDbType = SqlDbType.Int };
            SqlParameter pUserAdSid = new SqlParameter() { ParameterName = "user_sid", SqlValue = User.AdSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pDescr = new SqlParameter() { ParameterName = "descr", SqlValue = Descr, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };

            var dt = Db.SpeCalc.ExecuteQueryStoredProcedure("save_question_position", pId, pQuestionId, pUserAdSid, pDescr, pCreatorAdSid);
            if (dt.Rows.Count > 0)
            {
                int id;
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;
            }
        }

        public static IEnumerable<QuePosition> GetList(int idQuestion)
        {
            SqlParameter pIdQuestion = new SqlParameter() { ParameterName = "id_question", SqlValue = idQuestion, SqlDbType = SqlDbType.Int };
            var dt = Db.SpeCalc.ExecuteQueryStoredProcedure("get_question_position", pIdQuestion);

            var lst = new List<QuePosition>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new QuePosition(row);
                lst.Add(model);
            }

            return lst;
        }

        public static void Close(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.SpeCalc.ExecuteQueryStoredProcedure("close_question_position", pId);
        }
    }
}