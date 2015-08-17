using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using DataProvider.Models.SpeCalc;

namespace DataProvider.Controllers.SpeCalc
{
    public class QueStateController : ApiController
    {
        [EnableQuery]
        public IQueryable<QueState> GetList()
        {
            return new EnumerableQuery<QueState>(QueState.GetList());
        }
    }
}
