using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
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
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
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
        
        public static IEnumerable<KeyValuePair<string, string>> GetUserListByAdGroup(AdGroup grp)
        {
            var list = new Dictionary<string, string>();

            using (WindowsImpersonationContextFacade impersonationContext
                = new WindowsImpersonationContextFacade(
                    nc))
            {
                var domain = new PrincipalContext(ContextType.Domain);
                var group = GroupPrincipal.FindByIdentity(domain, IdentityType.Sid, AdUserGroup.GetSidByAdGroup(grp));
                if (group != null)
                {
                    var members = group.GetMembers(true);
                    foreach (var principal in members)
                    {
                        var userPrincipal = UserPrincipal.FindByIdentity(domain, principal.SamAccountName);
                        if (userPrincipal != null)
                        {
                            var name = Employee.ShortName(userPrincipal.DisplayName);
                            var sid = userPrincipal.Sid.Value;
                            list.Add(sid, name);
                        }
                    }
                }
            }

            return list.OrderBy(x => x.Value);
        }

        public static IEnumerable<KeyValuePair<string, string>> GetGroupListByAdOrg(AdOrg org)
        {
            var list = new Dictionary<string, string>();

            using (WindowsImpersonationContextFacade impersonationContext
                = new WindowsImpersonationContextFacade(
                    nc))
            {
                var domain = new PrincipalContext(ContextType.Domain, "UN1T.GROUP", String.Format("{0}, DC=UN1T,DC=GROUP", AdOrganization.GetAdPathByAdOrg(org)));
                GroupPrincipal groupList = new GroupPrincipal(domain, "*");
                PrincipalSearcher ps = new PrincipalSearcher(groupList);

                foreach (var grp in ps.FindAll())
                {
                    list.Add(grp.Sid.Value, grp.Name);
                }
            }

            return list;
        }

        private static string GetLoginFromEmail(string email)
        {
            return !string.IsNullOrEmpty(email) ? StringHelper.Trim(email.Substring(0, email.IndexOf("@", StringComparison.Ordinal))) : String.Empty;
        }

        public static NetworkCredential GetAdUserCredentials()
        {
            string accUserName = @"UN1T\adUnit_prog";
            string accUserPass = "1qazXSW@";

            string domain = "UN1T";//accUserName.Substring(0, accUserName.IndexOf("\\"));
            string name = "adUnit_prog";//accUserName.Substring(accUserName.IndexOf("\\") + 1);

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
                string username = String.IsNullOrEmpty(emp.AdLogin) ? GetLoginFromEmail(emp.Email) : StringHelper.Trim(emp.AdLogin);

                if (String.IsNullOrEmpty(username.Trim())) return string.Empty;
                string mail = StringHelper.Trim(emp.Email);
                string fullName = StringHelper.Trim(emp.FullName);
                string surname = StringHelper.Trim(emp.Surname);
                string name = StringHelper.Trim(emp.Name);
                string position = emp.Position != null ? emp.Position.Id > 0 && String.IsNullOrEmpty(emp.Position.Name) ? new Position(emp.Position.Id).Name : emp.Position.Name : String.Empty;
                string workNum = StringHelper.Trim(emp.WorkNum);
                string mobilNum = StringHelper.Trim(emp.MobilNum);
                string city = emp.City != null ? StringHelper.Trim(emp.City.Name) : String.Empty;
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
                            managerName = (string)resultManager.Properties["DistinguishedName"][0];
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
                            up.UserPrincipalName = username + "@unitgroup.ru";
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
                    //search.PropertiesToLoad.Add("modifyTimeStamp");
                    //search.PropertiesToLoad.Add("whenChanged");
                    //search.PropertiesToLoad.Add("whenCreated");

                    SearchResult resultUser = search.FindOne();

                    if (resultUser == null) return String.Empty;

                    //string s = resultUser.Properties["modifyTimeStamp"][0].ToString();
                    //string s1 = resultUser.Properties["whenChanged"][0].ToString();
                    //string s2 = resultUser.Properties["whenCreated"][0].ToString();
                    //DateTime d = DateTime.FromFileTime((Int64)resultUser.Properties["uSNChanged"][0]);

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
                    SecurityIdentifier sid = new SecurityIdentifier((byte[])resultUser.Properties["objectsid"][0],
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
                string fakseLogin = null;

                if (ConfigurationManager.AppSettings["UserProxy"] == "True")
                {
                    fakseLogin = ConfigurationManager.AppSettings["UserProxyLogin"];
                }
                string login = fakseLogin ?? user.Identity.Name;
                var context = new PrincipalContext(ContextType.Domain);
                var userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, login);

                if (userPrincipal == null) return false;
                if (userPrincipal.IsMemberOf(context, IdentityType.Sid, AdUserGroup.GetSidByAdGroup(AdGroup.SuperAdmin))) { return true; }//Если юзер Суперадмин

                return groups.Select(grp => GroupPrincipal.FindByIdentity(context, IdentityType.Sid, AdUserGroup.GetSidByAdGroup(grp))).Where(g => g != null).Any(g => g.GetMembers(true).Cast<UserPrincipal>().Any(usr => usr.SamAccountName == login));
            }
        }

        private static List<GroupObj> _groups = new List<GroupObj>();

        public static List<string> GetMembershipWithPath(string groupSid)
        {
            List<string> retVal = new List<string>();

            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
            GroupPrincipal grp = GroupPrincipal.FindByIdentity(ctx, IdentityType.Sid, groupSid);
            if (grp != null)
            {
                BuildHList(grp, 0, null);
                foreach (UserPrincipal usr in grp.GetMembers(true))
                    retVal.Add(GetMbrPath(usr));
            }

            return retVal;
        }

        private static void BuildHList(GroupPrincipal node, int level, GroupPrincipal parent)
        {
            PrincipalSearchResult<Principal> rslts = node.GetMembers();
            _groups.Add(new GroupObj() { Group = node, Level = level, Parent = parent });
            foreach (GroupPrincipal grp in rslts.Where(g => g is GroupPrincipal))
                BuildHList(grp, level + 1, node);
        }

        private static string GetMbrPath(UserPrincipal usr)
        {
            Stack<string> output = new Stack<string>();
            StringBuilder retVal = new StringBuilder();
            GroupObj fg = null, tg = null;
            output.Push(usr.Name);
            foreach (GroupObj go in _groups)
            {
                if (usr.IsMemberOf(go.Group))
                {
                    output.Push(go.Group.Name);
                    fg = go;
                    while (fg.Parent != null)
                    {
                        output.Push(fg.Parent.Name);
                        tg = (from g in _groups where g.Group == fg.Parent select g).FirstOrDefault();
                        fg = tg;
                    }
                    break;
                }
            }
            while (output.Count > 1)
                retVal.AppendFormat("{0} ->", output.Pop());
            retVal.Append(output.Pop());

            return retVal.ToString();
        }

        public class GroupObj
        {
            public GroupPrincipal Group { get; set; }
            public int Level { get; set; }
            public GroupPrincipal Parent { get; set; }
        }

        /// <summary>  
        /// Gets the Container Name (CN) of the input user.  
        /// </summary>  
        public static string GetUserContainerName(string userName)
        {
            DirectoryEntry entry = new DirectoryEntry(DomainPath);
            // Create a DirectorySearcher object.  
            DirectorySearcher mySearcher = new DirectorySearcher(entry);
            mySearcher.Filter = string.Format("(&(sAMAccountName={0}))", userName);
            mySearcher.SearchScope = SearchScope.Subtree; //Search from base down to ALL children.   
            SearchResultCollection result = mySearcher.FindAll();
            if (result.Count == 0)
                throw new ApplicationException(string.Format("User '{0}' Not Found in Active Directory.", userName));
            return result[0].GetDirectoryEntry().Name.Replace("CN=", string.Empty);
        }

        public static bool UserIs(IPrincipal user, params AdGroup[] groups)
        {
            using (WindowsImpersonationContextFacade impersonationContext
                = new WindowsImpersonationContextFacade(
                    nc))
            {
                var context = new PrincipalContext(ContextType.Domain);
                var userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, user.Identity.Name);

                if (userPrincipal == null) return false;
                //if (userPrincipal.IsMemberOf(context, IdentityType.Sid, AdUserGroup.GetSidByAdGroup(AdGroup.SuperAdmin))) { return true; }//Если юзер Суперадмин

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

        public static string CreateSimpleAdUser(string username, string password, string name, string description, string adPath = "OU=Users,OU=UNIT,DC=UN1T,DC=GROUP")
        {
            using (WindowsImpersonationContextFacade impersonationContext
               = new WindowsImpersonationContextFacade(
                   nc))
            {
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

                if (!userIsExist)
                {
                    //Создаем аккаунт в AD
                    using (
                        var pc = new PrincipalContext(ContextType.Domain, "UN1T", adPath))
                    {
                        using (var up = new UserPrincipal(pc))
                        {
                            up.SamAccountName = username;
                            up.UserPrincipalName = username + "@unitgroup.ru";

                            up.SetPassword(password);
                            up.Enabled = true;
                            up.PasswordNeverExpires = true;
                            up.UserCannotChangePassword = true;
                            up.Description = description;
                            //up.DistinguishedName = "DC=unitgroup.ru";
                            try
                            {
                                up.Save();
                            }
                            catch (PrincipalOperationException ex)
                            {

                            }
                            up.UnlockAccount();
                        }
                    }
                }

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

                    SearchResult resultUser = search.FindOne();

                    if (resultUser == null) return String.Empty;

                    DirectoryEntry user = resultUser.GetDirectoryEntry();
                    //SetProp(ref user, ref resultUser, "mail", mail);
                    SetProp(ref user, ref resultUser, "displayname", name);
                    SetProp(ref user, ref resultUser, "givenName", username);
                    SetProp(ref user, ref resultUser, "sn", name);
                    SetProp(ref user, ref resultUser, "title", description);
                    //SetProp(ref user, ref resultUser, "telephonenumber", workNum);
                    //SetProp(ref user, ref resultUser, "mobile", mobilNum);
                    SetProp(ref user, ref resultUser, "l", description);
                    SetProp(ref user, ref resultUser, "company", name);
                    SetProp(ref user, ref resultUser, "department", "1");
                    //SetProp(ref user, ref resultUser, "manager", "");
                    //user.Properties["jpegPhoto"].Clear();
                    //SetProp(ref user, ref resultUser, "jpegPhoto", photo);
                    user.CommitChanges();

                    SecurityIdentifier sid = new SecurityIdentifier((byte[])resultUser.Properties["objectsid"][0],
                        0);

                    return sid.Value;

                }
                return String.Empty;
            }
        }

        public static string CreateAdGroup(string name, string adPath)
        {
            string grpSid = String.Empty;

            using (WindowsImpersonationContextFacade impersonationContext
              = new WindowsImpersonationContextFacade(
                  nc))
            {
                //DirectoryEntry directoryEntry = new DirectoryEntry(DomainPath);
                //DirectoryEntry ou = directoryEntry.Children.Find(adPath);
                //DirectoryEntry group = ou.Children.Add($"CN={name}", "group");
                //group.Properties["samAccountName"].Value = name;
                //group.CommitChanges();

                bool groupIsExist = false;
                DirectoryEntry directoryEntry = new DirectoryEntry(DomainPath);

                using (directoryEntry)
                {
                    //Если пользователь существует
                    DirectorySearcher search = new DirectorySearcher(directoryEntry);
                    search.Filter = String.Format("(&(objectClass=user)(sAMAccountName={0}))", name);
                    SearchResult resultGroup = search.FindOne();
                    groupIsExist = resultGroup != null && resultGroup.Properties.Contains("sAMAccountName");


                    if (!groupIsExist)
                    {

                        DirectoryEntry ou = directoryEntry.Children.Find(adPath);
                        DirectoryEntry group = ou.Children.Add($"CN={name}", "group");
                        group.Properties["samAccountName"].Value = name;
                        group.CommitChanges();
                        SecurityIdentifier sid = new SecurityIdentifier((byte[])group.Properties["objectsid"][0],
                            0);
                        grpSid = sid.Value;
                    }
                    else
                    {
                        SecurityIdentifier sid = new SecurityIdentifier((byte[])resultGroup.Properties["objectsid"][0],
                            0);
                        grpSid = sid.Value;
                    }
                }
            }

            return grpSid;
        }

        public static void IncludeUser2AdGroup(string userSid, params AdGroup[] groups)
        {
            using (WindowsImpersonationContextFacade impersonationContext
               = new WindowsImpersonationContextFacade(
                   nc))
            {
                var context = new PrincipalContext(ContextType.Domain);
                var userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.Sid, userSid);
                if (userPrincipal == null) return;

                foreach (var grp in groups)
                {
                    //Если пользователь не является членом группы то включаем
                    if (userPrincipal.IsMemberOf(context, IdentityType.Sid, AdUserGroup.GetSidByAdGroup(grp)))
                    {
                        continue;
                    }
                    else
                    {
                        var group = GroupPrincipal.FindByIdentity(context, IdentityType.Sid,
                            AdUserGroup.GetSidByAdGroup(grp));
                        if (group != null)
                        {
                            group.Members.Add(userPrincipal);
                            group.Save();
                        }
                    }
                }

            }
        }

        public static void IncludeUser2AdGroup(string userSid, params string[] groupSidList)
        {
            using (WindowsImpersonationContextFacade impersonationContext
               = new WindowsImpersonationContextFacade(
                   nc))
            {
                var context = new PrincipalContext(ContextType.Domain);
                var userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.Sid, userSid);
                if (userPrincipal == null) return;

                foreach (var grpSid in groupSidList)
                {
                    //Если пользователь не является членом группы то включаем
                    if (userPrincipal.IsMemberOf(context, IdentityType.Sid, grpSid))
                    {
                        continue;
                    }
                    else
                    {
                        var group = GroupPrincipal.FindByIdentity(context, IdentityType.Sid, grpSid);
                        if (group != null)
                        {
                            group.Members.Add(userPrincipal);
                            group.Save();
                        }
                    }
                }

            }
        }

        public static void ExcludeUserFromAdGroup(string userSid, params AdGroup[] groups)
        {
            using (WindowsImpersonationContextFacade impersonationContext
               = new WindowsImpersonationContextFacade(
                   nc))
            {
                var context = new PrincipalContext(ContextType.Domain);
                var userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.Sid, userSid);
                if (userPrincipal == null) return;

                foreach (var grp in groups)
                {
                    //Если пользователь является членом группы то исключаем
                    if (userPrincipal.IsMemberOf(context, IdentityType.Sid, AdUserGroup.GetSidByAdGroup(grp)))
                    {
                        var group = GroupPrincipal.FindByIdentity(context, IdentityType.Sid, AdUserGroup.GetSidByAdGroup(grp));
                        if (group != null)
                        {
                            group.Members.Remove(userPrincipal);
                            group.Save();
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

        public static void ExcludeUserFromAdGroup(string userSid, params string[] groupSidList)
        {
            using (WindowsImpersonationContextFacade impersonationContext
               = new WindowsImpersonationContextFacade(
                   nc))
            {
                var context = new PrincipalContext(ContextType.Domain);
                var userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.Sid, userSid);
                if (userPrincipal == null) return;

                foreach (var grpSid in groupSidList)
                {
                    //Если пользователь является членом группы то исключаем
                    if (userPrincipal.IsMemberOf(context, IdentityType.Sid, grpSid))
                    {
                        var group = GroupPrincipal.FindByIdentity(context, IdentityType.Sid, grpSid);
                        if (group != null)
                        {
                            group.Members.Remove(userPrincipal);
                            group.Save();
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

        public static EmployeeSm GetUserBySid(string sid)
        {
            var result = new EmployeeSm();

            using (WindowsImpersonationContextFacade impersonationContext
                = new WindowsImpersonationContextFacade(
                    nc))
            {
                var context = new PrincipalContext(ContextType.Domain);
                var userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.Sid, sid);

                if (userPrincipal != null)
                {
                    result.AdSid = sid;
                    result.FullName = userPrincipal.DisplayName;
                    result.DisplayName = Employee.ShortName(result.FullName);
                }
            }

            return result;
        }

        public static string GetADGroupNameBySid(string sid)
        {
            string name = null;
            using (WindowsImpersonationContextFacade impersonationContext
                = new WindowsImpersonationContextFacade(
                    nc))
            {

                //Если пользователь существует
                var context = new PrincipalContext(ContextType.Domain);
                var gp = GroupPrincipal.FindByIdentity(context, IdentityType.Sid, sid);
                name = gp.SamAccountName;

            }

            return name;
        }
    }
}