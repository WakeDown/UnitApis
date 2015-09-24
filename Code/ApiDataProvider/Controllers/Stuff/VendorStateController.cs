using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Channels;
using System.Web.Http;
using System.Web.Http.OData;
using DataProvider.Helpers;
using DataProvider.Models.Stuff;
using DataProvider.Objects;
using Objects;

namespace DataProvider.Controllers.Stuff
{
    public class VendorStateController : BaseApiController
    {
        public VendorState Get(int id)
        {
            return new VendorState(id);
        }
        public IEnumerable<VendorState> GetList()
        {
            return VendorState.GetList();
        }

        public IEnumerable<VendorState> GetDeliverList(byte listType)
        {
            byte expires = (byte)(listType==2? 1 : 0);
            byte newbie = (byte)(listType == 0 ? 1 : 0);
            byte updated = (byte)(listType == 1 ? 1 : 0);
            return VendorState.DeliverList(expires, newbie, updated);
        }

        public VendorState GetPrevValue(int id)
        {
            return VendorState.GetPrevValue(id);
        } 
        public HttpResponseMessage SetDeliverySent(VendorState[] vendorStates, byte deliveryType)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);
            byte expires = (byte) (deliveryType == 2 ? 1 : 0);
            byte newbie = (byte)(deliveryType == 0 ? 1 : 0);
            byte updated = (byte)(deliveryType == 1 ? 1 : 0);
            try
            {
                foreach (VendorState vendorState in vendorStates)
                {
                    VendorState.SetDeliverySent(vendorState.Id, expires, newbie, updated);
                }
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));
            }
            return response;
        }
        public IEnumerable<string> GetMailAddressList()
        {
           return(VendorState.GetMailAddressVendorStateExpiresDeliveryList());
        }
        public IEnumerable<VendorState> GetHistoryList(int id)
        {
            return VendorState.GetHistoryList(id);
        }
        [AuthorizeAd(AdGroup.VendorStateEditor)]
        public HttpResponseMessage Save(VendorState vnd)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                vnd.CurUserAdSid = GetCurUser().Sid;
                vnd.Save();
                response.Content = new StringContent(String.Format("{{\"id\":{0}}}", vnd.Id));
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));
            }
            return response;
        }
        [AuthorizeAd(AdGroup.VendorStateEditor)]
        public HttpResponseMessage Close(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);
            var deleter = GetCurUser().Sid;
            try
            {
                VendorState.Close(id, deleter);
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