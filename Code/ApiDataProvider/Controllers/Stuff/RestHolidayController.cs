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
    public class RestHolidayController : BaseApiController
    {
        public IEnumerable<RestHoliday> GetList()
        {
            return RestHoliday.GetList();
        }

        public RestHoliday Get(int id)
        {
            var model = new RestHoliday(id);
            return model;
        }

        [AuthorizeAd()]
        public HttpResponseMessage Save(RestHoliday model)
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
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }

        [AuthorizeAd()]
        public HttpResponseMessage CanEdit(int[] idArray, bool canEdit = false)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                RestHoliday.Confirm(GetCurUser().Sid, idArray, canEdit);
                //response.Content = new StringContent(String.Format("{{\"id\":{0}}}", model.Id));
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }

        [AuthorizeAd()]
        public HttpResponseMessage Confirm(int[] idArray)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                RestHoliday.Confirm(GetCurUser().Sid, idArray, confirmed: true);
                //response.Content = new StringContent(String.Format("{{\"id\":{0}}}", model.Id));
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
                RestHoliday.Close(id, GetCurUser().Sid);
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
