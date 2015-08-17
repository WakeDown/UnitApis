using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SelectPdf;
using Stuff.Models;
using Stuff.Objects;

namespace Stuff.Controllers
{
    public class ReportController : BaseController
    {
        public ActionResult GetItBudgetReport()
        {
            if (!CurUser.IsSystemUser()) return RedirectToAction("AccessDenied", "Error");

            HtmlToPdf converter = new HtmlToPdf();

            string url = Url.Action("ItBudgetView");
            var leftPartUrl = String.Format("{0}://{1}:{2}", Request.RequestContext.HttpContext.Request.Url.Scheme, Request.RequestContext.HttpContext.Request.Url.Host, Request.RequestContext.HttpContext.Request.Url.Port);
            url = String.Format("{1}{0}", url, leftPartUrl);
            PdfDocument doc = converter.ConvertUrl(url);
            MemoryStream stream = new MemoryStream();
            doc.Save(stream);
            return File(stream.ToArray(), "application/pdf");
        }
        
        public ActionResult ItBudgetView()
        {
            if (!CurUser.IsSystemUser()) return RedirectToAction("AccessDenied", "Error");

            var list = Report.GetItBudgetList();
            return View(list);
        }
    }
}