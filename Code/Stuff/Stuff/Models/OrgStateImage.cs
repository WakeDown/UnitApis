using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Stuff.Objects;

namespace Stuff.Models
{
    public class OrgStateImage:DbModel
    {
        public int Id { get; set; }
        public int IdOrganization { get; set; }
        public byte[] Image { get; set; }

        public OrgStateImage() { }

        public OrgStateImage(byte[] data)
        {
            Image = data;
        }

        //private void FillSelf(OrgStateImage model)
        //{
        //    Id = model.Id;
        //    IdOrganization = model.IdOrganization;
        //    Image = model.Image;
        //}

        //public bool Save(out ResponseMessage responseMessage)
        //{
        //    Uri uri = new Uri(String.Format("{0}/Department/Save", OdataServiceUri));
        //    string json = JsonConvert.SerializeObject(this);
        //    bool result = PostJson(uri, json, out responseMessage);
        //    return result;
        //}

        //public static IEnumerable<Department> GetList()
        //{
        //    Uri uri = new Uri(String.Format("{0}/Department/GetList", OdataServiceUri));
        //    string jsonString = GetJson(uri);

        //    var deps = JsonConvert.DeserializeObject<IEnumerable<Department>>(jsonString);

        //    return deps;
        //}

        public static bool Delete(int id, out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/Organization/CloseStateImage?id={1}", OdataServiceUri, id));
            string json = String.Empty;//String.Format("{{\"id\":{0}}}",id);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }
    }
}