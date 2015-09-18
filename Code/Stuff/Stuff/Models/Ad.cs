using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Stuff.Objects;

namespace Stuff.Models
{
    public class Ad : DbModel
    {

        public static string GenEmailAddressByName(string surname, string name)
        {
            Uri uri = new Uri(String.Format("{0}/Ad/GetEmailAddressByName?surname={1}&name={2}", OdataServiceUri, surname, name));
            string jsonString = GetJson(uri);

            string email = JsonConvert.DeserializeObject<string>(jsonString);

            return email;
        }
    }
}