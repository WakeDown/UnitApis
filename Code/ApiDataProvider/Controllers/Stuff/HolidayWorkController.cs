using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataProvider.Models.Stuff;
using Objects;

namespace DataProvider.Controllers.Stuff
{
    
    public class HolidayWorkController : BaseApiController
    {
        [HttpGet]
        public HolidayResult CheckIsPreHoliday(DateTime? date)
        {
            if (!date.HasValue) throw new ArgumentException("Не указана дата");
            return WorkDay.CheckIsPreHoliday(date.Value);
        }

        public string[] GetConfirms(DateTime? date)
        {
            if (!date.HasValue) throw new ArgumentException("Не указана дата");
            return HolidayWork.GetConfirms(date.Value);
        }


        public HttpResponseMessage SaveConfirm(string fullName)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                string email = HolidayWork.SaveConfirm(fullName);
                response.Content = new StringContent(String.Format("{{\"email\":\"{0}\"}}", email));
                //response.Content = new StringContent(String.Format("{{\"id\":{0}}}", id));
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(String.Format("{{\"errorMessage\":\"{0}\"}}", ex.Message));
            }
            return response;
        }
    }
}
