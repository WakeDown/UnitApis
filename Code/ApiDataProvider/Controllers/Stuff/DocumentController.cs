using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using DataProvider.Models.Stuff;
using DataProvider.Objects;
using Objects;

namespace DataProvider.Controllers.Stuff
{
    public class DocumentController : BaseApiController
    {
        public IEnumerable<Document> GetList()
        {
            return Document.GetList();
        }

        public IEnumerable<Document> GetMyList()
        {
            var emp = new Employee(GetCurUser().Sid);
            int idDepartment = emp.Department.Id;
            int idPosition = emp.Position.Id;
            int idEmployee = emp.Id;

            return Document.GetList(idDepartment, idPosition, idEmployee);
        }

        [AuthorizeAd(Groups = new[] { AdGroup.PersonalManager })]
        public HttpResponseMessage Save(Document model)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                if (model.Data.Length > 512000000) throw new ArgumentException("Размер файла превышает 50 Мб");
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
        [AuthorizeAd(Groups = new[] { AdGroup.PersonalManager })]
        public HttpResponseMessage Close(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                Document.Close(id, GetCurUser().Sid);
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(String.Format("{{\"errorMessage\":\"{0}\"}}", ex.Message));

            }
            return response;
        }

        public HttpResponseMessage GetData(string sid)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();

            httpResponseMessage.Content = new ByteArrayContent(Document.GetData(sid));

            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            httpResponseMessage.StatusCode = HttpStatusCode.OK;

            return httpResponseMessage;

            //byte[] result = null;
            //if (id.HasValue)result = Document.GetData(id.Value);
            //return result;
        }
    }
}
