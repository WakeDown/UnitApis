using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Stuff.Objects;

namespace Stuff.Models
{
    public class Budget : DbModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Descr { get; set; }
        public int? EmpCount { get; set; }

        public Budget() { }

        public Budget(int id)
        {
            Uri uri = new Uri(String.Format("{0}/Budget/Get?id={1}", OdataServiceUri, id));
            string jsonString = GetJson(uri);
            var model = JsonConvert.DeserializeObject<Budget>(jsonString);
            FillSelf(model);
        }

        private void FillSelf(Budget model)
        {
            Id = model.Id;
            Name = model.Name;
            Descr = model.Descr;
        }

        public static SelectList GetSelectionList()
        {
            return new SelectList(GetList(), "Id", "Name");
        }

        public static IEnumerable<Budget> GetList()
        {
            Uri uri = new Uri(String.Format("{0}/Budget/GetList", OdataServiceUri));
            string jsonString = GetJson(uri);
            var model = JsonConvert.DeserializeObject<IEnumerable<Budget>>(jsonString);
            return model;
        }

        public bool Save(out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/Budget/Save", OdataServiceUri));
            string json = JsonConvert.SerializeObject(this);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }

        public static bool Delete(int id, out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/Budget/Close?id={1}", OdataServiceUri, id));
            string json = String.Empty;
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }
    }
}