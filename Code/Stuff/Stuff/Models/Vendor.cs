using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Stuff.Objects;

namespace Stuff.Models
{
    public class Vendor:DbModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Vendor() { }

        public Vendor(int id)
        {
            Uri uri = new Uri(String.Format("{0}/Vendor/Get?id={1}", OdataServiceUri, id));
            string jsonString = GetJson(uri);
            var model = JsonConvert.DeserializeObject<Vendor>(jsonString);
            FillSelf(model);
        }

        private void FillSelf(Vendor model)
        {
            Id = model.Id;
            Name = model.Name;
        }

        public static IEnumerable<Vendor> GetList()
        {
            Uri uri = new Uri(String.Format("{0}/Vendor/GetList", OdataServiceUri));
            string jsonString = GetJson(uri);
            var model = JsonConvert.DeserializeObject<IEnumerable<Vendor>>(jsonString);
            return model;
        }

        public bool Save(out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/Vendor/Save", OdataServiceUri));
            string json = JsonConvert.SerializeObject(this);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }

        public static bool Delete(int id, out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/Vendor/Close?id={1}", OdataServiceUri, id));
            string json = String.Empty;
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }
    }
}