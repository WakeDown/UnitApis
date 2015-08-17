using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Objects
{
    public class AdUserGroup
    {
        public AdGroup Group { get; set; }
        public string Sid { get; set; }
        public string Name { get; set; }

        public AdUserGroup(AdGroup grp, string sid, string name)
        {
            Group = grp;
            Sid = sid;
            Name = name;
        }

        public static IEnumerable<AdUserGroup> GetList()
        {
            var list = new List<AdUserGroup>();
            list.Add(new AdUserGroup(AdGroup.SuperAdmin, "S-1-5-21-1970802976-3466419101-4042325969-4031", "SuperAdmin"));//Суперадмин
            list.Add(new AdUserGroup(AdGroup.SystemUser, "S-1-5-21-1970802976-3466419101-4042325969-4031", "SystemUser"));//Системынй пользователь
            list.Add(new AdUserGroup(AdGroup.PersonalManager, "S-1-5-21-1970802976-3466419101-4042325969-4033", "Personal-Manager"));//Менеджер по персоналу
            list.Add(new AdUserGroup(AdGroup.NewEmployeeNote, "S-1-5-21-1970802976-3466419101-4042325969-4036", "NewEmployeeNote"));//Уведомление о новом пользователе

            list.Add(new AdUserGroup(AdGroup.SpeCalcKontroler, "S-1-5-21-1970802976-3466419101-4042325969-4286", "SpeCalcKontroler"));
            list.Add(new AdUserGroup(AdGroup.SpeCalcManager, "S-1-5-21-1970802976-3466419101-4042325969-4283", "SpeCalcManager"));
            list.Add(new AdUserGroup(AdGroup.SpeCalcProduct, "S-1-5-21-1970802976-3466419101-4042325969-4284", "SpeCalcProduct"));
            list.Add(new AdUserGroup(AdGroup.SpeCalcOperator, "S-1-5-21-1970802976-3466419101-4042325969-4287", "SpeCalcOperator"));
            list.Add(new AdUserGroup(AdGroup.SpeCalcKonkurs, "S-1-5-21-1970802976-3466419101-4042325969-4285", "SpeCalcKonkurs"));
            return list;
        }


        public static string GetSidByAdGroup(AdGroup grp)
        {
            return GetList().Single(g => g.Group == grp).Sid;
        }
    }
}