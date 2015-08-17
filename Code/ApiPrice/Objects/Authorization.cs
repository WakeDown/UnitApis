using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Objects
{
    public class AuthorizeAdAttribute : AuthorizeAttribute
    {
        public AdGroup[] Groups { get; set; }

        protected bool AuthorizeCore(HttpContextBase httpContext)
        {
            //if (base.AuthorizeCore(httpContext))
            //{
                /* Return true immediately if the authorization is not 
                locked down to any particular AD group */
                //if (String.IsNullOrEmpty(Group))
                if (!Groups.Any())
                    return true;

                // Get the AD groups
                //var groups = Groups.Split(',').ToList<string>();

                // Verify that the user is in the given AD group (if any)
                var context = new PrincipalContext(ContextType.Domain);
                var userPrincipal = UserPrincipal.FindByIdentity(context,
                                                     IdentityType.SamAccountName,
                                                     httpContext.User.Identity.Name);
                //Если юзер Суперадмин
                if (userPrincipal.IsMemberOf(context, IdentityType.Sid, AdUserGroup.GetSidByAdGroup(AdGroup.SuperAdmin))){return true;}

                foreach (AdGroup group in Groups)
                {
                    if (userPrincipal.IsMemberOf(context, IdentityType.Sid, AdUserGroup.GetSidByAdGroup(group)))
                    {
                        return true;
                    }
                }
            //}
            return false;
        }
    }
}