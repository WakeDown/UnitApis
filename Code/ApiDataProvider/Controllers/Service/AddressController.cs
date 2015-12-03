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
    public class AddressController : BaseApiController
    {
        public IEnumerable<Address> GetList(int? idContractor = null, int? idContract = null, int? idDevice = null, string addrName = null)
        {
            return Address.GetList(idContractor, idContract, idDevice, addrName);
        }

        public IEnumerable<KeyValuePair<string, string>> GetSelectionList(int? idContractor = null, int? idContract = null, int? idDevice = null, string addrName = null)
        {
            return Address.GetSelectionList(idContractor, idContract, idDevice, addrName);
        }
    }
}
