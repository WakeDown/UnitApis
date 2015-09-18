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
    public class LanguageController : BaseApiController
    {
        public IEnumerable<Language> GetList()
        {
            return Language.GetList();
        }
    }
}
