using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace ApiPrice.Models
{
    public class Merlion
    {
        public static string Login = "MDC2909|IT";
        public static string Password = "123456789";

        static MerlionBase.MLPortClient Client = new MerlionBase.MLPortClient();

        public Merlion()
        {
            if (Client.State != CommunicationState.Opened)Client.Open();
        }

        public static string GetPriceByPartNum(string partNum)
        {
            string price = String.Empty;

            //Client.getItems()

            return price;
        }
    }
}