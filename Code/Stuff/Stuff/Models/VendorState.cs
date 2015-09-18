using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Stuff.Objects;

namespace Stuff.Models
{
    public class VendorState : DbModel
    {
        public int? Id { get; set; }
        public string StateName { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string StateDescription { get; set; }
        public byte [] Picture { get; set; }
        public DateTime EndDate { get; set; }
        public int UnitOrganizationId { get; set; }
        public string UnitOrganizationName { get; set; }
        public int LanguageId { get; set; }
        public string LanguageName { get; set; }
        public string Author { get; set; }
        public DateTime CreationDate { get; set; }

        public VendorState(int id)
        {
            Uri uri = new Uri(String.Format("{0}/VendorState/Get?id={1}", OdataServiceUri, id));
             string jsonString = GetJson(uri);
            var vnd = JsonConvert.DeserializeObject<VendorState>(jsonString);
            FillSelf(vnd);
        }

        public VendorState()
        {
           
        }
        private void FillSelf(VendorState vnd)
        {
            Id = vnd.Id;
            StateName = vnd.StateName;
            VendorId = vnd.VendorId;
            VendorName = vnd.VendorName;
            StateDescription = vnd.StateDescription;
            Picture = vnd.Picture;
            EndDate = vnd.EndDate;
            UnitOrganizationId = vnd.UnitOrganizationId;
            LanguageId = vnd.LanguageId;
        }
        public bool Save(out ResponseMessage responseMessage)
        {
            
            Uri uri = new Uri(String.Format("{0}/VendorState/Save", OdataServiceUri));
            string json = JsonConvert.SerializeObject(this);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }

        public static bool Delete(int id, out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/VendorState/Close?id={1}", OdataServiceUri, id));
            string json = String.Empty;//String.Format("{{\"id\":{0}}}",id);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }
        public static List<VendorState> GetList()
        {
            Uri uri = new Uri(String.Format("{0}/VendorState/GetList", OdataServiceUri));
            string json = GetJson(uri);
            var list = JsonConvert.DeserializeObject<List<VendorState>>(json);
            return (list);
        }

        public static List<VendorState> GetHistoryList(int id)
        {
            Uri uri = new Uri(String.Format("{0}/VendorState/GetHistoryList?id={1}", OdataServiceUri, id));
            string json = GetJson(uri);
            var list = JsonConvert.DeserializeObject<List<VendorState>>(json);
            return (list);
        }
    }
}
