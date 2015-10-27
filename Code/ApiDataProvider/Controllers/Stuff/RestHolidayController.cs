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
        /// <summary>
        /// Список периодов
        /// </summary>
        /// <param name="employeeSid"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public IEnumerable<RestHoliday> GetList(string employeeSid = null, int? year = null)
        {
            var curUser = GetCurUser();
            //if (!curUser.HasAccess(AdGroup.RestHolidayViewAllEmpList))
            //{
            //    employeeSid = curUser.Sid;
            //}
            
            return RestHoliday.GetList(employeeSid, year ?? DateTime.Now.Year);
        }

        public RestHoliday Get(int id)
        {
            var model = new RestHoliday(id);
            return model;
        }

        public DateTime GetEndDate(DateTime dateStart, int duration)
        {
           return RestHoliday.GetEndDate(dateStart, duration);
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

        /// <summary>
        /// Закрытие/открытие возможности редактировани периода автору
        /// </summary>
        /// <param name="idArray"></param>
        /// <param name="canEdit"></param>
        /// <returns></returns>
        //[AuthorizeAd(AdGroup.RestHolidayConfirm)]
        public HttpResponseMessage CanEdit(int[] idArray, bool canEdit = false)
        {
            if (canEdit && !GetCurUser().HasAccess(AdGroup.RestHolidayConfirm)) return new HttpResponseMessage(HttpStatusCode.Unauthorized);

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

        /// <summary>
        /// Закрытие/открытие возможности редактировани периода автору
        /// </summary>
        /// <param name="employeeSid"></param>
        /// /// <param name="year"></param>
        /// <param name="canEdit"></param>
        /// <returns></returns>
        //[AuthorizeAd(AdGroup.RestHolidayConfirm)]
        public HttpResponseMessage CanEdit(string employeeSid, int year, bool canEdit = false)
        {
            if (canEdit && !GetCurUser().HasAccess(AdGroup.RestHolidayConfirm)) return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                RestHoliday.Confirm(GetCurUser().Sid, employeeSid, year, canEdit);
                //response.Content = new StringContent(String.Format("{{\"id\":{0}}}", model.Id));
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));
            }
            return response;
        }

        [AuthorizeAd(AdGroup.RestHolidayConfirm)]
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

        [AuthorizeAd(AdGroup.RestHolidayConfirm)]
        public HttpResponseMessage Confirm(string employeeSid, int year)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                RestHoliday.Confirm(GetCurUser().Sid, employeeSid, year, confirmed: true);
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

        /// <summary>
        /// Возвращают пару год число оставшихся в году дней по сиду
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="full">Ограничение по количеству выводимых годов</param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<int, int>> GetYears4Employee(string sid, bool full =false)
        {
            int? topRows = null;
            if (!full)
            {
                topRows=  3;
            }
            return RestHoliday.GetYears4Employee(sid, topRows);
        }

        /// <summary>
        /// Получает cписок всех EmployeeRestHolidays в указанном году
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public IEnumerable<EmployeeRestHoliday> GetEmployeeList(int year)
        {
            return RestHoliday.GetEmployeeList(year);
        }

    }
}
