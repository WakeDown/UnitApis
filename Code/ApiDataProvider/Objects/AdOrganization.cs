using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataProvider.Objects
{
    public enum AdOrg
    {
        None,
        EngeneerGroups,
            ZipClient
    }

    public class AdOrganization
    {
        public AdOrg Org { get; set; }
        public string AdPath { get; set; }
        public string Name { get; set; }

        public AdOrganization(AdOrg org, string path, string name)
        {
            Org = org;
            AdPath = path;
            Name = name;
        }

        public static IEnumerable<AdOrganization> GetList()
        {
            var list = new List<AdOrganization>();
            list.Add(new AdOrganization(AdOrg.EngeneerGroups, "OU=engeneer-groups,OU=Service,OU=System-Groups,OU=UNIT", "EngeneersGroup"));
            list.Add(new AdOrganization(AdOrg.ZipClient, "OU=Zip-client,OU=Users External,DC=UN1T,DC=GROUP", "Zip-client"));
            return list;
        }

        public static string GetAdPathByAdOrg(AdOrg org)
        {
            return GetList().Single(g => g.Org == org).AdPath;
        }

        public static AdOrg GetAdOrgByAdPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return AdOrg.None;
            var grp = GetList().Single(g => g.AdPath == path).Org;
            return grp;
        }
    }
}