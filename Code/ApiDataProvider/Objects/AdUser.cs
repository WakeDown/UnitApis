using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using DataProvider.Helpers;
using DocumentFormat.OpenXml.Spreadsheet;

namespace DataProvider.Objects
{
    public class AdUser
    {
        public IPrincipal User { get; set; }
        public string Sid { get; set; }

        public List<AdGroup> AdGroups { get; set; }

        public bool Is(params AdGroup[] groups)
        {
            return groups.Select(grp => AdGroups.Contains(grp)).Any(res => res);
            //return AdHelper.UserIs(User, groups);
        }

        public bool HasAccess(params AdGroup[] groups)
        {
            if (AdGroups == null || !AdGroups.Any()) return false;
            if (AdGroups.Contains(AdGroup.SuperAdmin)) return true;
            return groups.Select(grp => AdGroups.Contains(grp)).Any(res => res);
            //return AdHelper.UserInGroup(User, groups);
        }
    }
}