using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataProvider.Models.Service;
using Objects;

namespace DataProvider.Controllers.Service
{
    public class ClaimStateController : BaseApiController
    {
        public IEnumerable<ClaimState> GetFilterList()
        {
            return ClaimState.GetFilterList();
        }

        public IEnumerable<ClaimStateGroup> GetGroupFilterList()
        {
            return ClaimStateGroup.GetFilterList();
        }
    }
}
