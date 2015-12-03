using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataProvider.Models.Service;
using Objects;

namespace DataProvider.Controllers.Service
{
    public class ClaimStateController : BaseApiController
    {
        public IEnumerable<ClaimState> GetFilterList()
        {
            return ClaimState.GetFilterList();
        }

        public IEnumerable<ClaimStateGroup> GetGroupFilterList(string servAdminSid = null, string servManagerSid = null, string servEngeneerSid = null, DateTime? dateStart = null, DateTime? dateEnd = null, int? topRows = null, string managerSid = null, string techSid = null, string serialNum = null, int? idDevice = null, bool? activeClaimsOnly = false, int? idClaimState = null, int? clientId = null, string clientSdNum = null, int? claimId = null, string deviceName = null, int? pageNum = null, string groupStates = null, string address = null)
        {
            return ClaimStateGroup.GetFilterList(GetCurUser(), servAdminSid, servEngeneerSid, dateStart, dateEnd, topRows, managerSid, techSid, serialNum, idDevice, activeClaimsOnly, idClaimState, clientId, clientSdNum, claimId: claimId, deviceName: deviceName, pageNum: pageNum, groupStates: groupStates, address: address, servManagerSid: servManagerSid);
        }
    }
}
