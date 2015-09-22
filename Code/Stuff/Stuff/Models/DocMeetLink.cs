using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Stuff.Objects;

namespace Stuff.Models
{
    public class DocMeetLink:DbModel
    {
        public int Id { get; set; }
        public int IdDocument { get; set; }
        public int? IdDepartment { get; set; }
        public int? IdPosition { get; set; }
        public int? IdEmployee { get; set; }

        public static DocMeetLinkList GetList(int idDocument)
        {
            Uri uri = new Uri(String.Format("{0}/DocMeetLink/GetList?idDocument={1}", OdataServiceUri, idDocument));
            string jsonString = GetJson(uri);

            var model = JsonConvert.DeserializeObject<DocMeetLinkList>(jsonString);

            return model;
        }

        public bool Save(out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/DocMeetLink/Save", OdataServiceUri));
            string json = JsonConvert.SerializeObject(this);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }

        public static bool Delete(int idDocument, int? idDepartment, int? idPosition, int? idEmployee, out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/DocMeetLink/Close?idDocument={1}&idDepartment={2}&idPosition={3}&idEmployee={4}", OdataServiceUri, idDocument, idDepartment, idPosition, idEmployee));
            string json = String.Empty;//JsonConvert.SerializeObject(new DocMeetLink(){IdDocument = idDocument, IdDepartment = idDepartment, IdPosition = idPosition, IdEmployee = idEmployee});
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }
    }
}