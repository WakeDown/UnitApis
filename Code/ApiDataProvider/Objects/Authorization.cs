using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using DataProvider.Helpers;

//using System.Web.Mvc;

namespace DataProvider.Objects
{
    public class AuthorizeAdAttribute : AuthorizeAttribute//AuthorizeAttribute
    {
        public AdGroup[] Groups { get; set; }

        public AuthorizeAdAttribute() { }

        public AuthorizeAdAttribute(params AdGroup[] groups)
        {
            Groups = groups;
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (base.IsAuthorized(actionContext))
            {
                if (Groups == null || !Groups.Any())return true;
                //var context = new PrincipalContext(ContextType.Domain);
                //var userPrincipal = UserPrincipal.FindByIdentity(context,
                //                                     IdentityType.SamAccountName,
                //                                     actionContext.ControllerContext.RequestContext.Principal.Identity.Name);

                return AdHelper.UserInGroup(actionContext.ControllerContext.RequestContext.Principal, Groups);

                //Если юзер Суперадмин
                //if (userPrincipal.IsMemberOf(context, IdentityType.Sid, AdUserGroup.GetSidByAdGroup(AdGroup.SuperAdmin))) { return true; }

                //foreach (AdGroup group in Groups)
                //{
                //    if (userPrincipal.IsMemberOf(context, IdentityType.Sid, AdUserGroup.GetSidByAdGroup(group)))
                //    {
                //        return true;
                //    }
                //}
            }
            return false;
        }

        ////protected override bool AuthorizeCore(HttpContextBase httpContext)
        ////{
        ////    if (base.AuthorizeCore(httpContext))
        ////    {
        ////        /* Return true immediately if the authorization is not 
        ////        locked down to any particular AD group */
        ////        //if (String.IsNullOrEmpty(Group))
        ////        if (!Groups.Any())
        ////            return true;

        ////        // Get the AD groups
        ////        //var groups = Groups.Split(',').ToList<string>();

        ////        // Verify that the user is in the given AD group (if any)
        ////        var context = new PrincipalContext(ContextType.Domain);
        ////        var userPrincipal = UserPrincipal.FindByIdentity(context,
        ////                                             IdentityType.SamAccountName,
        ////                                             httpContext.User.Identity.Name);
        ////        //Если юзер Суперадмин
        ////        if (userPrincipal.IsMemberOf(context, IdentityType.Sid, AdUserGroup.GetSidByAdGroup(AdGroup.SuperAdmin))){return true;}

        ////        foreach (AdGroup group in Groups)
        ////        {
        ////            if (userPrincipal.IsMemberOf(context, IdentityType.Sid, AdUserGroup.GetSidByAdGroup(group)))
        ////            {
        ////                return true;
        ////            }
        ////        }
        ////    }
        ////    return false;
        ////}
    }
}