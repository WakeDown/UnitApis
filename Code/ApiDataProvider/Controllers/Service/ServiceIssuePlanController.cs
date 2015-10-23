using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataProvider.Helpers;
using DataProvider.Models.Service;
using Objects;

namespace DataProvider.Controllers.Service
{
    public class ServiceIssuePlanController : BaseApiController
    {
        public IHttpActionResult Get(int? id)
        {
            if (!id.HasValue) return NotFound();
            
            return Ok(new ServiceIssuePlan(id.Value));
        }

        public IHttpActionResult Get(int? idServiceIssue, int? IdServiceType)
        {
            if (!idServiceIssue.HasValue || !IdServiceType.HasValue) return NotFound();
            
            return Ok(new ServiceIssuePlan(idServiceIssue.Value, IdServiceType.Value));
        }

        public IEnumerable<ServiceIssuePlan> GetList(DateTime? periodStart, DateTime? periodEnd, string engeneerSid = null)
        {
            if (!periodStart.HasValue) periodStart = DateTime.Now;
            if (!periodEnd.HasValue) periodEnd = DateTime.Now;
            return ServiceIssuePlan.GetListUnitProg(periodStart.Value, periodEnd.Value, engeneerSid: engeneerSid);
            //return ServiceIssuePlan.GetList(periodStart.Value, periodEnd.Value);
        }

        public IEnumerable<ServiceIssuePlaningItem> GetCitiesList(DateTime? periodStart, DateTime? periodEnd, string engeneerSid = null)
        {
            if (!periodStart.HasValue) periodStart = DateTime.Now;
            if (!periodEnd.HasValue) periodEnd = DateTime.Now;
            var planList = ServiceIssuePlan.GetListUnitProg(periodStart.Value, periodEnd.Value, engeneerSid:engeneerSid);
            var citiesList = planList.GroupBy(x => x.CityId)
                .Select(x => new ServiceIssuePlaningItem(x.Key, x.First().CityName, x.Count(), x.First().CityShortName, String.Join(",", x.Select(z => z.IdServiceIssue)))).OrderBy(x => x.ShortName).ToArray();

            return citiesList;
            //return ServiceIssuePlan.GetList(periodStart.Value, periodEnd.Value);
        }

        public IEnumerable<ServiceIssuePlaningItem> GetAddressList(DateTime? periodStart, DateTime? periodEnd, int? idCity = null, string engeneerSid = null)
        {
            if (!periodStart.HasValue) periodStart = DateTime.Now;
            if (!periodEnd.HasValue) periodEnd = DateTime.Now;
            var planList = ServiceIssuePlan.GetListUnitProg(periodStart.Value, periodEnd.Value, idCity: idCity, engeneerSid: engeneerSid);
            var addressList = planList.GroupBy(x => x.Address)
                .Select(x => new ServiceIssuePlaningItem(0, x.Key, x.Count(), issuesIdList: String.Join(",", x.Select(z => z.IdServiceIssue))))
                .OrderBy(x => x.Name)
                .ToArray();

            return addressList;
            //return ServiceIssuePlan.GetList(periodStart.Value, periodEnd.Value);
        }

        public IEnumerable<ServiceIssuePlaningItem> GetClientList(DateTime? periodStart, DateTime? periodEnd, int? idCity=null, string address=null, string engeneerSid = null)
        {
            if (!periodStart.HasValue) periodStart = DateTime.Now;
            if (!periodEnd.HasValue) periodEnd = DateTime.Now;
            var planList = ServiceIssuePlan.GetListUnitProg(periodStart.Value, periodEnd.Value, idCity: idCity, address: address, engeneerSid:engeneerSid);
            var clientList = planList.GroupBy(x => x.ClientId)
                .Select(x => new ServiceIssuePlaningItem(x.Key, x.First().ClientName, x.Count(), issuesIdList: String.Join(",", x.Select(z => z.IdServiceIssue))))
                .OrderBy(x => x.Name)
                .ToArray();

            return clientList;
            //return ServiceIssuePlan.GetList(periodStart.Value, periodEnd.Value);
        }

        public IEnumerable<ServiceIssuePlaningItem> GetEngeneerList(DateTime? periodStart, DateTime? periodEnd, string engeneerSid = null)
        {
            if (!periodStart.HasValue) periodStart = DateTime.Now;
            if (!periodEnd.HasValue) periodEnd = DateTime.Now;
            var planList = ServiceIssuePlan.GetListUnitProg(periodStart.Value, periodEnd.Value, engeneerSid: engeneerSid);
            var clientList = planList.GroupBy(x => x.EngeneerSid)
                .Select(x => new ServiceIssuePlaningItem(x.Key, x.First().EngeneerName, x.Count(), issuesIdList: String.Join(",", x.Select(z => z.IdServiceIssue))))
                .OrderBy(x => x.Name)
                .ToArray();

            return clientList;
            //return ServiceIssuePlan.GetList(periodStart.Value, periodEnd.Value);
        }

        public IEnumerable<ServiceIssuePlaningItem> GetDeviceIssueList(DateTime? periodStart, DateTime? periodEnd, int? idCity=null, string address=null, int? idClient=null, string engeneerSid = null)
        {
            if (!periodStart.HasValue) periodStart = DateTime.Now;
            if (!periodEnd.HasValue) periodEnd = DateTime.Now;
            var planList = ServiceIssuePlan.GetListUnitProg(periodStart.Value, periodEnd.Value, idCity:idCity, address: address, idClient: idClient, engeneerSid:engeneerSid);
            var deviceIssueList = planList.Where(x => x.CityId == idCity && x.Address == address).GroupBy(x => x.DeviceId)
                .Select(x => new ServiceIssuePlaningItem(x.First().IdServiceIssue, x.First().DeviceName, x.Count()))
                .OrderBy(x => x.Name)
                .ToArray();

            return deviceIssueList;
            //return ServiceIssuePlan.GetList(periodStart.Value, periodEnd.Value);
        }

        public HttpResponseMessage Save(ServiceIssuePlan model)
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

        public HttpResponseMessage SaveList(IEnumerable<ServiceIssuePlan> list)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                string idList = ServiceIssuePlan.SaveList(GetCurUser().Sid, list);
                response.Content = new StringContent($"{{\"idArr\":\"{idList}\"}}");
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }

        public IEnumerable<ServiceIssuePeriodItem> GetPeriodMonthList(int? year, int? month)
        {
            if (!year.HasValue) year = DateTime.Now.Year;
            if (!month.HasValue) month = DateTime.Now.Month;

            return ServiceIssuePlan.GetPeriodMonthList(year.Value, month.Value);
        }

        public IEnumerable<ServiceIssuePeriodItem> GetPeriodMonthCurPrevNextList(int? year, int? month)
        {
            if (!year.HasValue) year = DateTime.Now.Year;
            if (!month.HasValue) month = DateTime.Now.Month;

            return ServiceIssuePlan.GetPeriodMonthCurPrevNextList(year.Value, month.Value);
        }
        
    }
}
