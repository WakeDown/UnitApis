using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Stuff.Objects;

namespace Stuff.Models
{
    public class EmpState : DbModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public EmpState() { }

        public EmpState(int id)
        {
            Uri uri = new Uri(String.Format("{0}/EmpState/Get?id={1}", OdataServiceUri, id));
            string jsonString = GetJson(uri);
            var model = JsonConvert.DeserializeObject<EmpState>(jsonString);
            FillSelf(model);
        }

        private void FillSelf(EmpState model)
        {
            Id = model.Id;
            Name = model.Name;
        }

        public static IEnumerable<EmpState> GetSelectionList()
        {
            Uri uri = new Uri(String.Format("{0}/EmpState/GetList", OdataServiceUri));
            string jsonString = GetJson(uri);

            var model = JsonConvert.DeserializeObject<IEnumerable<EmpState>>(jsonString);

            return model;
        }
    }
}