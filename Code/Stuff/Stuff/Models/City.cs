using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stuff.Models
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static IEnumerable<City> GetSelectionList()
        {
            var lst = new List<City>();

            lst.Add(new City() { Id = 1, Name = "City1" });
            lst.Add(new City() { Id = 2, Name = "City2" });
            lst.Add(new City() { Id = 3, Name = "City3" });

            return lst;
        }
    }
}