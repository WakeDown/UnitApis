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
    public class QuePositionController : BaseApiController
    {
        [EnableQuery]
        public IQueryable<QuePosition> GetList(int idQuestion)
        {
            return new EnumerableQuery<QuePosition>(QuePosition.GetList(idQuestion));
        }

        public QuePosition Get(int id)
        {
            var model = new QuePosition(id);
            return model;
        }
        [AuthorizeAd(Groups = new[] { AdGroup.SpeCalcKontroler, AdGroup.SpeCalcManager, AdGroup.SpeCalcOperator })]
        public HttpResponseMessage Save(QuePosition model)
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

        [AuthorizeAd(Groups = new[] { AdGroup.SpeCalcKontroler, AdGroup.SpeCalcManager, AdGroup.SpeCalcOperator })]
        public HttpResponseMessage Close(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                QuePosition.Close(id);
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
