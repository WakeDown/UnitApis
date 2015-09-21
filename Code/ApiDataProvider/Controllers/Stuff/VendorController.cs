using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using DataProvider.Models.Stuff;
using DataProvider.Objects;
using Objects;

namespace DataProvider.Controllers.Stuff
{
    public class VendorController : BaseApiController
    {
        public IQueryable<Vendor> GetList()
        {
            return new EnumerableQuery<Vendor>(Vendor.GetList());
        }

        public Vendor Get(int id)
        {
            var model = new Vendor(id);
            return model;
        }
        
        public HttpResponseMessage Save(Vendor model)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                model.CurUserAdSid = GetCurUser().Sid;
                model.Save();
                response.Content = new StringContent(String.Format("{{\"id\":{0}}}", model.Id));
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(String.Format("{{\"errorMessage\":\"{0}\"}}", ex.Message));

            }
            return response;
        }
        [AuthorizeAd(Groups = new[] { AdGroup.SpeCalcKontroler })]
        public HttpResponseMessage Close(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);
            var deleter = GetCurUser().Sid;
            try
            {
                Vendor.Close(id, deleter);
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
