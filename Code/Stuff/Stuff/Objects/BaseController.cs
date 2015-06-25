using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Stuff.Models;

namespace Stuff.Objects
{
    public class BaseController:Controller
    {
        [NonAction]
        public AdUser GetCurUser()
        {
            AdUser user = new AdUser();
            try
            {
                var wi = (WindowsIdentity) base.User.Identity;
                if (wi.User != null)
                {
                    var domain = new PrincipalContext(ContextType.Domain);
                    string sid = wi.User.Value;
                    user.Sid = sid;
                    var login = wi.Name.Remove(0, wi.Name.IndexOf("\\", StringComparison.CurrentCulture) + 1);
                    user.Login = login;
                    var userPrincipal = UserPrincipal.FindByIdentity(domain, login);
                    if (userPrincipal != null)
                    {
                        var mail = userPrincipal.EmailAddress;
                        var name = userPrincipal.DisplayName;
                        user.Email = mail;
                        user.FullName = name;
                        user.AdGroups = new List<AdGroup>();
                        var wp = new WindowsPrincipal(wi);
                        foreach (var role in AdUserGroup.GetList())
                        {
                            var grpSid = new SecurityIdentifier(role.Sid);
                            if (wp.IsInRole(grpSid))
                            {
                                user.AdGroups.Add(role.Group);
                            }
                        }
                    }
                }
            }
            catch
            {
                
            }

            return user;
        }

        protected AdUser DisplayCurUser()
        {
            AdUser curUser = GetCurUser();
            if (curUser == new AdUser()) RedirectToAction("AccessDenied", "Error");
            ViewBag.CurUser = curUser;
            return curUser;
        }
    }
}