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
        public Position PositionOrg { get; set; }
        public string ExpirenceString { get; set; }
        public string FullNameDat { get; set; }
        public string FullNameRod { get; set; }
        public string ShortNameDat { get; set; }
        public string ShortNameRod { get; set; }
        public int? IdBudget { get; set; }

        public AdGroup[] AdGroups { get; set; }

        public Employee()
        {
            DateCame = DateTime.Now;
        }

        public Employee(int id)
        {
            Uri uri = new Uri(String.Format("{0}/Employee/Get?id={1}", OdataServiceUri, id));
            string jsonString = GetJson(uri);

            Employee emp = JsonConvert.DeserializeObject<Employee>(jsonString);
            FillSelf(emp);
        }

        public Employee(string sid)
        {
            Uri uri = new Uri(String.Format("{0}/Employee/Get?adSid={1}", OdataServiceUri, sid));
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
            ExpirenceString = emp.ExpirenceString;
            FullNameDat = emp.FullNameDat;
            FullNameRod = emp.FullNameRod;
            ShortNameDat = emp.ShortNameDat;
            ShortNameRod = emp.ShortNameRod;
            IdBudget = emp.IdBudget;
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
        public static bool SetStateDecree(int id, out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/Employee/SetStateDecree?id={1}", OdataServiceUri, id));
            string json = String.Empty;//String.Format("{{\"id\":{0}}}",id);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }
        public static bool SetStateFired(int id, out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/Employee/SetStateFired?id={1}", OdataServiceUri, id));
            string json = String.Empty;//String.Format("{{\"id\":{0}}}",id);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }
        public static bool SetStateStuff(int id, out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/Employee/SetStateStuff?id={1}", OdataServiceUri, id));
            string json = String.Empty;//String.Format("{{\"id\":{0}}}",id);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }
        public static IEnumerable<Employee> GetList(int? idCity = null, int? idDepartment = null, bool showHidden = true)
        {
            Uri uri = new Uri(String.Format("{0}/Employee/GetList?idCity={1}&idDepartment={2}&showHidden={3}", OdataServiceUri, idCity, idDepartment, showHidden));
            string jsonString = GetJson(uri);

            var emps = JsonConvert.DeserializeObject<IEnumerable<Employee>>(jsonString);

            return emps;
        }

        public static IEnumerable<Employee> GetFiredList()
        {
            Uri uri = new Uri(String.Format("{0}/Employee/GetFiredList", OdataServiceUri));
            string jsonString = GetJson(uri);

            var emps = JsonConvert.DeserializeObject<IEnumerable<Employee>>(jsonString);

            return emps;
        }

        public static IEnumerable<Employee> GetDecreeList()
        {
            Uri uri = new Uri(String.Format("{0}/Employee/GetDecreeList", OdataServiceUri));
            string jsonString = GetJson(uri);

            var emps = JsonConvert.DeserializeObject<IEnumerable<Employee>>(jsonString);

            return emps;
        }

        public static IEnumerable<Employee> GetNewbieList()
        {
            Uri uri = new Uri(String.Format("{0}/Employee/GetStNewbieList", OdataServiceUri));
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

        public Employee GetDirector()
        {
            Uri uri = new Uri(String.Format("{0}/Employee/GetDirector", OdataServiceUri));
            string jsonString = GetJson(uri);
            Employee emp = JsonConvert.DeserializeObject<Employee>(jsonString);
            FillSelf(emp);
            return emp;
        }

        public Employee GetDepartmentDirector(string employeeSid)
        {
            Uri uri = new Uri(String.Format("{0}/Employee/GetDepartmentDirector?employeeSid={1}", OdataServiceUri, employeeSid));
            string jsonString = GetJson(uri);
            Employee emp = JsonConvert.DeserializeObject<Employee>(jsonString);
            FillSelf(emp);
            return emp;
        }

        ////public static byte[] GetPhoto(int id)
        ////{
        ////    Uri uri = new Uri(String.Format("{0}/Employee/GetPhoto?id={1}", OdataServiceUri, id));
        ////    return GetImage
        ////}

        public static string GetServerSid()
        {
            Uri uri = new Uri(String.Format("{0}/Employee/GetCurrentUserSid", OdataServiceUri));
            return GetJson(uri);
        }

        public static string GetServerUserName()
        {
            Uri uri = new Uri(String.Format("{0}/Employee/GetCurrentUserName", OdataServiceUri));
            return GetJson(uri);
        }
    }
}