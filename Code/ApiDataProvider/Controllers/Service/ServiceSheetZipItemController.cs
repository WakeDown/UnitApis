using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataProvider.Helpers;
using DataProvider.Models.Service;
using DataProvider.Objects;
using Objects;

namespace DataProvider.Controllers.Service
{
    public class ServiceSheetZipItemController : BaseApiController
    {
        public IEnumerable<ServiceSheetZipItem> GetList(int serviceSheetId)
        {
            return ServiceSheetZipItem.GetList(serviceSheetId);
        }

        public ServiceSheetZipItem Get(int id)
        {
            var model = new ServiceSheetZipItem(id);
            return model;
        }

        [AuthorizeAd()]
        public HttpResponseMessage Save(ServiceSheetZipItem model)
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
        public HttpResponseMessage Close(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                ServiceSheetZipItem.Close(id, GetCurUser().Sid);
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
