using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace StuffBirthdayMailDelivery
{
    class TranslitDate
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
            dict.Add(1, "январе");
            dict.Add(2, "феврале");
            dict.Add(3, "марте");
            dict.Add(4, "апреле");
            dict.Add(5, "мае");
            dict.Add(6, "июне");
            dict.Add(7, "июле");
            dict.Add(8, "августе");
            dict.Add(9, "сентябре");
            dict.Add(10, "октябре");
            dict.Add(11, "ноябре");
            dict.Add(12, "декабре");
        }
    }
}
