using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Objects
{
    public class DbModel
    {
        private IPrincipal _curUser;
        public IPrincipal CurUser
        {
            get { return _curUser; }
            set
            {
                _curUser = value;
                string sid = null;
                var wi = (WindowsIdentity)_curUser.Identity;
                if (wi.User != null)
                {
                    CurUserAdSid = wi.User.Value;
                }
                CurUserAdSid = String.Empty;
            }
        }
        public string CurUserAdSid { get; set; }
    }
}