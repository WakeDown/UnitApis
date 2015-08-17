using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Principal;
using System.Web;
using System.Web.SessionState;
using DataProvider.Models.Stuff;
using DataProvider.Objects;

namespace DataProvider.Helpers
{
    public class AdHelper
    {
        private const string DomainPath = "LDAP://DC=UN1T,DC=GROUP";
        private static NetworkCredential nc = GetAdUserCredentials();

        public static MailAddress[] GetRecipientsFromAdGroup(AdGroup group)
        {
            var list = new List<MailAddress>();
            using (WindowsImpersonationContextFacade impersonationContext
                = new WindowsImpersonationContextFacade(
                    nc))
            {
                string sid = AdUserGroup.GetSidByAdGroup(group);
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain, DomainPath);
                GroupPrincipal grp = GroupPrincipal.FindByIdentity(ctx, IdentityType.Sid, sid);

                if (grp != null)
                {
                    foreach (Principal p in grp.GetMembers(true))
                    {
                        string email = new Employee(p.Sid.Value).Email;
                        if (String.IsNullOrEmpty(email)) continue;
                        list.Add(new MailAddress(email));
                    }
                    grp.Dispose();
                }

                ctx.Dispose();

                return list.ToArray();
            }
        }

        private static string GetLoginFromEmail(string email)
        {
            return !string.IsNullOrEmpty(email) ? email.Substring(0, email.IndexOf("@", StringComparison.Ordinal)) : String.Empty;
        }

        public static NetworkCredential GetAdUserCredentials()
        {
            string accUserName = "UN1T\rehov";
            string accUserPass = "R3xQwi!!";

            string domain = "UN1T";//accUserName.Substring(0, accUserName.IndexOf("\\"));
            string name = "rehov";//accUserName.Substring(accUserName.IndexOf("\\") + 1);

            NetworkCredential nc = new NetworkCredential(name, accUserPass, domain);

            return nc;
        }

        //public static void GetEmployeeManager(Employee emp, out string managerUsername)
        //{
        //    managerUsername = String.Empty;
        //    if (emp.Department == null) return;

        //    if (!Department.CheckUserIsChief(emp.Department.Id, emp.Id))
        //    {
        //        managerUsername = emp.Manager != null ? GetLoginFromEmail(emp.Manager.Email) : String.Empty;
        //    }

        //    if (String.IsNullOrEmpty(managerUsername))
        //    {
        //        Department currDep = new Department(emp.Department.Id);
        //        GetParentDepChief(currDep, out managerUsername);
        //    }
            
        //}

        //public static void GetParentDepChief(Department dep, out string managerUsername)
        //{
        //    managerUsername = String.Empty;
        //    if (dep.ParentDepartment != null && dep.ParentDepartment.Id > 0)
        //    {
        //        var parentDep = new Department(dep.ParentDepartment.Id);
        //        var overManager = new Employee(parentDep.Chief.Id);
        //        managerUsername = GetLoginFromEmail(overManager.Email);
        //        if (String.IsNullOrEmpty(managerUsername))
        //        {
        //            GetParentDepChief(parentDep, out managerUsername);
        //        }
        //    }
        //}
        public static string SaveUser(Employee emp)
        {
            if (!emp.HasAdAccount) return String.Empty;
            using (WindowsImpersonationContextFacade impersonationContext
                = new WindowsImpersonationContextFacade(
                    nc))
            {
                string username = String.IsNullOrEmpty(emp.AdLogin) ? GetLoginFromEmail(emp.Email) : emp.AdLogin;

                if (String.IsNullOrEmpty(username.Trim())) return string.Empty;
                string mail = emp.Email;
                string fullName = emp.FullName;
                string surname = emp.Surname;
                string name = emp.Name;
                string position = emp.Position != null ? emp.Position.Id > 0 && String.IsNullOrEmpty(emp.Position.Name) ? new Position(emp.Position.Id).Name : emp.Position.Name : String.Empty;
                string workNum = emp.WorkNum;
                string mobilNum = emp.MobilNum;
                string city = emp.City != null ? emp.City.Name : String.Empty;
                string org = emp.Organization != null ? emp.Organization.Id > 0 && String.IsNullOrEmpty(emp.Organization.Name) ? new Organization(emp.Organization.Id).Name : emp.Organization.Name : String.Empty;
                string dep = emp.Department != null ? emp.Department.Id > 0 && String.IsNullOrEmpty(emp.Department.Name) ? new Department(emp.Department.Id).Name : emp.Department.Name : String.Empty;
                var photo = emp.Photo != null && emp.Photo.Length > 0 ? emp.Photo : null;
                Employee manager = new Employee(emp.Manager.Id);
                string managerUsername = String.IsNullOrEmpty(manager.AdLogin)
                    ? GetLoginFromEmail(manager.Email)
                    : manager.AdLogin;
                //GetEmployeeManager(emp, out managerUsername);
                string managerName = String.Empty;
                bool userIsExist = false;

                DirectoryEntry directoryEntry = new DirectoryEntry(DomainPath);

                using (directoryEntry)
                {
                    //Если пользователь существует
                    DirectorySearcher search = new DirectorySearcher(directoryEntry);
                    search.Filter = String.Format("(&(objectClass=user)(sAMAccountName={0}))", username);
                    SearchResult resultUser = search.FindOne();
                    userIsExist = resultUser != null && resultUser.Properties.Contains("sAMAccountName");
                }

                if (!String.IsNullOrEmpty(managerUsername.Trim()))
                {
                    using (directoryEntry)
                    {
                        DirectorySearcher search = new DirectorySearcher(directoryEntry);
                        search.Filter = String.Format("(&(objectClass=user)(sAMAccountName={0}))", managerUsername);
                        search.PropertiesToLoad.Add("DistinguishedName");
                        SearchResult resultManager = search.FindOne();
                        if (resultManager != null)
                            managerName = (string) resultManager.Properties["DistinguishedName"][0];
                    }
                }

                if (!userIsExist)
                {
                    //Создаем аккаунт в AD
                    using (
                        var pc = new PrincipalContext(ContextType.Domain, "UN1T", "OU=Users,OU=UNIT,DC=UN1T,DC=GROUP"))
                    {
                        using (var up = new UserPrincipal(pc))
                        {
                            up.SamAccountName = username;
                            up.UserPrincipalName = username;
                            up.SetPassword("z-123456");
                            up.Enabled = true;
                            up.ExpirePasswordNow();

                            try
                            {
                                up.Save();
                            }
                            catch (PrincipalOperationException ex)
                            {

                            }
                        }
                    }

                    //Создаем аккаунт в Exchange

                    //Создаем аккаунт в Lync
                }

                //Еще один путь для изменения параметров
                //if (up.GetUnderlyingObjectType() == typeof(DirectoryEntry))
                //{
                //    DirectoryEntry entry = (DirectoryEntry)up.GetUnderlyingObject();
                //        entry.Properties["streetAddress"].Value = address;

                //        entry.CommitChanges();

                //}

                directoryEntry = new DirectoryEntry(DomainPath);
                using (directoryEntry)
                {

                    //DirectoryEntry user = directoryEntry.Children.Add("CN=" + username, "user");
                    DirectorySearcher search = new DirectorySearcher(directoryEntry);
                    search.Filter = String.Format("(&(objectClass=user)(sAMAccountName={0}))", username);
                    search.PropertiesToLoad.Add("objectsid");
                    search.PropertiesToLoad.Add("samaccountname");
                    search.PropertiesToLoad.Add("userPrincipalName");
                    search.PropertiesToLoad.Add("mail");
                    search.PropertiesToLoad.Add("usergroup");
                    search.PropertiesToLoad.Add("displayname");
                    search.PropertiesToLoad.Add("givenName");
                    search.PropertiesToLoad.Add("sn");
                    search.PropertiesToLoad.Add("title");
                    search.PropertiesToLoad.Add("telephonenumber");
                    search.PropertiesToLoad.Add("homephone");
                    search.PropertiesToLoad.Add("mobile");
                    search.PropertiesToLoad.Add("manager");
                    search.PropertiesToLoad.Add("l");
                    search.PropertiesToLoad.Add("company");
                    search.PropertiesToLoad.Add("department");
                    //search.PropertiesToLoad.Add("userAccountControl");
                    
                        SearchResult resultUser = search.FindOne();

                        if (resultUser == null) return String.Empty;

                        DirectoryEntry user = resultUser.GetDirectoryEntry();
                        //user.Properties["sAMAccountName"].Value =username;
                        //user.Properties["userPrincipalName"].Value =username;
                        SetProp(ref user, ref resultUser, "mail", mail);
                        SetProp(ref user, ref resultUser, "displayname", fullName);
                        SetProp(ref user, ref resultUser, "givenName", surname);
                        SetProp(ref user, ref resultUser, "sn", name);
                        SetProp(ref user, ref resultUser, "title", position);
                        SetProp(ref user, ref resultUser, "telephonenumber", workNum);
                        SetProp(ref user, ref resultUser, "mobile", mobilNum);
                        SetProp(ref user, ref resultUser, "l", city);
                        SetProp(ref user, ref resultUser, "company", org);
                        SetProp(ref user, ref resultUser, "department", dep);
                        SetProp(ref user, ref resultUser, "manager", managerName);
                        user.Properties["jpegPhoto"].Clear();
                        SetProp(ref user, ref resultUser, "jpegPhoto", photo);
                        //using (WindowsImpersonationContextFacade impersonationContext= new WindowsImpersonationContextFacade(nc))
                        //{
                        user.CommitChanges();
                        //}
                        SecurityIdentifier sid = new SecurityIdentifier((byte[]) resultUser.Properties["objectsid"][0],
                            0);

                        return sid.Value;
                    
                }
                return String.Empty;
            }
        }

        public static void SetProp(ref DirectoryEntry user, ref SearchResult result, string name, object value)
        {
            if (value == null || String.IsNullOrEmpty(value.ToString()) || String.IsNullOrEmpty(name)) return;
            if (result.Properties.Contains(name))
            {
                user.Properties[name].Value = value;
            }
            else
            {
                user.Properties[name].Add(value);
            }
        }

        public static string GenerateLoginByName(string surname, string name)
        {
            using (WindowsImpersonationContextFacade impersonationContext
                = new WindowsImpersonationContextFacade(
                    nc))
            {
                string login = String.Empty;
                int maxLoginLength = 19; //-1 - потомучто будет точка
                var trans = new Transliteration();
                string surnameTranslit = trans.GetTranslit(surname);
                string nameTranslit = trans.GetTranslit(name);
                //Если длина транслита превышает максимальное значение
                if (surnameTranslit.Length > maxLoginLength)
                {
                    surnameTranslit = surnameTranslit.Substring(0, maxLoginLength);
                    nameTranslit = String.Empty;
                }
                else if (surnameTranslit.Length + nameTranslit.Length > maxLoginLength)
                {
                    nameTranslit = nameTranslit.Substring(0, maxLoginLength - surnameTranslit.Length);
                }

                bool flag = false;
                int i = 0;
                int j = 1;
                string nameAccount = nameTranslit;
                string surnameAccount = surnameTranslit;
                do
                {
                    if (i >= 1 && i < nameTranslit.Length)
                    {
                        nameAccount = nameTranslit.Substring(0, i);
                    }
                    else if (i >= 1 && i >= nameTranslit.Length)
                    {
                        login = "ERROR";
                        break;
                        //nameAccount = String.Format("{1}{0}", nameTranslit, j++);
                    }

                    login = String.Format("{0}.{1}", nameAccount, surnameAccount).ToLower();

                    DirectoryEntry directoryEntry = new DirectoryEntry(DomainPath);
                    using (directoryEntry)
                    {
                        DirectorySearcher search = new DirectorySearcher(directoryEntry);
                        search.Filter = String.Format("(&(objectClass=user)(sAMAccountName={0}))", login);
                        SearchResult result = search.FindOne();
                        flag = result != null && result.Properties.Contains("sAMAccountName");
                    }
                    i++;
                } while (flag);

                return login;
            }
        }

        public static bool UserInGroup(IPrincipal user, params AdGroup[] groups)
        {
            using (WindowsImpersonationContextFacade impersonationContext
                = new WindowsImpersonationContextFacade(
                    nc))
            {
                var context = new PrincipalContext(ContextType.Domain);
                var userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName,
                    user.Identity.Name);

                if (userPrincipal == null) return false;
                if (userPrincipal.IsMemberOf(context, IdentityType.Sid, AdUserGroup.GetSidByAdGroup(AdGroup.SuperAdmin))) { return true; }//Если юзер Суперадмин

                foreach (var grp in groups)
                {
                    if (userPrincipal.IsMemberOf(context, IdentityType.Sid, AdUserGroup.GetSidByAdGroup(grp)))
                    {
                        return true;
                    }
                }


                return false;
            }
        }
    }
}