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
    public class Employee : DbModel
    {
        //protected static Uri OdataServiceEmployeeUri = new Uri(String.Format("{0}/Employee", OdataServiceUri));

        public int Id { get; set; }
        public string AdSid { get; set; }
        public Employee Manager { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string FullName { get; set; }
        public string DisplayName { get; set; }
        public Position Position { get; set; }
        public Organization Organization { get; set; }
        public string Email { get; set; }
        public string WorkNum { get; set; }
        public string MobilNum { get; set; }
        public EmpState EmpState { get; set; }
        //public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public City City { get; set; }
        public byte[] Photo { get; set; }
        public DateTime? DateCame { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool IsChief { get; set; }
        public bool Male { get; set; }
        public bool HasAdAccount { get; set; }
        public Employee Creator { get; set; }
        /// <summary>
        /// Официальная должность
        /// </summary>
        public Position PositionOrg { get; set; }

        public AdGroup[] AdGroups { get; set; }

        public Employee() { }

        public Employee(int id)
        {
            Uri uri = new Uri(String.Format("{0}/Employee/Get?id={1}", OdataServiceUri, id));
            string jsonString = GetJson(uri);

            Employee emp = JsonConvert.DeserializeObject<Employee>(jsonString);
            FillSelf(emp);
        }

        private void FillSelf(Employee emp)
        {
            Id = emp.Id;
            AdSid = emp.AdSid;
            Manager = emp.Manager;
            Surname = emp.Surname;
            Name = emp.Name;
            Patronymic = emp.Patronymic;
            FullName = emp.FullName;
            DisplayName = emp.DisplayName;
            Position = emp.Position;
            Organization = emp.Organization;
            Email = emp.Email;
            WorkNum = emp.WorkNum;
            MobilNum = emp.MobilNum;
            EmpState = emp.EmpState;
            Department = emp.Department;
            City = emp.City;
            Photo = emp.Photo;
            DateCame = emp.DateCame;
            BirthDate = emp.BirthDate;
            IsChief = emp.IsChief;
            Male = emp.Male;
            PositionOrg = emp.PositionOrg;
            HasAdAccount = emp.HasAdAccount;
            Creator = emp.Creator;
        }

        public void FillAdGroups()
        {
            
        }

        public bool Save(out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/Employee/Save", OdataServiceUri));
            string json = JsonConvert.SerializeObject(this);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }

        public static bool Delete(int id, out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/Employee/Close?id={1}", OdataServiceUri, id));
            string json = String.Empty;//String.Format("{{\"id\":{0}}}",id);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }

        public static IEnumerable<Employee> GetList()
        {
            Uri uri = new Uri(String.Format("{0}/Employee/GetList", OdataServiceUri));
            string jsonString = GetJson(uri);

            var emps = JsonConvert.DeserializeObject<IEnumerable<Employee>>(jsonString);

            return emps;
        }

        public static IEnumerable<Employee> GetSelectionList()
        {
            Uri uri = new Uri(String.Format("{0}/Employee/GetList", OdataServiceUri));
            string jsonString = GetJson(uri);

            var emps = JsonConvert.DeserializeObject<IEnumerable<Employee>>(jsonString);

            return emps;
        }

        ////public static byte[] GetPhoto(int id)
        ////{
        ////    Uri uri = new Uri(String.Format("{0}/Employee/GetPhoto?id={1}", OdataServiceUri, id));
        ////    return GetImage
        ////}
    }
}