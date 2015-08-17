using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Stuff.Objects;

namespace StuffDelivery.Models
{
    public class Employee : DbModel
    {
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

        public static IEnumerable<string> GetHolidayWorkDeliveryRecipientList()
        {
            Uri uri = new Uri(String.Format("{0}/Employee/GetHolidayWorkDeliveryRecipientList", OdataServiceUri));
            string jsonString = GetJson(uri);
            var email = JsonConvert.DeserializeObject<IEnumerable<string>>(jsonString);
            return email;
        }

        public static IEnumerable<string> GetFullRecipientList(string citySysName=null)
        {
            Uri uri = new Uri(String.Format("{0}/Employee/GetFullRecipientList?citySysName={1}", OdataServiceUri, citySysName));
            string jsonString = GetJson(uri);
            var email = JsonConvert.DeserializeObject<IEnumerable<string>>(jsonString);
            return email;
        }
        public static IEnumerable<Employee> GetTodayBirthdayList()
        {
            Uri uri = new Uri(String.Format("{0}/Employee/GetTodayBirthdayList", OdataServiceUri));
            string jsonString = GetJson(uri);
            var emps = JsonConvert.DeserializeObject<IEnumerable<Employee>>(jsonString);
            return emps;
        }
        public static IEnumerable<Employee> GetNextMonthBirthdayList()
        {
            Uri uri = new Uri(String.Format("{0}/Employee/GetNextMonthBirthdayList", OdataServiceUri));
            string jsonString = GetJson(uri);
            var emps = JsonConvert.DeserializeObject<IEnumerable<Employee>>(jsonString);
            return emps;
        }
        public static IEnumerable<Employee> GetNewbieList()
        {
            Uri uri = new Uri(String.Format("{0}/Employee/GetNewbieList", OdataServiceUri));
            string jsonString = GetJson(uri);
            var emps = JsonConvert.DeserializeObject<IEnumerable<Employee>>(jsonString);
            return emps;
        }

        public static bool SetNewbieDeliverySend(out ResponseMessage responseMessage,params Employee[] emps)
        {
            Uri uri = new Uri(String.Format("{0}/Employee/SetDeliverySend", OdataServiceUri));
            string json = JsonConvert.SerializeObject(emps);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }
    }
}