using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataProvider.Models.SpeCalc;
using DataProvider.Models.Stuff;
using Objects;

namespace DataProvider.Controllers.SpeCalc
{
    public class TenderController : BaseApiController
    {
        public IEnumerable<Tender> GetManagerReport(DateTime? dateStart, DateTime? dateEnd)
        {
            if (!dateStart.HasValue || !dateEnd.HasValue) return null;//throw new ArgumentException("Не указаны дата начали или дата окончания");

            return Tender.GetManagerReport(dateStart.Value, dateEnd.Value);
            
        }
    }
}
