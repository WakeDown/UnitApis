﻿using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http;
using DataProvider.Helpers;
using DataProvider.Models.Stuff;
using DataProvider.Objects;
using Objects;
using WebGrease.Css.Extensions;

namespace DataProvider.Controllers
{
    [Authorize]
    public class AdController : BaseApiController
    {
        [AllowAnonymous]
        public IEnumerable<AdUser> GetUsers()
        {
            var list = AdHelper.GetUserList();
            return list;
        } 

        public IEnumerable<KeyValuePair<string, string>> GetUserListByGroupSid(string sid)
        {
            return AdHelper.GetUserListByAdGroup(sid);
        }

        public IEnumerable<KeyValuePair<string, string>> GetUserListByAdGroup(AdGroup group)
        {
            return AdHelper.GetUserListByAdGroup(group);
        }

        public IEnumerable<KeyValuePair<string, string>> GetGroupListByAdOrg(AdOrg org)
        {
            return AdHelper.GetGroupListByAdOrg(org);
        }

        public string GetSid()
        {
            string curSid = GetCurUser().Sid;
            return curSid;
        }
        [HttpGet]

        public bool UserInGroup(string groupSid)
        {
            var grp = AdUserGroup.GetAdGroupBySid(groupSid);
            return AdHelper.UserInGroup(GetCurUser().User, grp);
        }

        public IEnumerable<string> GetUserGroups()
        {
            List<string> result = new List<string>();
            foreach (AdGroup grp in GetCurUser().AdGroups)
            {
                result.Add(AdUserGroup.GetSidByAdGroup(grp));
            }

            return result;
        }

        [HttpGet]
        [AuthorizeAd(Groups = new[] { AdGroup.PersonalManager })]
        public string GetEmailAddressByName(string surname, string name)
        {
            return String.Format("{0}@unitgroup.ru", GetLoginByName(surname, name));
        }

        [HttpGet]
        [AuthorizeAd(Groups = new[] { AdGroup.PersonalManager })]
        public string GetLoginByName(string surname, string name)
        {
            return AdHelper.GenerateLoginByName(surname, name);
            //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            //try
            //{
            //    response.Content = new StringContent(AdHelper.CreateLoginByName(surname, name));
            //}
            //catch (Exception ex)
            //{
            //    response = new HttpResponseMessage(HttpStatusCode.OK);
            //    response.Content = new StringContent(String.Format("{{\"errorMessage\":\"{0}\"}}", ex.Message));

            //}
            //return response;
        }
        
        [HttpGet]
        [AuthorizeAd(AdGroup.SystemUser)]
        public HttpResponseMessage Synchronyze()
        {
            //RequestContext.Principal

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);
            //string sid = GetCurUser().Sid;
            //try
            //{
                foreach (Employee emp in Employee.GetList(getPhoto: true, getNewbies:true))
                {
                    Employee e = emp;
                    //try
                    //{
                       string sid= AdHelper.SaveUser(e);
                        if (!String.IsNullOrEmpty(sid))
                        {
                            e.AdSid = sid;
                            e.Save();
                        }
                    //}
                    //catch (UnauthorizedAccessException ex)
                    //{
                    //    continue;
                    //}
                }
            //}
            //catch (Exception ex)
            //{
            //    response = new HttpResponseMessage(HttpStatusCode.OK);
            //    response.Content = new StringContent(String.Format("{{\"errorMessage\":\"{0}\"}}", ex.Message));

            //}
            return response;
        }
    }
}
