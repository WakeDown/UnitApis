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
        }

        public static IEnumerable<City> GetSelectionList()
        {
            Uri uri = new Uri(String.Format("{0}/City/GetList", OdataServiceUri));
            string jsonString = GetJson(uri);

            var model = JsonConvert.DeserializeObject<IEnumerable<City>>(jsonString);

            return model;
        }
    }
}