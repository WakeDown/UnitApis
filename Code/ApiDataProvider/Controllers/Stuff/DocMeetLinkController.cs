using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataProvider.Models.Stuff;
using DataProvider.Objects;
using Objects;

namespace DataProvider.Controllers.Stuff
{
    public class DocMeetLinkController : BaseApiController
    {
        public DocMeetLinkList GetList(int idDocument)
        {
            return DocMeetLinkList.GetList(idDocument);
        }

        [AuthorizeAd(Groups = new[] { AdGroup.PersonalManager })]
        public HttpResponseMessage Save(DocMeetLink model)
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

        [AuthorizeAd(Groups = new[] { AdGroup.PersonalManager })]
        public HttpResponseMessage Close(int? idDocument, int? idDepartment, int? idPosition, int? idEmployee)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                if (!idDocument.HasValue) throw new ArgumentException("Не указан idDocument");

                DocMeetLink.Close(idDocument.Value, GetCurUser().Sid, idDepartment, idPosition, idEmployee);
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
