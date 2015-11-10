using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Web.Http;
using System.Web.Mvc;
using System.Xml;
using DataProvider.Helpers;
using DataProvider.Models.Service;
using DataProvider.Objects;
using Newtonsoft.Json;
using Objects;

namespace DataProvider.Controllers.Service
{
    public class DeviceController : BaseApiController
    {
        public DeviceSearchResult GetSearchList(int? idContractor = null, string contractorName = null, int? idContract = null, string contractNumber = null, int? idDevice = null, string deviceName = null, string serialNum = null)
        {
            var devices = Device.GetList(idContractor, contractorName, idContract, contractNumber, idDevice, deviceName, serialNum);
            var vendors = from device in devices
                group device by device.Vendor
                into d
                orderby d.Key
                select d.Key;
                
                //devices.GroupBy(d=>d.Vendor).Select();

            return new DeviceSearchResult(devices, vendors);
        }

        public IEnumerable<Device> GetList(int? idContractor = null, string contractorName = null, int? idContract = null, string contractNumber = null, int? idDevice = null, string deviceName = null)
        {
            return Device.GetList(idContractor, contractorName, idContract, contractNumber, idDevice, deviceName);
        }

        [AuthorizeAd()]
        public HttpResponseMessage GetInfo(string serialNum)
        {
            if (String.IsNullOrEmpty(serialNum)) return Request.CreateResponse(HttpStatusCode.NotFound); ;

            var model =  Device.GetInfo(serialNum);
            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(model))
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return resp;
        }

        [AuthorizeAd()]
        public HttpResponseMessage GetInfoList()
        {
            var list = Device.GetInfoList();

            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(list))
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return resp;
        }

        public byte[] GetInfoListHash()
        {
            var list = Device.GetInfoList();
            return MathHelper.GetChecksum(list);
        }

        [System.Web.Http.HttpGet]
        public bool CheckSerialNumIsExists(string serialNum, int idClaim)
        {
            int? idDevice;
            bool exists = Device.SerialNumIsExists(serialNum, out idDevice);
            return exists;
        }
    }
}
