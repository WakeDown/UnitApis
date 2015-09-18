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
    public class Language:DbModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public static string GetLangById(int id)
        {
            return GetList().First(e => e.Id == id).Name;
        }

        public static IEnumerable<Language> GetList()
        {
            Uri uri = new Uri(String.Format("{0}/Language/GetList", OdataServiceUri));
            string jsonString = GetJson(uri);
            var model = JsonConvert.DeserializeObject<IEnumerable<Language>>(jsonString);
            return model;
        }
    }
}
