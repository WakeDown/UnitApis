using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Models.Stuff;
using DataProvider.Objects;
using WebGrease.Css.Ast.Selectors;

namespace DataProvider.Models.SpeCalc
{
    public class Question : DbModel
    {
        public int Id { get; set; }
        public Employee Manager { get; set; }
        public DateTime DateLimit { get; set; }
        public string Descr { get; set; }
        public Employee Creator { get; set; }
        public QueState State { get; set; }
        public DateTime DateCreate { get; set; }


        public Question() { }

        public Question(DataRow row)
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueInt(row["id"]);
            Manager = new Employee(row["manager_sid"].ToString());
            DateLimit = Db.DbHelper.GetValueDateTime(row["date_limit"]);
            Descr = row["descr"].ToString();
            State = new QueState(Db.DbHelper.GetValueInt(row["id_que_state"]));
            DateCreate = Db.DbHelper.GetValueDateTime(row["dattim1"]);
            //State = new QueState() { Id = Db.DbHelper.GetValueInt(row["id_que_state"]), Name = row["que_state"].ToString() };
        }

        public Question(int id)
        {
            SqlParameter pId = new SqlParameter() {ParameterName = "id", SqlValue = id,SqlDbType = SqlDbType.Int };
            SqlParameter pTop = new SqlParameter() { ParameterName = "top", SqlValue = 1, SqlDbType = SqlDbType.Int };
            var dt = Db.SpeCalc.ExecuteQueryStoredProcedure("get_question", pId, pTop);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
        }

        public static QueState GetQuestionCurrState(int? idQuestion)
        {
            var state = new QueState();
            SqlParameter pId = new SqlParameter() { ParameterName = "id_question", SqlValue = idQuestion, SqlDbType = SqlDbType.Int };
            var dt = Db.SpeCalc.ExecuteQueryStoredProcedure("get_question_curr_state", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                state = new QueState(row);
            }
            return state;
        }

        public void Save()
        {
            if (Id <= 0) this.State = new QueState().GetFirstState();

            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            SqlParameter pManagerAdSid = new SqlParameter() { ParameterName = "manager_sid", SqlValue = Manager.AdSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pDateLimit = new SqlParameter() { ParameterName = "date_limit", SqlValue = DateLimit, SqlDbType = SqlDbType.DateTime };
            SqlParameter pDescr = new SqlParameter() { ParameterName = "descr", SqlValue = Descr, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pIdQueState = new SqlParameter() { ParameterName = "id_que_state", SqlValue = State.Id, SqlDbType = SqlDbType.Int };

            var dt = Db.SpeCalc.ExecuteQueryStoredProcedure("save_question", pId, pManagerAdSid, pDateLimit, pDescr, pCreatorAdSid, pIdQueState);
            if (dt.Rows.Count > 0)
            {
                int id;
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;
                SetQuestionState(id, new QueState().GetFirstState().Id, null, CurUserAdSid);
            }

            
        }

        public static IEnumerable<Question> GetList(AdUser curUser, int? id = null, string managerSid = null, string queStates = null, int? top = null, string prodSid = null)
        {
            if (!top.HasValue) top = 30;
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            SqlParameter pManagerSid = new SqlParameter() { ParameterName = "manager_sid", SqlValue = managerSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pQueStates = new SqlParameter() { ParameterName = "lst_que_states", SqlValue = queStates, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pTop = new SqlParameter() { ParameterName = "top", SqlValue = top, SqlDbType = SqlDbType.Int };
            SqlParameter pProdSid = new SqlParameter() { ParameterName = "prod_sid", SqlValue = prodSid, SqlDbType = SqlDbType.VarChar };

            if (!AdHelper.UserInGroup(curUser.User, AdGroup.SuperAdmin, AdGroup.SpeCalcKontroler))
            {
                if (AdHelper.UserInGroup(curUser.User, AdGroup.SpeCalcManager))
                {
                    pManagerSid.SqlValue = curUser.Sid;
                }

                if (AdHelper.UserInGroup(curUser.User, AdGroup.SpeCalcProduct))
                {
                    pProdSid.SqlValue = curUser.Sid;
                }
            }

            var dt = Db.SpeCalc.ExecuteQueryStoredProcedure("get_question", pId, pManagerSid, pQueStates, pTop, pProdSid);

            var lst = new List<Question>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new Question(row);
                lst.Add(model);
            }

            return lst;
        }
        
        public static void Close(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.SpeCalc.ExecuteQueryStoredProcedure("close_question", pId);
        }

        public static bool CheckQuestionCanBeAnswered(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id_question", SqlValue = id, SqlDbType = SqlDbType.Int };

            var dt = Db.SpeCalc.ExecuteQueryStoredProcedure("check_question_can_be_answered", pId);

            bool result = false;
            if (dt.Rows.Count > 0)
            {
                result = dt.Rows[0]["result"].ToString().Equals("1");
            }

            return result;
        }

        public static bool CheckQuestionCanBeSent(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id_question", SqlValue = id, SqlDbType = SqlDbType.Int };

            var dt = Db.SpeCalc.ExecuteQueryStoredProcedure("check_question_can_be_sent", pId);

            bool result = false;
            if (dt.Rows.Count > 0)
            {
                result = dt.Rows[0]["result"].ToString().Equals("1");
            }

            return result;
        }

        public static void SetQuestionState(int id, int idQueState, string descr, string creatorSid)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id_question", SqlValue = id, SqlDbType = SqlDbType.Int };
            SqlParameter pIdQueState = new SqlParameter() { ParameterName = "id_que_state", SqlValue = idQueState, SqlDbType = SqlDbType.Int };
            SqlParameter pDescr = new SqlParameter() { ParameterName = "descr", SqlValue = descr, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCreatorSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = creatorSid, SqlDbType = SqlDbType.VarChar };

            Db.SpeCalc.ExecuteStoredProcedure("save_question_state", pId, pIdQueState, pDescr, pCreatorSid);
        }

        public static void SetSentState(int id, string descr, string creatorSid)
        {
            if (CheckQuestionCanBeSent(id))
            {
                SetQuestionState(id, new QueState().GetSentState().Id, descr, creatorSid);
                SendQuestionSentNote(id);
            }
            else
            {
                throw new Exception("Невозможно передать позицию в работу! Введите и сохраните хотябы один вопрос!");
            }
        }

        private static void SendQuestionSentNote(int id, bool isTest = false)
        {
            string body = String.Format("<p>Добрый день!</p><p>Вам направлен вопрос в системе СпецРасчет.</p>" +
                                        "<p>Ссылка: <a href='{0}'>{0}</a></p>", Settings.SpeCalc.Url + "/Question/Index/" + id);
            var recipients = QuePosition.GetList(id).Select(p => new Employee(p.User.AdSid).Email).ToArray();
            MessageHelper.SendMailSmtp("Новый вопрос в системе СпецРасчет", body, true, recipients, null, Settings.SpeCalc.DefaultMailFrom, isTest);
        }

        public static void SetProcessState(int id, string descr, string creatorSid)
        {
            SetQuestionState(id, new QueState().GetProcessState().Id, descr, creatorSid);
        }

        public static void SetAnsweredState(int id, string descr, string creatorSid)
        {
            //Проверяем все ли заполнено
            if (CheckQuestionCanBeAnswered(id))
            {
                SetQuestionState(id, new QueState().GetAnsweredState().Id, descr, creatorSid);
                SendQuestionAnsweredNote(id);
            }
            else
            {
                throw new Exception("Невозможно передать позицию на подтверждение! Не все позиции заполнены!");
            }
        }

        private static void SendQuestionAnsweredNote(int id, bool isTest = false)
        {
            string body = String.Format("<p>Добрый день!</p><p>Получен ответ на ваш вопрос в системе СпецРасчет.</p>" +
                                        "<p>Ссылка: <a href='{0}'>{0}</a></p>", Settings.SpeCalc.Url + "/Question/Index/" + id);
            var recipients = new Employee(new Question(id).Manager.AdSid).Email;
            MessageHelper.SendMailSmtp("Ответ на вопрос в системе СпецРасчет", body, true, recipients, null, Settings.SpeCalc.DefaultMailFrom, isTest);
        }

        public static void SetAprovedState(int id, string descr, string creatorSid)
        {
            SetQuestionState(id, new QueState().GetAprovedState().Id, descr, creatorSid);
        }
    }
}