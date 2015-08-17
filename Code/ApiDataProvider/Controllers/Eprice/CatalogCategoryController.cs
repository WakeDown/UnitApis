using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataProvider.Helpers;
using DataProvider.Models.Eprice;
using DataProvider.Objects;
using Objects;

namespace DataProvider.Controllers.Eprice
{
    public class CatalogCategoryController : BaseApiController
    {
        
        public HttpResponseMessage SaveList(Catalog catalog)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                //if (catalog != null)
                //{
                    if (catalog.Categories.Any())
                    {
                        foreach (CatalogCategory cat in catalog.Categories)
                        {
                            cat.Provider = catalog.Provider;
                            cat.Save();
                        }
                    }
                //}
                //response.Content = new StringContent(String.Format("{{\"id\":{0}}}", dep.Id));
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));
                

            }
            return response;
        }

        
        public HttpResponseMessage Save(CatalogCategory cat)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                cat.Save();
                //response.Content = new StringContent(String.Format("{{\"id\":{0}}}", dep.Id));
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }
    }
}
