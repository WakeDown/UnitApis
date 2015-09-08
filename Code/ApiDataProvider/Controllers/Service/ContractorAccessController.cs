using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Models.Service;
using DataProvider.Objects;
using Objects;

namespace DataProvider.Controllers.Service
{
    public class ContractorAccessController : BaseApiController
    {
        [AuthorizeAd(AdGroup.SuperAdmin, AdGroup.ServiceClaimContractorAccess, AdGroup.ServiceControler)]
        public IEnumerable<ContractorAccess> GetList()
        {
            return ContractorAccess.GetList();
        }

        [AuthorizeAd(Groups = new[] { AdGroup.SuperAdmin, AdGroup.ServiceClaimContractorAccess, AdGroup.ServiceControler })]
        public ContractorAccess Get(int id)
        {
            var model = new ContractorAccess(id);
            return model;
        }

        [AuthorizeAd(Groups = new[] { AdGroup.SuperAdmin, AdGroup.ServiceClaimContractorAccess })]
        public HttpResponseMessage Save(ContractorAccess model)
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

        [AuthorizeAd(Groups = new[] { AdGroup.ServiceClaimContractorAccess })]
        public HttpResponseMessage Close(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                ContractorAccess.Close(id, GetCurUser().Sid);
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }

        public IEnumerable<KeyValuePair<string, string>> GetOrgList()
        {
            return AdHelper.GetGroupListByAdOrg(AdOrg.EngeneerGroups);
        }
    }
}