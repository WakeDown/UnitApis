using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Stuff.Objects;

namespace Stuff.Models
{
    public class Organization:DbModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int EmpCount { get; set; }
        public Employee Creator { get; set; }

        public Organization() { }

        public Organization(int id)
        {
            Uri uri = new Uri(String.Format("{0}/Organization/Get?id={1}", OdataServiceUri, id));
            string jsonString = GetJson(uri);
            var model = JsonConvert.DeserializeObject<Organization>(jsonString);
            FillSelf(model);
        }

        private void FillSelf(Organization model)
        {
            Id = model.Id;
            Name = model.Name;
            EmpCount = model.EmpCount;
        }

        public static IEnumerable<Organization> GetSelectionList()
        {
            Uri uri = new Uri(String.Format("{0}/Organization/GetList", OdataServiceUri));
            string jsonString = GetJson(uri);

            var model = JsonConvert.DeserializeObject<IEnumerable<Organization>>(jsonString);

            return model;
        }

        public static IEnumerable<Organization> GetList()
        {
            Uri uri = new Uri(String.Format("{0}/Organization/GetList", OdataServiceUri));
            string jsonString = GetJson(uri);

            var model = JsonConvert.DeserializeObject<IEnumerable<Organization>>(jsonString);

            return model;
        }

        public bool Save(out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/Organization/Save", OdataServiceUri));
            string json = JsonConvert.SerializeObject(this);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }

        public static bool Delete(int id, out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/Organization/Close?id={1}", OdataServiceUri, id));
            string json = String.Empty;//String.Format("{{\"id\":{0}}}",id);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }
    }
}