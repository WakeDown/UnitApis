using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataProvider.Models.SpeCalc
{
    public partial class Settings
    {
        //public static string[] Emails4Test = { "anton.rehov@unitgroup.ru" };
        //public static string[] Emails4SysError = { "anton.rehov@unitgroup.ru" };
        //public static string[] Emails4NewEmployee = { "anton.rehov@unitgroup.ru", "anton.ivonin@unitgroup.ru", "evgeniy.chetverikov@unitgroup.ru" };

        public class SpeCalc
        {
            public const string Url = "https://spec.unitgroup.ru";
            public const string DefaultMailFrom = "spec@un1t.group";
        }
        
    }
}