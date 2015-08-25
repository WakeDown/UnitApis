using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using ClosedXML.Excel;
using DataProvider.Helpers;
using DataProvider.Models.Service;
using Objects;

namespace DataProvider.Controllers.Service
{
    public class ClassifierController : BaseApiController
    {
        public IEnumerable<ClassifierItem> GetList()
        {
            return Classifier.GetList();
        }

        public IEnumerable<ClassifierCaterory> GetCategoryLowerList()
        {
            return ClassifierCaterory.GetLowerList();
        }

        public IEnumerable<WorkType> GetWorkTypeList()
        {
            return WorkType.GetList();
        }

        [HttpPost]
        public HttpResponseMessage SaveAttributes(ClassifierAttributes attrs)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                attrs.CurUserAdSid = GetCurUser().Sid;
                attrs.Save();
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }

        public ClassifierAttributes GetAttributes()
        {
            return ClassifierAttributes.Get();
        }

        public HttpResponseMessage GetExcel()
        {
            var data = Classifier.GetExcel();
            HttpResponseMessage response = Request.CreateResponse();
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new ByteArrayContent(data);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = "Classifier.xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            response.Content.Headers.ContentLength = data.Length;

            return response;
        }

        [HttpPost]
        public HttpResponseMessage SaveFromExcel(byte[] data)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            //try
            //{
                MemoryStream ms = new MemoryStream(data);
                var wb = new XLWorkbook(ms);

                Classifier.SaveFromExcel(wb, GetCurUser().Sid);

                //model.CurUserAdSid = GetCurUser().Sid;
                //model.Save(SetNextState.End);
                //response.Content = new StringContent(String.Format("{{\"id\":{0},\"sid\":\"{1}\"}}", model.Id, model.Sid));
            //}
            //catch (Exception ex)
            //{
            //    response = new HttpResponseMessage(HttpStatusCode.OK);
            //    response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            //}
            return response;
        }
    }
}
