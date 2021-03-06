﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Models.Stuff;
using DataProvider.Objects;
using Objects;

namespace DataProvider._TMPLTS
{
    public class Controller : BaseApiController
    {
        public IEnumerable<Model> GetList()
        {
            return Model.GetList();
        }

        public Model Get(int id)
        {
            var model = new Model(id);
            return model;
        }

        [AuthorizeAd()]
        public HttpResponseMessage Save(Model model)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                model.CurUserAdSid = GetCurUser().Sid;
                model.Save();
                response.Content = new StringContent($"{{\"id\":{model.Id}}}");
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }

        [AuthorizeAd()]
        public HttpResponseMessage Close(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                Model.Close(id,GetCurUser().Sid);
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }
    }
}