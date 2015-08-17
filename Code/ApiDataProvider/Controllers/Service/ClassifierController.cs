using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ClosedXML.Excel;
using DataProvider.Helpers;
using DataProvider.Models.Service;
using Objects;

namespace DataProvider.Controllers.Service
{
    public class ClassifierController : BaseApiController
    {
        public IEnumerable<ClassifierCaterory> GetList()
        {
            return ClassifierCaterory.GetList();
        }

        [HttpPost]
        public HttpResponseMessage SaveFromExcel(byte[] data)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                MemoryStream ms = new MemoryStream(data);
                var wb = new XLWorkbook(ms);

                Classifier.SaveFromExcel(wb, GetCurUser().Sid);

                //model.CurUserAdSid = GetCurUser().Sid;
                //model.Save(SetNextState.End);
                //response.Content = new StringContent(String.Format("{{\"id\":{0},\"sid\":\"{1}\"}}", model.Id, model.Sid));
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }
    }
}
