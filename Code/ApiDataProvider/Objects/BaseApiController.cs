using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using DataProvider.Objects;

namespace Objects
{
    public class BaseApiController:ApiController
    {

        //private IPrincipal _curUser;
        //public IPrincipal CurUser
        //{
        //    get { return _curUser; }
        //    set
        //    {
        //        _curUser = value;
        //        string sid = null;
        //        var wi = (WindowsIdentity)_curUser.Identity;
        //        if (wi.User != null)
        //        {
        //            CurUserAdSid = wi.User.Value;
        //        }
        //        CurUserAdSid = String.Empty;
        //    }
        //}
        //public string CurUserAdSid { get; set; }

        public BaseApiController()
        {

            //SetCurUser();
        }

        protected AdUser GetCurUser()
        {
            AdUser curUser = new AdUser();
            curUser.User =  base.RequestContext.Principal;
            
            string sid = null;
            var wi = (WindowsIdentity)RequestContext.Principal.Identity;
            if (wi.User != null)
            {
                var domain = new PrincipalContext(ContextType.Domain);
                curUser.Sid = wi.User.Value;
            }
            return curUser;
        }

        //protected string GetCurUserSid()
        //{
        //    string sid = null;
        //    var wi = (WindowsIdentity)base.RequestContext.Principal.Identity;
        //    if (wi.User != null)
        //    {
        //        var domain = new PrincipalContext(ContextType.Domain);
        //        sid = wi.User.Value;
        //    }

        //    return sid;
        //}
    }
}