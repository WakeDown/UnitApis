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
        }

        public static IEnumerable<Position> GetSelectionList()
        {
            Uri uri = new Uri(String.Format("{0}/Position/GetList", OdataServiceUri));
            string jsonString = GetJson(uri);

            var model = JsonConvert.DeserializeObject<IEnumerable<Position>>(jsonString);

            return model;
        }
    }

    
}