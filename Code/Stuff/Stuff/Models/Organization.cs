using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Stuff.Objects;

namespace Stuff.Models
{
    public class Organization:DbModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int EmpCount { get; set; }
        //public Employee Creator { get; set; }
        public string AddressUr { get; set; }
        public string AddressFact { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Inn { get; set; }
        public string Kpp { get; set; }
        public string Ogrn { get; set; }
        public string Rs { get; set; }
        public string Bank { get; set; }
        public string Ks { get; set; }
        public string Bik { get; set; }
        public string Okpo { get; set; }
        public string Okved { get; set; }
        //public string ManagerShortName { get; set; }
        //public string ManagerName { get; set; }
        //public string ManagerNameDat { get; set; }
        //public string ManagerPosition { get; set; }
        //public string ManagerPositionDat { get; set; }
        public string Site { get; set; }
        public Employee Director { get; set; }

        public List<OrgStateImage> StateImages { get; set; }

        public IEnumerable<Image> Statuses { get; set; }

        public Organization()
        {
            StateImages = new List<OrgStateImage>();
            Director = new Employee();
        }

        public Organization(int id): this()
        {
            Uri uri = new Uri(String.Format("{0}/Organization/Get?id={1}", OdataServiceUri, id));
            string jsonString = GetJson(uri);
            var model = JsonConvert.DeserializeObject<Organization>(jsonString);
            FillSelf(model);
        }

        public Organization(string sysName)
            : this()
        {
            Uri uri = new Uri(String.Format("{0}/Organization/Get?sysName={1}", OdataServiceUri, sysName));
            string jsonString = GetJson(uri);
            var model = JsonConvert.DeserializeObject<Organization>(jsonString);
            FillSelf(model);
        }

        private void FillSelf(Organization model)
        {
            Id = model.Id;
            Name = model.Name;
            EmpCount = model.EmpCount;
            AddressUr = model.AddressUr;
            AddressFact = model.AddressFact;
            Phone = model.Phone;
            Email = model.Email;
            Inn = model.Inn;
            Kpp = model.Kpp;
            Ogrn = model.Ogrn;
            Rs = model.Rs;
            Bank = model.Bank;
            Ks = model.Ks;
            Bik = model.Bik;
            Okpo = model.Okpo;
            Okved = model.Okved;
            //ManagerName = model.ManagerName;
            //ManagerNameDat = model.ManagerNameDat;
            //ManagerPosition = model.ManagerPosition;
            //ManagerPositionDat = model.ManagerPositionDat;
            Site = model.Site;
            StateImages = model.StateImages;
            //ManagerShortName = model.ManagerShortName;
            Director = model.Director;
        }

        public static IEnumerable<Organization> GetSelectionList()
        {
            Uri uri = new Uri(String.Format("{0}/Organization/GetList", OdataServiceUri));
            string jsonString = GetJson(uri);

            var model = JsonConvert.DeserializeObject<IEnumerable<Organization>>(jsonString);

            return model;
        }

        public static IEnumerable<Organization> GetList()
        {
            Uri uri = new Uri(String.Format("{0}/Organization/GetList", OdataServiceUri));
            string jsonString = GetJson(uri);

            var model = JsonConvert.DeserializeObject<IEnumerable<Organization>>(jsonString);

            return model;
        }

        public bool Save(out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/Organization/Save", OdataServiceUri));
            string json = JsonConvert.SerializeObject(this);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }

        public static bool Delete(int id, out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/Organization/Close?id={1}", OdataServiceUri, id));
            string json = String.Empty;//String.Format("{{\"id\":{0}}}",id);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }
    }
}