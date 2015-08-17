using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataProvider.Objects;

namespace DataProvider.Models.Eprice
{
    public class Catalog : DbModel
    {
        public IEnumerable<CatalogCategory> Categories { get; set; }
        public ProductProvider Provider { get; set; }

        public void Save()
        {

        }
    }
}