using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stuff.Models
{
    public class Organization
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static IEnumerable<Organization> GetList()
        {
            var lst = new List<Organization>();

            lst.Add(new Organization() { Id = 1, Name = "Org1" });
            lst.Add(new Organization() { Id = 2, Name = "Org2" });
            lst.Add(new Organization() { Id = 3, Name = "Org3" });

            return lst;
        }
    }
}