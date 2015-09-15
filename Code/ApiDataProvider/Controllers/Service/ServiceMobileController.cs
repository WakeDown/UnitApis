using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using DataProvider.Helpers;
using DataProvider.Models;
using DataProvider.Models.Service;
using DataProvider.Objects;
using Newtonsoft.Json;
using Objects;

namespace DataProvider.Controllers.Service
{
    [AuthorizeAd(AdGroup.ServiceMobileUser)]
    public class ServiceMobileController : BaseApiController
    {
        [HttpPost]
        public HttpResponseMessage SavePlanServiceIssue(PlanServiceIssue model)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                model.CurUserAdSid = GetCurUser().Sid;
                int id = model.MobileSave();
                response.Content = new StringContent(String.Format("{{\"id\":{0}}}", id));
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));
            }
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return response;
        }

        public HttpResponseMessage GetMobileUserList()
        {
            var model = MobileUser.GetList();
            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(model))
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return resp;
        }

        public HttpResponseMessage GetPlanActionTypeList()
        {
            var model = WorkType.GetPlanActionTypeList();
            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(model))
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return resp;
        }

        public HttpResponseMessage GetDeviceInfo(string serialNum)
        {
            if (String.IsNullOrEmpty(serialNum)) return Request.CreateResponse(HttpStatusCode.NotFound); ;

            var model = Device.GetInfo(serialNum);
            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(model))
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return resp;
        }

        public HttpResponseMessage GetDeviceInfoList()
        {
            var list = Device.GetInfoList();

            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(list))
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return resp;
        }

        public byte[] GetDeviceInfoListHash()
        {
            var list = Device.GetInfoList();
            return MathHelper.GetChecksum(list);
        }
    }
}