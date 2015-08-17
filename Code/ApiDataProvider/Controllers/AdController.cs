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
    public class AdController : BaseApiController
    {
        public string GetSid()
        {
            string curSid = GetCurUser().Sid;
            return curSid;
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
        [AuthorizeAd(Groups = new[] { AdGroup.SystemUser })]
        public HttpResponseMessage Synchronyze()
        {
            //RequestContext.Principal

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);
            string sid = GetCurUser().Sid;
            try
            {
                foreach (Employee emp in Employee.GetList(getPhoto: true))
                {
                    Employee e = emp;
                    try
                    {
                        if (e.DisplayName == "Тестов Т.Т.")
                        {
                            string s = "";
                        }
                        AdHelper.SaveUser(e);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(String.Format("{{\"errorMessage\":\"{0}\"}}", ex.Message));

            }
            return response;
        }
    }
}
