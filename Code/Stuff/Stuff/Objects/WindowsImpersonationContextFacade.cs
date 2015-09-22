using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Web;

namespace Stuff.Objects
{
    public class WindowsImpersonationContextFacade : IDisposable
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain,
                                            String lpszPassword, int dwLogonType, int dwLogonProvider,
                                            ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);

        private const int LOGON32_PROVIDER_DEFAULT = 0;
        private const int LOGON32_LOGON_INTERACTIVE = 2;

        private string m_Domain;
        private string m_Password;
        private string m_Username;
        private IntPtr m_Token;

        private WindowsImpersonationContext m_Context = null;

        protected bool IsInContext
        {
            get { return m_Context != null; }
        }

        public WindowsImpersonationContextFacade(string domain, string username, string password)
        {
            m_Domain = domain;
            m_Username = username;
            m_Password = password;
            Enter();
        }

        public WindowsImpersonationContextFacade(NetworkCredential credentials)
        {
            m_Domain = credentials.Domain;
            m_Username = credentials.UserName;
            m_Password = credentials.Password;
            Enter();
        }

        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        private void Enter()
        {
            if (this.IsInContext) return;
            m_Token = IntPtr.Zero;
            bool logonSuccessfull = LogonUser(
                m_Username,
                m_Domain,
                m_Password,
                LOGON32_LOGON_INTERACTIVE,
                LOGON32_PROVIDER_DEFAULT,
                ref m_Token);
            if (logonSuccessfull == false)
            {
                int error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }
            WindowsIdentity identity = new WindowsIdentity(m_Token);
            m_Context = identity.Impersonate();
        }

        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        private void Leave()
        {
            if (this.IsInContext == false) return;
            m_Context.Undo();

            if (m_Token != IntPtr.Zero) CloseHandle(m_Token);
            m_Context = null;
        }

        public void Dispose()
        {
            Leave();
        }
    }
}