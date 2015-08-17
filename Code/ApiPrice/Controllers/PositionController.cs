using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ApiPrice.Models;
using Objects;

namespace ApiPrice.Controllers
{
    public class PositionController : BaseApiController
    {
        [HttpGet]
        public HttpResponseMessage GetPriceByPartNum(string partNum)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);
            string sid = GetCurUser().Sid;
            try
            {
                var treolanPrice = Treolan.GetPriceByPartNum(partNum);

                response.Content = new StringContent(String.Format("{{\"price: \"{0}\"}}", treolanPrice));
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
