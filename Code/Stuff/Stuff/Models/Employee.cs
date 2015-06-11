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
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public City City { get; set; }
        public byte[] Photo { get; set; }
        public DateTime? DateCame { get; set; }

        public AdGroup[] AdGroups { get; set; }

        public Employee() { }

        public Employee(int id)
        {
            //Id = id;
            //AdSid = "12213ohjboi5345oijbnio";
            //Manager = new Employee() { DisplayName = "Медведевских А.А." };
            //Surname = "Рехов";
            //Name = "Антон";
            //Patronymic = "Игоревич";
            //FullName = "Рехов Антон Игоревич";
            //DisplayName = "Рехов А.И.";
            //Position = new Position() { Id = 1, Name = "Pos1" };
            //Organization = new Organization() { Id = 1, Name = "Org1" };
            //Email = "anton.rehov@unitgroup.ru";
            //WorkNum = "111";
            //MobilNum = "+79536001000";
            //Department = new Department() { Id = 1, Name = "Dep1" };
            //City = new City() { Id = 1, Name = "City1" };
            //DateCame = DateTime.Now;

            Uri uri = new Uri(String.Format("{0}/Employee/Get/{1}", OdataServiceUri, id));
            string jsonString = GetJson(uri);

            var emp = JsonConvert.DeserializeObject<Employee>(jsonString);
            this.Id = emp.Id;
            this.AdSid = emp.AdSid;
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
            DepartmentId = emp.Department.Id;
            Department = emp.Department;
            City = emp.City;
            Photo = emp.Photo;
            DateCame = emp.DateCame;
        }


        public void FillAdGroups()
        {
            
        }

        internal void Save()
        {
            Id = 1;
        }

        public static List<Employee> GetSelectionList()
        {
            return new List<Employee>(){new Employee(1)};
        }
    }
}