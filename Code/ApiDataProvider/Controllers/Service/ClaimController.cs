using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
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

        public ListResult<Claim> GetList(string servAdminSid = null, string servEngeneerSid = null, DateTime? dateStart = null, DateTime? dateEnd = null, int? topRows = null, string managerSid = null, string techSid = null, string serialNum = null, int? idDevice = null, bool? activeClaimsOnly = false, int? idClaimState = null, int? clientId = null, string clientSdNum = null)
        {
            int cnt;
            var list = Claim.GetList(GetCurUser(), out cnt, servAdminSid, servEngeneerSid, dateStart, dateEnd, topRows, managerSid, techSid, serialNum, idDevice, activeClaimsOnly, idClaimState, clientId, clientSdNum);
            return new ListResult<Claim>(list, cnt);
        }

        [AuthorizeAd(Groups = new[] { AdGroup.SuperAdmin, AdGroup.ServiceTech, AdGroup.ServiceControler, AdGroup.ServiceAdmin, AdGroup.ServiceManager, AdGroup.ServiceEngeneer })]
        public Claim Get(int id)
        {
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

        [AuthorizeAd(Groups = new[] { AdGroup.SuperAdmin, AdGroup.ServiceTech, AdGroup.ServiceControler, AdGroup.ServiceAdmin, AdGroup.ServiceManager })]
        public IEnumerable<Claim2ClaimState> GetStateHistory(int? id, int? topRows)
        {
           if (!id.HasValue) return new[] { new Claim2ClaimState() };
            return Claim2ClaimState.GetList(id.Value, topRows);
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

        [AuthorizeAd(Groups = new[] { AdGroup.SuperAdmin, AdGroup.ServiceControler, AdGroup.ServiceTech, AdGroup.ServiceAdmin, AdGroup.ServiceManager })]
        public HttpResponseMessage GoBack(Claim model)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                model.CurUserAdSid = GetCurUser().Sid;
                //model.Save(SetNextState.Back);
                model.Go(false);
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

        [AuthorizeAd(Groups = new[] { AdGroup.SuperAdmin, AdGroup.ServiceControler, AdGroup.ServiceTech, AdGroup.ServiceAdmin, AdGroup.ServiceManager })]
        public HttpResponseMessage Go(Claim model)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                model.CurUserAdSid = GetCurUser().Sid;
                model.Go();
                response.Content = new StringContent($"{{\"id\":{model.Id},\"sid\":\"{model.Sid}\"}}");
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }

        [AuthorizeAd(Groups = new[] { AdGroup.SuperAdmin, AdGroup.ServiceControler, AdGroup.ServiceTech, AdGroup.ServiceAdmin, AdGroup.ServiceManager })]
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

        [AuthorizeAd(Groups = new[] { AdGroup.SuperAdmin, AdGroup.ServiceControler })]
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

        public IEnumerable<KeyValuePair<string, string>> GetWorkTypeSpecialistSelectionList(int idWorkType)
        {
            return Claim.GetWorkTypeSpecialistSelectionList(idWorkType);
        }

        public IEnumerable<ZipClaim> GetClaimZipClaimList(int idClaim)
        {
            return Claim.GetClaimZipClaimList(idClaim);
        }

        public ServiceSheet GetLastServiceSheet(int idClaim)
        {
            return Claim.GetLastServiceSheet(idClaim);
        }

        public IEnumerable<ServiceSheet> GetClaimServiceSheetList(int idClaim)
        {
            return Claim.GetClaimServiceSheetList(idClaim);
        }

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        public IHttpActionResult RemoteStateChange(int? idClaim, string stateSysName, string creatorSid, string descr = null, int? idZipClaim = null)
        {
            if (!idClaim.HasValue || String.IsNullOrEmpty(stateSysName)) return NotFound();

            Claim.RemoteStateChange(idClaim.Value, stateSysName, creatorSid, descr, idZipClaim);

            var claim = new Claim(idClaim.Value);
            claim.CurUserAdSid = creatorSid;
            //claim.Descr = descr;
            claim.Go();
            return Ok();
        }

        /// <summary>
        /// Для создания заявки на ЗИП из планирования
        /// </summary>
        /// <param name="idServiceCame"></param>
        /// <returns></returns>
        //[AuthorizeAd(AdGroup.ServiceAdmin, AdGroup.ServiceControler)]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        public IHttpActionResult RemoteCreate4ZipClaim(int? idServiceCame)
        {
            if (!idServiceCame.HasValue) return NotFound();
            int id = Claim.SaveFromServicePlan4ZipClaim(idServiceCame.Value);
            return Ok();
        }

        public IEnumerable<ServiceSheetZipItem> GetOrderedZipItemList(int claimId, int? notInserviceSheetId = null)
        {
            return Claim.GetOrderedZipItemList(claimId, notInserviceSheetId);
        }
    }
}
