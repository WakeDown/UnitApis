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

        public bool Is(params AdGroup[] groups)
        {
           return AdHelper.UserIs(User, groups);
        }

        public bool HasAccess(params AdGroup[] groups)
        {
            return AdHelper.UserInGroup(User, groups);
        }
    }
}