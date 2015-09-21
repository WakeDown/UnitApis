using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataProvider.Models.SpeCalc
{
    public partial class Settings
    {
        public static string[] Emails4Test = { "alexandr.romanov@unitgroup.ru" };
        public static string[] Emails4SysError = { "anton.rehov@unitgroup.ru" };
        public static string[] Emails4NewEmployee = { "anton.rehov@unitgroup.ru", "anton.ivonin@unitgroup.ru", "evgeniy.chetverikov@unitgroup.ru" };

        public class SpeCalc
        {
            public const string Url = "http://uiis-1.un1t.group:10112";
            public const string DefaultMailFrom = "SpeCalc@un1t.group";
        }
        
    }
}