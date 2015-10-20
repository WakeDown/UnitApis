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
            return ServiceIssuePlan.GetListUnitProg(periodStart.Value, periodEnd.Value, engeneerSid);
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
