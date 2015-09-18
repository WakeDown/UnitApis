using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stuff.Models
{
    public class StatementRestFewDays:Statement
    {
        public int DaysCount { get; set; }
        public DateTime DateStart { get; set; }
        DateTime DateEnd { get; set; }
        //public Employee Employee { get; set; }
        public string SidEmployee { get; set; }

        public StatementRestFewDays(){}

        public StatementRestFewDays(string sidEmployee)
        {
            DaysCount = 1;
            SidEmployee = sidEmployee;
            DateStart = DateTime.Now;
            
        }

        public void Configure()
        {

            base.Configure(SidEmployee);
            SetMatchersOficial(SidEmployee);
            //Matchers = new List<Employee>() {Employee.Manager};
            //var depDir = new Employee().GetDepartmentDirector(SidEmployee);
            //if (depDir != null)
            //{
            //    if (Employee.Manager.Id != depDir.Id) Matchers.Add(depDir);
            //}

            //var dir = new Employee().GetDirector();
            //if (dir != null)
            //{
            //    if (Employee.Manager.Id != dir.Id && depDir.Id != dir.Id) Matchers.Add(dir);
            //}
            if (DaysCount <= 0) throw new ArgumentException("Количество дней должно быть больше 0");
            DateEnd = DateStart.AddDays(DaysCount - 1);

            Name = "З А Я В Л Е Н И Е";

            if (DateEnd.Date == DateStart.Date)
            {
                Text =
                    String.Format(
                        "Прошу предоставить мне отпуск без сохранения заработной платы в количестве {0} календарных дней {1:dd.MM.yyyy} г., {2}.",
                        DaysCount, DateStart, Cause);
            }
            else
            {
                Text =
                    String.Format(
                        "Прошу предоставить мне отпуск без сохранения заработной платы в количестве {0} календарных дней с {1:dd.MM.yyyy} г. по {2:dd.MM.yyyy} г., включительно, {3}.",
                        DaysCount, DateStart, DateEnd, Cause);
            }

            //public StatementRestFewDays(int idEmployee, int daysCount, DateTime dateStart, string cause = null)
            //{
            //    if (daysCount <= 0) throw new ArgumentException("Количество дней должно быть больше 0");
            //    DaysCount = daysCount;
            //    DateStart = dateStart;
            //    if (!String.IsNullOrEmpty(cause))Cause = cause;
            //    DateEnd = DateStart.AddDays(DaysCount);

            //    Name = "З А Я В Л Е Н И Е";
            //    Text = String.Format("прошу предоставить мне отпуск без сохранения заработной платы в количестве {0} календарных дней с {1:dd.MM.yyyy} г. по {2:dd.MM.yyyy} г., включительно, {3}.", DaysCount, DateStart, DateEnd, Cause);
            //}
        }
    }
}