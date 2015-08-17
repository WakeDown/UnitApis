using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataProvider.Objects;

namespace DataProvider.Models.Service
{
    public class Vendor:DbModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}