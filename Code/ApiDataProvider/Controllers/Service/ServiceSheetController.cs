﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataProvider.Helpers;
using DataProvider.Models.Service;
using DataProvider.Objects;
using DataProvider._TMPLTS;
using Objects;

namespace DataProvider.Controllers.Service
{
    public class ServiceSheetController : BaseApiController
    {
        [AuthorizeAd(AdGroup.ServiceControler, AdGroup.ServiceEngeneer, AdGroup.ServiceAdmin, AdGroup.ServiceTech)]
        public IEnumerable<ServiceSheet> GetList(int? idClaim=null, int? idClaim2ClaimState = null)
        {
            return ServiceSheet.GetList(idClaim, idClaim2ClaimState);
        }

        [AuthorizeAd(AdGroup.ServiceControler, AdGroup.ServiceEngeneer, AdGroup.ServiceAdmin, AdGroup.ServiceTech)]
        public ServiceSheet Get(int id)
        {
            var model = new ServiceSheet(id);
            return model;
        }

        [AuthorizeAd(AdGroup.ServiceControler, AdGroup.ServiceEngeneer, AdGroup.ServiceAdmin, AdGroup.ServiceTech)]
        public HttpResponseMessage SaveNotInstalledComment(ServiceSheet model)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                model.CurUserAdSid = GetCurUser().Sid;
                model.SaveNotInstalledComment();
                response.Content = new StringContent($"{{\"id\":{model.Id}}}");
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }

        [HttpPost]
        [AuthorizeAd(AdGroup.ServiceControler, AdGroup.ServiceAdmin)]
        public HttpResponseMessage SavePayed(ServiceSheet model)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                model.CurUserAdSid = GetCurUser().Sid;
                model.SavePayed(GetCurUser());
                response.Content = new StringContent($"{{\"id\":{model.Id}}}");
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }
        
        //[AuthorizeAd(AdGroup.ServiceControler, AdGroup.ServiceEngeneer, AdGroup.ServiceAdmin, AdGroup.ServiceTech)]
        //public HttpResponseMessage Save(ServiceSheet model)
        //{
        //    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

        //    try
        //    {
        //        model.CurUserAdSid = GetCurUser().Sid;
        //        model.Save();
        //        response.Content = new StringContent(String.Format("{{\"id\":{0}}}", model.Id));
        //    }
        //    catch (Exception ex)
        //    {
        //        response = new HttpResponseMessage(HttpStatusCode.OK);
        //        response.Content = new StringContent(String.Format("{{\"errorMessage\":\"{0}\"}}", ex.Message));

        //    }
        //    return response;
        //}
    }
}
