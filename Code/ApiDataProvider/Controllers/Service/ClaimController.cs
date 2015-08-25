using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataProvider.Helpers;
using DataProvider.Models;
using DataProvider.Models.Service;
using DataProvider.Objects;
using DataProvider._TMPLTS;
using Objects;

namespace DataProvider.Controllers.Service
{
    public class ClaimController : BaseApiController
    {

        public ListResult<Claim> GetList(int? idAdmin = null, int? idEngeneer = null, DateTime? dateStart = null, DateTime? dateEnd = null, int? topRows = null)
        {
            //AdHelper.UserInGroup(GetCurUser().User, AdGroup.ServiceEngeneer, AdGroup.SuperAdmin);
            int cnt;
            var list = Claim.GetList(out cnt, idAdmin, idEngeneer, dateStart, dateEnd, topRows);
            return new ListResult<Claim>(list, cnt);
        }

        [AuthorizeAd(Groups = new[] { AdGroup.SuperAdmin })]
        public Claim Get(int id)
        {
            AdHelper.UserInGroup(GetCurUser().User, AdGroup.ServiceEngeneer, AdGroup.ServiceAdmin, AdGroup.ServiceControler, AdGroup.ServiceTech, AdGroup.SuperAdmin);
            Claim model;
            try
            {
                model = new Claim(id, GetCurUser(), true);
            }
            catch (AccessDenyException ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent(ex.Message),
                    ReasonPhrase = ex.Message
                };
                throw new HttpResponseException(resp);
            }
            return model;
        }

        [AuthorizeAd(Groups = new[] { AdGroup.SuperAdmin })]
        public IEnumerable<Claim2ClaimState> GetStateHistory(int? id)
        {
            if (!id.HasValue) return new[] { new Claim2ClaimState() };
            return Claim2ClaimState.GetList(id.Value);
        }

        //[AuthorizeAd(Groups = new[] { AdGroup.SuperAdmin })]
        //public HttpResponseMessage SaveAndGoNextState(Claim model)
        //{
        //    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

        //    try
        //    {
        //        model.CurUserAdSid = GetCurUser().Sid;
        //        model.Save(/*SetNextState.Next*/);
        //        //model.Go2State(SetNextState.Next);
        //        response.Content = new StringContent(String.Format("{{\"id\":{0},\"sid\":\"{1}\"}}", model.Id, model.Sid));
        //    }
        //    catch (Exception ex)
        //    {
        //        response = new HttpResponseMessage(HttpStatusCode.OK);
        //        response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

        //    }
        //    return response;
        //}

        [AuthorizeAd(Groups = new[] { AdGroup.SuperAdmin })]
        public HttpResponseMessage GoBack(Claim model)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                model.CurUserAdSid = GetCurUser().Sid;
                //model.Save(SetNextState.Back);
                //model.Go(SetNextState.Back);
                //model.Go2State(SetNextState.Back);
                response.Content = new StringContent(String.Format("{{\"id\":{0},\"sid\":\"{1}\"}}", model.Id, model.Sid));
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }

        //[AuthorizeAd(Groups = new[] { AdGroup.SuperAdmin })]
        //public HttpResponseMessage SaveAndGoEndState(Claim model)
        //{
        //    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

        //    try
        //    {
        //        model.CurUserAdSid = GetCurUser().Sid;
        //        //model.Save(SetNextState.End);
        //        model.Save();
        //        model.Go2State(SetNextState.End);
        //        response.Content = new StringContent(String.Format("{{\"id\":{0},\"sid\":\"{1}\"}}", model.Id, model.Sid));
        //    }
        //    catch (Exception ex)
        //    {
        //        response = new HttpResponseMessage(HttpStatusCode.OK);
        //        response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

        //    }
        //    return response;
        //}

        [AuthorizeAd(Groups = new[] { AdGroup.SuperAdmin })]
        public HttpResponseMessage Go(Claim model)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                model.CurUserAdSid = GetCurUser().Sid;
                model.Go();
                response.Content = new StringContent(String.Format("{{\"id\":{0},\"sid\":\"{1}\"}}", model.Id, model.Sid));
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }

        [AuthorizeAd(Groups = new[] { AdGroup.SuperAdmin })]
        public HttpResponseMessage Save(Claim model)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                model.CurUserAdSid = GetCurUser().Sid;
                model.Save();
                response.Content = new StringContent(String.Format("{{\"id\":{0},\"sid\":\"{1}\"}}", model.Id, model.Sid));
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }

        [AuthorizeAd(Groups = new[] { AdGroup.SuperAdmin })]
        public HttpResponseMessage Close(string sid)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                Claim.Close(sid, GetCurUser().Sid);
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(String.Format("{{\"errorMessage\":\"{0}\"}}", ex.Message));

            }
            return response;
        }

        public IEnumerable<KeyValuePair<string, string>> GetCurrentClaimSpecialistList(int id)
        {
            return Claim.GetSpecialistList(id);
        }
    }
}
