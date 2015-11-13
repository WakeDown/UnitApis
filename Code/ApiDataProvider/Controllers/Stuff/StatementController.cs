using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataProvider.Helpers;
using DataProvider.Models.Stuff;
using DataProvider.Objects;
using Objects;

namespace DataProvider.Controllers.Stuff
{
    public class StatementController : BaseApiController
    {
        public IEnumerable<StatementPrint> GetPrintList()
        {
            return StatementPrint.GetList();
        }

        //public StatementPrint Get(int id)
        //{
        //    var model = new StatementPrint(id);
        //    return model;
        //}

        [AuthorizeAd()]
        public HttpResponseMessage Save(StatementPrint model)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                model.CurUserAdSid = GetCurUser().Sid;
                model.Save();
                response.Content = new StringContent($"{{\"id\":{model.Id}}}");
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));
            }
            return response;
        }

        [AuthorizeAd()]
        public IHttpActionResult SetConfirmed(int id)
        {
            StatementPrint.Confirm(id, GetCurUser().Sid, true);
            return Ok();
        }

        [AuthorizeAd()]
        public IHttpActionResult SetUnconfirmed(int id)
        {
            StatementPrint.Confirm(id, GetCurUser().Sid, false);
            return Ok();
        }

        //[AuthorizeAd()]
        //public HttpResponseMessage Close(int id)
        //{
        //    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

        //    try
        //    {
        //        StatementPrint.Close(id, GetCurUser().Sid);
        //    }
        //    catch (Exception ex)
        //    {
        //        response = new HttpResponseMessage(HttpStatusCode.OK);
        //        response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

        //    }
        //    return response;
        //}

        public IEnumerable<StatementType> GetTypeList()
        {
            return StatementType.GetList();
        }

        public StatementType GetType(string sysName)
        {
            return new StatementType(sysName);
        }
    }
}
