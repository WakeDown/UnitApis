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

        public List<AdGroup> AdGroups { get; set; }

        public string Login { get; set; }
        private string _fullName;
        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;
                DisplayName = ShortName(FullName);
            }
        }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string DepartmentName { get; set; }
        public string PositionName { get; set; }
        public string Gender { get; set; }

        public byte[] PhotoByte { get; set; }

        //private byte[] _photo;
        public byte[] Photo
        {
            //get { return _photo; }
            set
            {
                //_photo = value;
                if (value != null)
                {
                    var base64 = Convert.ToBase64String(value);
                    var src = String.Format("data:image/gif;base64,{0}", base64);
                    PhotoBase64Src = src;
                }
                else
                {
                    PhotoBase64Src = null;
                }
            }
        }
        public string PhotoBase64Src { get; set; }

        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string CityName { get; set; }
        public string ManagerName { get; set; }
        public string ManagerSid { get; set; }
        public bool Enabled { get; set; }

        public void SetUserGroups(IEnumerable<AdGroup> groups) => AdGroups =groups.ToList();

        public static string ShortName(string fullName)
        {
            if (String.IsNullOrEmpty(fullName)) return String.Empty;
            string result = String.Empty;
            string[] nameArr = fullName.Split(' ');
            for (int i = 0; i < nameArr.Count(); i++)
            {
                //if (i > 2) break;
                string name = nameArr[i];
                if (String.IsNullOrEmpty(name)) continue;
                if (i > 0) name = name[0] + ".";
                if (i == 1) name = " " + name;
                result += name;
            }
            return result;
        }

        public bool Is(params AdGroup[] groups)
        {
            return groups.Select(grp => AdGroups.Contains(grp)).Any(res => res);
            //return AdHelper.UserIs(User, groups);
        }

        public bool HasAccess(params AdGroup[] groups)
        {
            if (AdGroups == null || !AdGroups.Any()) return false;
            if (AdGroups.Contains(AdGroup.SuperAdmin)) return true;
            return groups.Select(grp => AdGroups.Contains(grp)).Any(res => res);
            //return AdHelper.UserInGroup(User, groups);
        }
    }
}