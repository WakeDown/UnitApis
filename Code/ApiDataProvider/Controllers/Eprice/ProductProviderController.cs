using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataProvider.Models.Eprice;

namespace DataProvider.Controllers.Eprice
{
    public class ProductProviderController : ApiController
    {
        public ProductProvider Get(int id)
        {
            var model = new ProductProvider(id);
            return model;
        }

        public ProductProvider Get(string sysName)
        {
            var model = new ProductProvider(sysName);
            return model;
        }
    }
}
