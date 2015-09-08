using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stuff.Models
{
    public class StatementRest:Statement
    {
        public int DaysCount { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string SidEmployee { get; set; }

        public StatementRest()
        {
            DateStart = DateTime.Now.AddDays(7);
            DaysCount = 14;
            //DateEnd = DateStart.AddDays(DaysCount);
        }

        public StatementRest(string sidEmployee):this()
        {
            SidEmployee = sidEmployee;
        }

        public void Configure()
        {

            base.Configure(SidEmployee);
            SetMatchersOficial(SidEmployee);
            DateEnd = DateStart.AddDays(DaysCount);
            if (DateEnd <= DateStart) throw new ArgumentException("Дата окончания должна быть больше даты начала");
            if (DaysCount <= 0) throw new ArgumentException("Количество дней должно быть больше 0");
            DateEnd = DateStart.AddDays(DaysCount - 1);

            Name = "З А Я В Л Е Н И Е";
            Text = $"Прошу предоставить мне очередной ежегодный отпуск в количестве {DaysCount} календарных дней с {DateStart:dd.MM.yyyy} г. по {DateEnd:dd.MM.yyyy} г. включительно.";
        }
    }
}