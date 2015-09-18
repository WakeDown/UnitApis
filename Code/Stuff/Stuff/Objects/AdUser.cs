using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Stuff.Helpers;

namespace Stuff.Objects
{
    public class AdUser
    {
        public string Sid { get; set; }
        public string Login { get; set; }
        public string FullName{get; set; }
        public string Email { get; set; }

        public string DisplayName
        {
            get { return MainHelper.ShortName(FullName); }
        }

        //public List<AdGroup> AdGroups { get; set; }
        

        public bool UserCanEdit()
        {
            if (String.IsNullOrEmpty(Sid)) return false;
            return HasAccess(AdGroup.SuperAdmin, AdGroup.PersonalManager);
        }

        public bool UserIsAdmin()
        {
            if (String.IsNullOrEmpty(Sid)) return false;
            return HasAccess(AdGroup.SuperAdmin);
        }

        public bool UserIsPersonalManager()
        {
            if (String.IsNullOrEmpty(Sid)) return false;
            return HasAccess(AdGroup.PersonalManager, AdGroup.SuperAdmin);
        }

        public bool IsSystemUser()
        {
            if (String.IsNullOrEmpty(Sid)) return false;
            return HasAccess(AdGroup.SystemUser, AdGroup.SuperAdmin);
        }

        public bool Is(params AdGroup[] groups)
        {
            bool result = false;
            if (String.IsNullOrEmpty(Sid)) return false;
            result = AdHelper.UserInGroup(Sid, groups);
            return result;
        }

        public bool HasAccess(params AdGroup[] groups)
        {
            bool result = false;
            if (String.IsNullOrEmpty(Sid)) return false;
            if (AdHelper.UserInGroup(Sid, AdGroup.SuperAdmin)) return true;
            result = AdHelper.UserInGroup(Sid, groups);
            return result;
        }
    }
}