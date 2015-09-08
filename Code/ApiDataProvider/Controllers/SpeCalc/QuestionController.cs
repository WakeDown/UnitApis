using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using DataProvider.Helpers;
using DataProvider.Models.SpeCalc;
using DataProvider.Models.Stuff;
using DataProvider.Objects;
using Objects;

namespace DataProvider.Controllers.SpeCalc
{
    //[AuthorizeAd(Groups = new[] { AdGroup.SpeCalcKontroler, AdGroup.SpeCalcManager, AdGroup.SpeCalcProduct, AdGroup.SpeCalcOperator })]
    public class QuestionController : BaseApiController
    {

        [EnableQuery]
        public IQueryable<Question> GetList(int? id = null, string managerSid = null, string queStates = null, int? top = null, string prodSid = null)
        {
            return new EnumerableQuery<Question>(Question.GetList(GetCurUser(), id, managerSid, queStates, top, prodSid));
        }

        public Question Get(int id)
        {
            var curSid = GetCurUser().Sid;
            var model = new Question(id) { CurUserAdSid = curSid };
            return model;
        }
        
        public QueState GetQuestionCurrState(int idQuestion)
        {
            return Question.GetQuestionCurrState(idQuestion);
        }
        
        public HttpResponseMessage Save(Question model)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                model.CurUserAdSid = GetCurUser().Sid;
                model.Save();
                response.Content = new StringContent(String.Format("{{\"id\":{0}, \"sid\":\"{1}\"}}", model.Id, model.CurUserAdSid));
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }

        
        public HttpResponseMessage Close(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                Question.Close(id);
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(String.Format("{{\"errorMessage\":\"{0}\"}}", ex.Message));

            }
            return response;
        }

        public HttpResponseMessage SetQuestionSent(int id, string descr = null)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                Question.SetSentState(id, descr, GetCurUser().Sid);
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(String.Format("{{\"errorMessage\":\"{0}\"}}", ex.Message));

            }
            return response;
        }

        
        public HttpResponseMessage SetQuestionProcess(int id, string descr = null)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                Question.SetProcessState(id, descr, GetCurUser().Sid);
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(String.Format("{{\"errorMessage\":\"{0}\"}}", ex.Message));
            }
            return response;
        }

        public HttpResponseMessage SetQuestionAnswered(int id, string descr = null)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                Question.SetAnsweredState(id, descr, GetCurUser().Sid);
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(String.Format("{{\"errorMessage\":\"{0}\"}}", ex.Message));
            }
            return response;
        }

        public HttpResponseMessage SetQuestionAproved(int id, string descr = null)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                Question.SetAprovedState(id, descr, GetCurUser().Sid);
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(String.Format("{{\"errorMessage\":\"{0}\"}}", ex.Message));

            }
            return response;
        }

        public IEnumerable<HistoryQueState> GetStateHistory(int idQuestion)
        {
            return HistoryQueState.GetStateHistory(idQuestion);
        }
    }
}
