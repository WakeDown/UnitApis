using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataProvider.Models.Eprice
{
    public class Currency
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SysName { get; set; }
        public string ProviderName { get; set; }
    }
}