using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using DataProvider.Models.SpeCalc;
using DataProvider.Objects;
using Objects;

namespace DataProvider.Controllers.SpeCalc
{
    public class QuePosAnswerController : BaseApiController
    {
        [EnableQuery]
        public IQueryable<QuePosAnswer> GetList(int idQuePosition)
        {
            return new EnumerableQuery<QuePosAnswer>(QuePosAnswer.GetList(idQuePosition));
        }

        public QuePosAnswer Get(int id)
        {
            var model = new QuePosAnswer(id);
            return model;
        }
        [AuthorizeAd(Groups = new[] { AdGroup.SpeCalcKontroler, AdGroup.SpeCalcManager })]
        public HttpResponseMessage Save(QuePosAnswer model)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                model.CurUserAdSid = GetCurUser().Sid;
                model.Save();
                response.Content = new StringContent(String.Format("{{\"id\":{0}}}", model.Id));
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(String.Format("{{\"errorMessage\":\"{0}\"}}", ex.Message));

            }
            return response;
        }

        [AuthorizeAd(Groups = new[] { AdGroup.SpeCalcKontroler, AdGroup.SpeCalcManager })]
        public HttpResponseMessage Close(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                QuePosAnswer.Close(id);
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(String.Format("{{\"errorMessage\":\"{0}\"}}", ex.Message));

            }
            return response;
        }
    }
}
