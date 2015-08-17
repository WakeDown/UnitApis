using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ApiPrice.Objects;

namespace ApiPrice.Models
{
    public class Treolan
    {
        public static string Login = "unitgroup_rai";
        public static string Password = "awklhy6w";
        static TreolanBase.WebServiceSoapPortClient ClientBase = new TreolanBase.WebServiceSoapPortClient();
        static TreolanProducts.B2BWebServiceSoapClient ClientProduct = new TreolanProducts.B2BWebServiceSoapClient();

        public static string GetPriceByPartNum(string partNum)
        {
            string price = String.Empty;
            string category = "";
            int criterion = 0;
            bool inArticul = false;
            bool inName = false;
            bool inMark = false;
            int showNc = 0;

            ClientBase.GenCatalog(ref Login, ref Password, ref category, ref partNum, ref criterion, ref inArticul,
                ref inName, ref inMark, ref showNc);

            return price;
        }
    }
}