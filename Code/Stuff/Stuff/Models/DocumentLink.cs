using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Stuff.Objects;

namespace Stuff.Models
{
    public class DocumentLink:DbModel
    {
        public int DocumentId { get; set; }
        public List<int> Positions { get; set; }

        public bool Save(out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/Save", OdataServiceUri));
            string json = JsonConvert.SerializeObject(this);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }
    }
}