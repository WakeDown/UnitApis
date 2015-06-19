using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Stuff.Objects;

namespace Stuff.Models
{
    public class Position : DbModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int EmpCount { get; set; }

        public Position() { }

        public Position(int id)
        {
            Uri uri = new Uri(String.Format("{0}/Position/Get?id={1}", OdataServiceUri, id));
            string jsonString = GetJson(uri);
            var model = JsonConvert.DeserializeObject<Position>(jsonString);
            FillSelf(model);
        }

        private void FillSelf(Position model)
        {
            Id = model.Id;
            Name = model.Name;
            EmpCount = model.EmpCount;
        }

        public static IEnumerable<Position> GetSelectionList()
        {
            Uri uri = new Uri(String.Format("{0}/Position/GetList", OdataServiceUri));
            string jsonString = GetJson(uri);

            var model = JsonConvert.DeserializeObject<IEnumerable<Position>>(jsonString);

            return model;
        }

        public static IEnumerable<Position> GetList()
        {
            Uri uri = new Uri(String.Format("{0}/Position/GetList", OdataServiceUri));
            string jsonString = GetJson(uri);

            var model = JsonConvert.DeserializeObject<IEnumerable<Position>>(jsonString);

            return model;
        }

        public bool Save(out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/Position/Save", OdataServiceUri));
            string json = JsonConvert.SerializeObject(this);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }

        public static bool Delete(int id, out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/Position/Close?id={1}", OdataServiceUri, id));
            string json = String.Empty;//String.Format("{{\"id\":{0}}}",id);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }
    }

    
}