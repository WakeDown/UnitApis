using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataProvider.Models.Service;
using Objects;

namespace DataProvider.Controllers.Global
{
    public class ContractorController : BaseApiController
    {
        public IEnumerable<Contractor> GetServiceList(int? idContractor = null, string contractorName = null, int? idContract = null, string contractNumber = null, int? idDevice = null, string deviceName = null)
        {
            return Contractor.GetServicePlanList(idContractor, contractorName, idContract, contractNumber, idDevice, deviceName);
        }

        public IEnumerable<Contractor> GetServiceClaimFilterList()
        {
            return Contractor.GetServiceClaimFilterList();
        }
    }
}
