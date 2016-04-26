using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataProvider.Helpers;
using DataProvider.Models.Stuff;
using DataProvider.Objects;
using Objects;

namespace DataProvider.Controllers
{
    public class UsersController : BaseApiController
    {
        public IEnumerable<string> GetUserGroups()
        {
            List<string> result = new List<string>();
            foreach (AdGroup grp in GetCurUser().AdGroups)
            {
                result.Add(AdUserGroup.GetSidByAdGroup(grp));
            }

            return result;
        }

        public IEnumerable<KeyValuePair<string, string>> GetUserListByGroupSid(string sid)
        {
            return AdHelper.GetUserListByAdGroup(sid);
        }

        public EmployeeSm GetUser(string sid)
        {
            return new EmployeeSm(sid);
        }
    }
}
