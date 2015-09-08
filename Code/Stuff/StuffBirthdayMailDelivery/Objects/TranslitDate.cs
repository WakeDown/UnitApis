using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace StuffDelivery.Objects
{
    class TranslitDate
    {
        static Dictionary<int, string>  dict = new Dictionary<int, string>();

        public TranslitDate()
        {
            
        }

        public static string GetMonthNamePredl(int month)
        {
            ProparePredl();

            foreach (KeyValuePair<int, string> pair in dict)
            {
                if (month == pair.Key) return pair.Value;
            }

            return null;
        }

        public static string GetMonthNameImenit(int month)
        {
            PropareImenit();

            foreach (KeyValuePair<int, string> pair in dict)
            {
                if (month == pair.Key) return pair.Value;
            }

            return null;
        }
        

        private static void PropareImenit()
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

        private static void ProparePredl()
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
