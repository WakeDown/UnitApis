using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stuff.Objects
{
    public class AdUserRole
    {
        public AdRole Role { get; set; }
        public string Sid { get; set; }
        public string Name { get; set; }

        public AdUserRole(AdRole role, string sid)
        {
            Role = role;
            Sid = sid;
        }

        public static IEnumerable<AdUserRole> GetList()
        {
            var list = new List<AdUserRole>();
            list.Add(new AdUserRole(AdRole.PersonalManager, "S-1-5-21-1970802976-3466419101-4042325969-4030"));//Менеджер по персоналу
            return list;
        }
    }
}