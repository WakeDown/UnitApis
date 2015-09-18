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
        public IEnumerable<ServiceIssuePlan> GetList(DateTime? periodStart, DateTime? periodEnd)
        {
            if (!periodStart.HasValue) periodStart = DateTime.Now;
            if (!periodEnd.HasValue) periodEnd = DateTime.Now;

            return ServiceIssuePlan.GetList(periodStart.Value, periodEnd.Value);
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

        

        public IEnumerable<ServiceIssuePeriodItem> GetPeriodList(int? year, int? month)
        {
            if (!year.HasValue) year = DateTime.Now.Year;
            if (!month.HasValue) month = DateTime.Now.Month;

            return ServiceIssuePlan.GetPeriodList(year.Value, month.Value);
        }
    }
}
