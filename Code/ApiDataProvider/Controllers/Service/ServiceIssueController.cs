using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataProvider.Helpers;
using DataProvider.Models.Service;
using DataProvider.Models.Stuff;
using DataProvider.Objects;
using Objects;

namespace DataProvider.Controllers.Service
{
    public class ServiceIssueController : BaseApiController
    {
        public IEnumerable<KeyValuePair<string, string>> GetEngeneerList(string orgSid = null)
        {

            if (orgSid == null)
            {
                return AdHelper.GetUserListByAdGroup(AdGroup.ServiceEngeneer).ToList();
            }
            else
            {
                return AdHelper.GetUserListByAdGroup(orgSid).ToList();
            }
        }

        public ServiceIssueTotal GetTotal(DateTime? month, string serviceEngeneerSid = null, bool? planed = null, bool? seted = null)
        {
            //if (!idCity.HasValue) throw new ArgumentException("Не указан город");
            if (!month.HasValue) month = DateTime.Now;

            var curUser = GetCurUser();
            string serviceAdminSid = null;
            if (curUser.Is(AdGroup.ServiceAdmin)) serviceAdminSid = curUser.Sid;

            var planList = PlanServiceIssue.GetClaimList(month.Value, serviceAdminSid: serviceAdminSid, serviceEngeneerSid: serviceEngeneerSid, planed: planed, seted: seted);
            

            int totalCount = planList.Count();
            int setedCount = planList.Count(x=>x.Seted);
            int noSetedCount = planList.Count(x => !x.Seted);
            int planedCount = planList.Count(x => x.Planed);
            int noPlanedCount = planList.Count(x => !x.Planed);

            var total = new ServiceIssueTotal() {TotalCount = totalCount , SetedCount = setedCount, NoSetedCount = noSetedCount, PlanedCount = planedCount, NoPlanedCount = noPlanedCount };
            return total;
        }

        public IEnumerable<ServiceIssuePlaningItem> GetPlaningDeviceIssueList(DateTime? month, int? idCity, string address, int? idClient, string serviceEngeneerSid = null, bool? planed = null, bool? seted = null)
        {
            if (!idCity.HasValue) throw new ArgumentException("Не указан город");
            //if (!idClient.HasValue) throw new ArgumentException("Не указан клиент");
            if (!month.HasValue) month = DateTime.Now;

            var curUser = GetCurUser();
            string serviceAdminSid = null;
            if (curUser.Is(AdGroup.ServiceAdmin)) serviceAdminSid = curUser.Sid;
            var planList = PlanServiceIssue.GetClaimList(month.Value, idCity, address, idClient, serviceAdminSid: serviceAdminSid, serviceEngeneerSid: serviceEngeneerSid, planed: planed, seted: seted);
            var deviceIssueList = planList.Where(x => x.IdCity == idCity && x.Address == address).GroupBy(x => x.IdDevice)
                .Select(x => new ServiceIssuePlaningItem(x.First().IdServiceClaim, x.First().DeviceName, x.Count()))
                .OrderBy(x => x.Name)
                .ToArray();

            return deviceIssueList;
        }

        public IEnumerable<ServiceIssuePlaningItem> GetPlaningClientList(DateTime? month, int? idCity = null, string address = null, string serviceEngeneerSid = null, bool? planed = null, bool? seted = null)
        {
            //if (!idCity.HasValue) throw new ArgumentException("Не указан город");
            if (!month.HasValue) month = DateTime.Now;

            var curUser = GetCurUser();
            string serviceAdminSid = null;
            if (curUser.Is(AdGroup.ServiceAdmin)) serviceAdminSid = curUser.Sid;

            var planList = PlanServiceIssue.GetClaimList(month.Value, idCity, address, serviceAdminSid: serviceAdminSid, serviceEngeneerSid: serviceEngeneerSid, planed: planed, seted: seted);
            var clientList = planList.GroupBy(x => x.IdClient)
                .Select(x => new ServiceIssuePlaningItem(x.Key, x.First().ClientName, x.Count(), issuesIdList: String.Join(",", x.Select(z => z.IdServiceClaim))))
                .OrderBy(x => x.Name)
                .ToArray();

            return clientList;
        }

        public IEnumerable<ServiceIssuePlaningItem> GetPlaningEngeneerList(DateTime? month, string serviceEngeneerSid = null, bool? planed = null, bool? seted = null)
        {
            //if (!idCity.HasValue) throw new ArgumentException("Не указан город");
            if (!month.HasValue) month = DateTime.Now;

            var curUser = GetCurUser();
            string serviceAdminSid = null;
            if (curUser.Is(AdGroup.ServiceAdmin)) serviceAdminSid = curUser.Sid;

            var planList = PlanServiceIssue.GetClaimList(month.Value, serviceAdminSid: serviceAdminSid, serviceEngeneerSid: serviceEngeneerSid, planed: planed, seted: seted);
            var clientList = planList.GroupBy(x => x.SpecialistSid)
                .Select(x => new ServiceIssuePlaningItem(x.Key, x.First().SpecialistName, x.Count(), issuesIdList: String.Join(",", x.Select(z => z.IdServiceClaim))))
                .OrderBy(x => x.Name)
                .ToArray();

            return clientList;
        }

        public IEnumerable<ServiceIssuePlaningItem> GetPlaningAddressList(DateTime? month, int? idCity, string serviceEngeneerSid = null, int? clientId = null, bool? planed = null, bool? seted = null)
        {
            if (!idCity.HasValue) throw new ArgumentException("Не указан город");
            if (!month.HasValue) month = DateTime.Now;

            var curUser = GetCurUser();
            string serviceAdminSid = null;
            if (curUser.Is(AdGroup.ServiceAdmin)) serviceAdminSid = curUser.Sid;

            var planList = PlanServiceIssue.GetClaimList(month.Value, idCity, serviceAdminSid: serviceAdminSid, serviceEngeneerSid: serviceEngeneerSid, idClient: clientId, planed: planed, seted: seted);
            var addressList = planList.GroupBy(x => x.Address)
                .Select(x => new ServiceIssuePlaningItem(0, x.Key, x.Count(), issuesIdList: String.Join(",", x.Select(z => z.IdServiceClaim))))
                .OrderBy(x => x.Name)
                .ToArray();

            return addressList;
        }

        public IEnumerable<ServiceIssuePlaningItem> GetPlaningCityList(DateTime? month, string serviceEngeneerSid = null, bool? planed = null, int? clientId = null, bool? seted = null)
        {
            if (!month.HasValue) month = DateTime.Now;

            var curUser = GetCurUser();
            string serviceAdminSid = null;
            if (curUser.Is(AdGroup.ServiceAdmin)) serviceAdminSid = curUser.Sid;

            var planList = PlanServiceIssue.GetClaimList(month.Value, serviceAdminSid: serviceAdminSid, serviceEngeneerSid: serviceEngeneerSid, planed:planed, idClient: clientId, seted:seted);
            var citiesList = planList.GroupBy(x => x.IdCity)
                .Select(x => new ServiceIssuePlaningItem(x.Key, x.First().CityName, x.Count(), x.First().CityShortName, String.Join(",", x.Select(z=>z.IdServiceClaim)))).OrderBy(x => x.ShortName).ToArray();

            return citiesList;

            //var result = new ServiceIssuePlaningResult(citiesList);
            //for (int i = 0; i< result.CitiesList.Count();  i++)
            //{
            //    int idCity = result.CitiesList[i].Id;
            //    var addressList = planList.Where(x => x.IdCity == idCity)
            //        .GroupBy(x => x.Address)
            //        .Select(
            //            x =>
            //                new ServiceIssuePlaningResult.ServiceIssuePlaningCityItem.ServiceIssuePlaningAddressItem(0,
            //                    x.Key, x.Count())).OrderBy(x => x.Name).ToArray();
            //    result.CitiesList[i].AddressList = addressList;

            //        for (int j = 0; j < result.CitiesList[i].AddressList.Count(); j++)
            //        {
            //            string address = result.CitiesList[i].AddressList[j].Name;
            //            var clientList =
            //                planList.Where(x => x.IdCity == result.CitiesList[i].Id 
            //                &&  x.Address.Equals(address)).GroupBy(x => x.IdClient)
            //                    .Select(
            //                        x =>
            //                            new ServiceIssuePlaningResult.ServiceIssuePlaningCityItem.
            //                                ServiceIssuePlaningAddressItem.ServiceIssuePlaningClientItem(x.Key,
            //                                    x.First().ClientName, x.Count())).OrderBy(x => x.Name)
            //                    .ToArray();
            //            result.CitiesList[i].AddressList[j].ClientList = clientList;

            //        }
            //}

            //return result;
        }

        [AuthorizeAd(Groups = new[] { AdGroup.SuperAdmin })]
        public IEnumerable<PlanServiceIssue> GetPlanList(DateTime? month)
        {
            if (!month.HasValue) month = DateTime.Now;

            return PlanServiceIssue.GetClaimList(month.Value);
        }

        //TODO: отдельно список для инцедентных выездов и общий

        [AuthorizeAd(Groups = new[] { AdGroup.SuperAdmin })]
        public ServiceIssue Get(int id)
        {
            var model = new ServiceIssue(id);
            return model;
        }

        //[AuthorizeAd(Groups = new[] { AdGroup.SuperAdmin })]
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

        //[AuthorizeAd(AdGroup.ServiceMobileUser)]
        //public HttpResponseMessage MobileSave(PlanServiceIssue model)
        //{
        //    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

        //    try
        //    {
        //        model.CurUserAdSid = GetCurUser().Sid;
        //        model.MobileSave();
        //        response.Content = new StringContent(String.Format("{{\"id\":{0}}}", model.Id));
        //    }
        //    catch (Exception ex)
        //    {
        //        response = new HttpResponseMessage(HttpStatusCode.OK);
        //        response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));
        //    }
        //    return response;
        //}
    }
}
