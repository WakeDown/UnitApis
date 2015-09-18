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

        protected void Configure(string empSid, string organizationSysName = null)
        {
            Employee = new Employee(empSid);
            Employee.Position = new Position(Employee.Position.Id);
            Employee.PositionOrg = new Position(Employee.PositionOrg.Id);
            if (String.IsNullOrEmpty(organizationSysName))
            {
                Organization = new Organization(Employee.Organization.Id);
            }
            else
            {
                Organization = new Organization(organizationSysName);
            }
            //Organization.Name = Organization.Name.Replace("«", "\"").Replace("»", "\"");
            Organization.Director = new Employee(Organization.Director.Id);
            Organization.Director.Position = new Position(Organization.Director.Position.Id);
            Organization.Director.PositionOrg = new Position(Organization.Director.PositionOrg.Id);
        }

        protected void SetMatchersOficial(string empSid)
        {
            Matchers = new List<Employee>() { Employee.Manager };
            var depDir = new Employee().GetDepartmentDirector(empSid);
            if (depDir != null)
            {
                if (Employee.Manager.Id != depDir.Id) Matchers.Add(depDir);
            }
            else
            {
                depDir = new Employee();
            }
            var dir = new Employee().GetDirector();
            if (dir != null)
            {
                if (Employee.Manager.Id != dir.Id && depDir.Id != dir.Id) Matchers.Add(dir);
            }
        }
    }
}