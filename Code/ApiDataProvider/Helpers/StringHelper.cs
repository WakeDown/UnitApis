using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataProvider.Helpers
{
    public class StringHelper
    {
        public static string Trim(string obj)
        {
            if (String.IsNullOrEmpty(obj)) return obj;
            else return obj.Trim();
        }
    }
}