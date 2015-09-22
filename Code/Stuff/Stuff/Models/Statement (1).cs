using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using Stuff.Models;

namespace Stuff.Models
{
    enum StatementType
    {
        FewHours,//несколько часов
        FewDays//несколько дней
    }

    public class Statement:Document
    {
        public Employee Employee { get; set; }
        public string Text { get; set; }
        public string Cause { get; set; }
        
        public List<Employee> Matchers { get; set; }

        private const string DefaultCause = "по семейным обстоятельствам";

        public Statement()
        {
            Name = String.Empty;
            Text = String.Empty;
            Cause = DefaultCause;
            Date = DateTime.Now;
        }

        //public Statement(int idEmployee)
        //{
        //    Employee = new Employee(idEmployee);
        //    Init();
        //}

        //public Statement(string sidEmployee)
        //{
        //    Employee = new Employee(sidEmployee);
        //    Init();
        //}

        protected void Configure(string empSid)
        {
            Employee = new Employee(empSid);
            Organization = new Organization(Employee.Organization.Id);
            Organization.Director = new Employee(Organization.Director.AdSid);
        }
    }
}