using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataProvider.Models.Eprice;
using Objects;

namespace DataProvider.Controllers.Eprice
{
    public class CatalogProductController : BaseApiController
    {
        public class PartNumValue 
        {
            public string PartNum { get; set; }
        }

        [HttpPost]
        public HttpResponseMessage GetMinPrice(PartNumValue partNumVlau)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);
            string sid = GetCurUser().Sid;
            try
            {
                //var treolanPrice = Treolan.GetPriceByPartNum(partNum);
                string priceStr = CatalogProduct.GetMinPrice(partNumVlau.PartNum).GetStr();
                //response.Content = new StringContent(String.Format("{{\"priceStr\": \"{0}\"}}", priceStr));
                response.Content = new StringContent(String.Format("{{\"Value\":\"{0}\"}}", priceStr));
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(String.Format("{{\"errorMessage\":\"{0}\"}}", ex.Message));

            }
            return response;
        }
    }
}
