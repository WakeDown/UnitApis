using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using DataProvider.Models.Stuff;

namespace DataProvider.Controllers.Stuff
{
    public class EmpStateController : ApiController
    {
        [EnableQuery]
        public IQueryable<EmpState> GetList()
        {
            return new EnumerableQuery<EmpState>(EmpState.GetList());
        }

        public EmpState Get(int id)
        {
            var model = new EmpState(id);
            return model;
        }
    }
}
