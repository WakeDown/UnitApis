using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Stuff.Models
{
    public class StatementRestFewHours : Statement
    {
        public DateTime HourStart { get; set; }
        public DateTime HourEnd { get; set; }
        public int HoursCount { get; set; }
        public DateTime DateRest { get; set; }

        public string SidEmployee { get; set; }

        public StatementRestFewHours(){}

        public StatementRestFewHours(string sidEmployee)
        {
            HoursCount = 2;
            SidEmployee = sidEmployee;
            DateRest = DateTime.Now;
            HourStart = new DateTime(2000, 1,1,12,0,0);
            
        }

        //public StatementRestFewHours(string sidEmployee, int hoursCount, DateTime hourStart, DateTime dateRest, string cause = null)
        //{
        //    Init(hoursCount, hourStart, dateRest, cause);
        //}

        //public StatementRestFewHours(int idEmployee, int hoursCount, DateTime hourStart, DateTime dateRest, string cause = null)
        //{
        //    Init(hoursCount, hourStart, dateRest, cause);
        //}
        //int hoursCount, DateTime hourStart, DateTime dateRest, string cause = null
        public void Configure()
        {
            base.Configure(SidEmployee);
            Organization.Director = new Employee().GetDirector();
            Matchers = new List<Employee>() {Employee.Manager};
            var depDir = new Employee().GetDepartmentDirector();
            if (Employee.Manager.AdSid != depDir.AdSid) Matchers.Add(depDir);
            if (HoursCount <= 0) throw new ArgumentException("Количество часов должно быть больше 0");
            Name = "Служебная записка";
            HoursCount = HoursCount;
            HourEnd = HourStart.AddHours(HoursCount);

            //Склоняем часы
            string hoursStr = "часа";
            int hDig = HoursCount % 10;
            if (hDig == 1) hoursStr = "час";
            if (hDig > 1 && hDig < 5) hoursStr = "часа";
            if (hDig > 4 && hDig < 21) hoursStr = "часов";

            Text =
                String.Format(
                    "Прошу предоставить мне {0} {5} рабочего времени с {1:HH:mm} по {2:HH:mm} {3:dd.MM.yyyy} года за свой счет {4}.",
                    HoursCount, HourStart, HourEnd, DateRest, Cause, hoursStr);
        }
    }
}