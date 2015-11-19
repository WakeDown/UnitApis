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
    public class ContractController : BaseApiController
    {
        public IEnumerable<Contract> GetList(int? idContractor = null, string contractorName = null, int? idContract = null, string contractNumber = null, int? idDevice = null, string deviceName = null)
        {
            return Contract.GetList(idContractor, contractorName, idContract, contractNumber, idDevice, deviceName);
        }

        public IEnumerable<KeyValuePair<int, string>> GetSelectionList(int? idContractor = null)
        {
            return Contract.GetSelectionList(idContractor);
        }
    }
}
