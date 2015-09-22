using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Stuff.Objects;

namespace Stuff.Models
{
    public class Document:DbModel
    {
        public string Sid { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public Organization Organization { get; set; }
        public DateTime Date { get; set; }

        public byte[] Data { get; set; }

        private void FillSelf(Document model)
        {
            Id = model.Id;
            Name = model.Name;
            Data = model.Data;
        }

        public bool Save(out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/Document/Save", OdataServiceUri));
            string json = JsonConvert.SerializeObject(this);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }

        public static IEnumerable<Document> GetList(int? idDepartment = null, int? idPosition = null, int? idEmployee = null)
        {
            Uri uri = new Uri(String.Format("{0}/Document/GetList?idDepartment={1}&idPosition={2}&idEmployee={3}", OdataServiceUri, idDepartment, idPosition, idEmployee));
            string jsonString = GetJson(uri);

            var model = JsonConvert.DeserializeObject<IEnumerable<Document>>(jsonString);

            return model;
        }

        public static IEnumerable<Document> GetSelectionList(int? idDepartment = null, int? idPosition = null, int? idEmployee = null)
        {
            Uri uri = new Uri(String.Format("{0}/Document/GetList?idDepartment={1}&idPosition={2}&idEmployee={3}", OdataServiceUri, idDepartment, idPosition, idEmployee));
            string jsonString = GetJson(uri);

            var model = JsonConvert.DeserializeObject<IEnumerable<Document>>(jsonString);

            return model;
        }

        public static bool Delete(int id, out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/Document/Close?id={1}", OdataServiceUri, id));
            string json = String.Empty;//String.Format("{{\"id\":{0}}}",id);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }

        public static IEnumerable<Document> GetMyList()
        {
            Uri uri = new Uri(String.Format("{0}/Document/GetMyList", OdataServiceUri));
            string jsonString = GetJson(uri);
            var model = JsonConvert.DeserializeObject<IEnumerable<Document>>(jsonString);
            return model;
        }

        //public static bool GetData(int id, out ResponseMessage responseMessage)
        //{
        //    Uri uri = new Uri(String.Format("{0}/Document/GetData?id={1}", OdataServiceUri, id));
        //    string json = String.Empty;//String.Format("{{\"id\":{0}}}",id);
        //    bool result = GetJson(uri);
        //    return result;
        //}
    }
}