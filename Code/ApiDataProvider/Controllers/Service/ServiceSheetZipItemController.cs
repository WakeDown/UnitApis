using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataProvider.Helpers;
using DataProvider.Models.Service;
using DataProvider.Objects;
using Objects;

namespace DataProvider.Controllers.Service
{
    public class ServiceSheetZipItemController : BaseApiController
    {
        public IEnumerable<ServiceSheetZipItem> GetIssuedList(int serviceSheetId)
        {
            return ServiceSheetZipItem.GetIssuedList(serviceSheetId);
        }

        public IEnumerable<ServiceSheetZipItem> GetOrderedList(int serviceSheetId, bool? realyOrdered = null)
        {
            return ServiceSheetZipItem.GetOrderedList(serviceSheetId, realyOrdered);
        }

        public IEnumerable<ServiceSheetZipItem> GetInstalledList(int serviceSheetId)
        {
            return ServiceSheetZipItem.GetInstalledList(serviceSheetId);
        }

        public IEnumerable<ServiceSheetZipItem> GetNotInstalledList(int serviceSheetId)
        {
            return ServiceSheetZipItem.GetNotInstalledList(serviceSheetId);
        }

        public ServiceSheetZipItem Get(int id)
        {
            var model = new ServiceSheetZipItem(id);
            return model;
        }

       

        [AuthorizeAd()]
        public HttpResponseMessage NotInstalledSaveList(int[] idOrderedZipItem, int serviceSheetId)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                ServiceSheetZipItem.NotInstalledSaveList(idOrderedZipItem, serviceSheetId, GetCurUser().Sid);
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }

        [AuthorizeAd()]
        public HttpResponseMessage IssuedClose(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                ServiceSheetZipItem.IssuedClose(id, GetCurUser().Sid);
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }

        [AuthorizeAd()]
        public HttpResponseMessage IssuedSave(ServiceSheetZipItem model)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                model.CurUserAdSid = GetCurUser().Sid;
                model.IssuedSave();
                response.Content = new StringContent($"{{\"id\":{model.Id}}}");
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }

        [AuthorizeAd()]
        public HttpResponseMessage OrderedClose(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                ServiceSheetZipItem.OrderedClose(id, GetCurUser().Sid);
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }

        [AuthorizeAd()]
        public HttpResponseMessage OrderedSave(ServiceSheetZipItem model)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                model.CurUserAdSid = GetCurUser().Sid;
                model.OrderedSave();
                response.Content = new StringContent($"{{\"id\":{model.Id}}}");
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }

        [AuthorizeAd()]
        public HttpResponseMessage SetInstalled(int id, int idServiceSheet, bool? installed = true)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                ServiceSheetZipItem.SetInstalled(id, idServiceSheet, GetCurUser().Sid, installed);
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
