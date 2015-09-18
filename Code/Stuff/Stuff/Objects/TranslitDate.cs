using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Stuff.Objects
{
    public class TranslitDate
    {
        Dictionary<int, string> dict = new Dictionary<int, string>();

        public TranslitDate()
        {
            Propare();
        }

        public string GetMonthName(int month)
        {
            foreach (KeyValuePair<int, string> pair in dict)
            {
                if (month == pair.Key) return pair.Value;
            }

            return null;
        }

        private void Propare()
        {
            dict.Add(1, "январь");
            dict.Add(2, "февраль");
            dict.Add(3, "март");
            dict.Add(4, "апрель");
            dict.Add(5, "май");
            dict.Add(6, "июнь");
            dict.Add(7, "июль");
            dict.Add(8, "август");
            dict.Add(9, "сентябрь");
            dict.Add(10, "октябрь");
            dict.Add(11, "ноябрь");
            dict.Add(12, "декабрь");
        }
    }
}
