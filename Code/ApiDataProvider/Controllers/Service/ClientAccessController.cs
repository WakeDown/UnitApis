using System;
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
    public class ClientAccessController : BaseApiController
    {
        [AuthorizeAd(AdGroup.SuperAdmin, AdGroup.ServiceClaimClientAccess, AdGroup.ServiceControler)]
        public IEnumerable<ClientAccess> GetList()
        {
            return ClientAccess.GetList();
        }

        [AuthorizeAd(Groups = new[] { AdGroup.SuperAdmin, AdGroup.ServiceClaimClientAccess, AdGroup.ServiceControler })]
        public ClientAccess Get(int id)
        {
            var model = new ClientAccess(id);
            return model;
        }

        [AuthorizeAd(Groups = new[] { AdGroup.SuperAdmin, AdGroup.ServiceClaimClientAccess })]
        public HttpResponseMessage SaveNew(ClientAccess model)
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

        [AuthorizeAd(Groups = new[] { AdGroup.SuperAdmin, AdGroup.ServiceClaimClientAccess })]
        public HttpResponseMessage Update(ClientAccess model)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                model.CurUserAdSid = GetCurUser().Sid;
                model.Save(true);
                response.Content = new StringContent(String.Format("{{\"id\":{0}}}", model.Id));
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }

        [AuthorizeAd(Groups = new[] { AdGroup.PersonalManager, AdGroup.ServiceClaimClientAccess })]
        public HttpResponseMessage Close(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                ClientAccess.Close(id, GetCurUser().Sid);
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
