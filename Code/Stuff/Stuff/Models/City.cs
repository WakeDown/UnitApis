using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Stuff.Objects;

namespace Stuff.Models
{
    public class City:DbModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int EmpCount { get; set; }

        public City() { }

        public City(int id)
        {
            Uri uri = new Uri(String.Format("{0}/City/Get?id={1}", OdataServiceUri, id));
            string jsonString = GetJson(uri);
            var model = JsonConvert.DeserializeObject<City>(jsonString);
            FillSelf(model);
        }

        private void FillSelf(City model)
        {
            Id = model.Id;
            Name = model.Name;
            EmpCount = model.EmpCount;
        }

        public static IEnumerable<City> GetSelectionList()
        {
            Uri uri = new Uri(String.Format("{0}/City/GetList", OdataServiceUri));
            string jsonString = GetJson(uri);
            var model = JsonConvert.DeserializeObject<IEnumerable<City>>(jsonString);
            return model;
        }

        public static IEnumerable<City> GetList()
        {
            Uri uri = new Uri(String.Format("{0}/City/GetList", OdataServiceUri));
            string jsonString = GetJson(uri);
            var model = JsonConvert.DeserializeObject<IEnumerable<City>>(jsonString);
            return model;
        }

        public bool Save(out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/City/Save", OdataServiceUri));
            string json = JsonConvert.SerializeObject(this);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }

        public static bool Delete(int id, out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/City/Close?id={1}", OdataServiceUri, id));
            string json = String.Empty;
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }
    }
}