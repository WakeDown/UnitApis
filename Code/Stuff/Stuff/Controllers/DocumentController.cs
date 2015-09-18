using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using SelectPdf;
//using SelectPdf;
using Stuff.Helpers;
using Stuff.Models;
using Stuff.Objects;
//using Select.HtmlToPdf
//using Document = Stuff.Models.Document;

namespace Stuff.Controllers
{
    public class DocumentController : BaseController
    {
        public ActionResult My()
        {
            var user = DisplayCurUser();
            var list = Document.GetMyList();
            return View(list);
        }

        public JsonResult DeleteDocument(int? id)
        {
            if (id.HasValue)
            {
                if (!CurUser.UserCanEdit()) return Json(new {errorMessage = "Отказано в доступе"});
                try
                {
                    ResponseMessage responseMessage;
                    bool complete = Document.Delete(id.Value, out responseMessage);
                    if (!complete) throw new Exception(responseMessage.ErrorMessage);
                }
                catch (Exception ex)
                {
                    ViewData["ServerError"] = ex.Message;
                    return Json(new { errorMessage = ex.Message });
                }
            }
            else
            {
                return Json(new { errorMessage = "Не указан документ" });
            }
            return Json(new { id = id });
        }

        //public ActionResult GetDocumentData(string sid)
        //{
        //    return File()
        //    //return File(cert.File, "text/plain", cert.FileName);
        //}

        public ActionResult Links(int? idDocument)
        {
            var user = DisplayCurUser();
            if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");

                return View();
        }

        [HttpPost]
        public JsonResult GetDocMeetLinks(int? idDocument)
        {
            var user = DisplayCurUser();
            if (!user.UserCanEdit()) return Json(new { errorMessage = "Отказано в доступе" });
            if (idDocument.HasValue)
            {
                var list = DocMeetLink.GetList(idDocument.Value);
                return Json(list);
            }
            else
            {
                return Json(new { errorMessage = "Не указан документ" });
            }
        }

        [HttpPost]
        public JsonResult SaveDocMeetLink(int? idDocument, int? idDepartment, int? idPosition, int? idEmployee)
        {
            var user = DisplayCurUser();
            if (!user.UserCanEdit()) return Json(new { errorMessage = "Отказано в доступе" });
            if (idDocument.HasValue && (idDepartment.HasValue || idPosition.HasValue || idEmployee.HasValue))
            {
                try
                {
                    ResponseMessage responseMessage;
                    bool complete = new DocMeetLink() { IdDocument = idDocument.Value, IdDepartment = idDepartment, IdPosition = idPosition, IdEmployee = idEmployee}.Save(out responseMessage);
                    if (!complete) throw new Exception(responseMessage.ErrorMessage);

                }
                catch (Exception ex)
                {
                    ViewData["ServerError"] = ex.Message;
                    return Json(new { errorMessage = ex.Message });
                }
            }
            //else
            //{
            //    return Json(new { errorMessage = "укажите idDocument" });
            //}
            return Json(new { });
        }

        [HttpPost]
        public JsonResult CloseDocMeetLink(int? idDocument, int? idDepartment, int? idPosition, int? idEmployee)
        {
            var user = DisplayCurUser();
            if (!user.UserCanEdit()) return Json(new { errorMessage = "Отказано в доступе" });
            if (idDocument.HasValue && (idDepartment.HasValue || idPosition.HasValue || idEmployee.HasValue))
            {
                try
                {
                    ResponseMessage responseMessage;
                    bool complete = DocMeetLink.Delete(idDocument.Value, idDepartment, idPosition, idEmployee, out responseMessage);
                    if (!complete) throw new Exception(responseMessage.ErrorMessage);

                }
                catch (Exception ex)
                {
                    ViewData["ServerError"] = ex.Message;
                    return Json(new { errorMessage = ex.Message });
                }
            }
            //else
            //{
            //    return Json(new { errorMessage = "укажите idDocument" });
            //}
            return Json(new { });
        }

        [HttpPost]
        public ActionResult SaveFile()
        {
            if (!CurUser.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");
            int id = 0;
            if (Request.Files.Count > 0)
            {
                try
                {
                    string noPdf = String.Empty;
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        var file = Request.Files[i];
                        if (Path.GetExtension(file.FileName) != ".pdf")
                        {
                            noPdf = "Файлы не PDF формата не были добавлены";
                            continue;
                        }

                        byte[] fileData = null;
                        using (var br = new BinaryReader(file.InputStream))
                        {
                            fileData = br.ReadBytes(file.ContentLength);
                        }

                        var doc = new Document() { Data = fileData, Name = file.FileName };
                        ResponseMessage responseMessage;
                        bool complete = doc.Save(out responseMessage);
                        if (!complete) throw new Exception(responseMessage.ErrorMessage);
                        TempData["noPdf"] = noPdf;
                    }
                }
                catch (Exception ex)
                {
                    ViewData["ServerError"] = ex.Message;
                    return View("Links");
                }
            }
            return RedirectToAction("Links", id);
            //return RedirectToAction("Index", "Calc", new { claimId = Request.QueryString["claimId"] });
        }

        [HttpGet]
        public ActionResult StatementRestFewHours()
        {
            var user = DisplayCurUser();
            return View("StatementFewHours", new StatementRestFewHours(user.Sid));
        }

        [HttpPost]
        public ActionResult StatementRestFewHours(StatementRestFewHours data)
        {
            try
            {
                data.Configure();
            }
            catch (Exception ex)
            {
                TempData["ServerError"] = ex.Message;
                return View("StatementFewHours", data);
            }

            HtmlToPdf converter = new HtmlToPdf();

            string url = Url.Action("StatementRestHours", new { sid = data.SidEmployee, dateRest = data.DateRest, hourStart = data.HourStart, hoursCount = data.HoursCount, cause = data.Cause });
            var leftPartUrl = String.Format("{0}://{1}:{2}", Request.RequestContext.HttpContext.Request.Url.Scheme, Request.RequestContext.HttpContext.Request.Url.Host, Request.RequestContext.HttpContext.Request.Url.Port);
            url = String.Format("{1}{0}", url, leftPartUrl);
            PdfDocument doc = converter.ConvertUrl(url);
            MemoryStream stream = new MemoryStream();
            doc.Save(stream);
            return File(stream.ToArray(), "application/pdf");

            //return View("StatementNoOf", data);
        }

        public ActionResult StatementRestHours(string sid, DateTime? dateRest, DateTime? hourStart, int? hoursCount, string cause)
        {
            var data = new StatementRestFewHours();
            data.SidEmployee = sid;
            data.DateRest = dateRest.Value;
            data.HourStart = hourStart.Value;
            data.HoursCount = hoursCount.Value;
            data.Cause = cause;
            data.Configure();

            return View("StatementNoOf", data);
        }

        [HttpGet]
        public ActionResult StatementRestFewDays()
        {
            return View("StatementFewDays", new StatementRestFewDays(CurUser.Sid));
        }

        [HttpPost]
        public ActionResult StatementRestFewDays(StatementRestFewDays data)
        {
            try
            {
                data.Configure();
            }
            catch (Exception ex)
            {
                TempData["ServerError"] = ex.Message;
                return View("StatementFewDays", data);
            }
            HtmlToPdf converter = new HtmlToPdf();
            
            string url = Url.Action("StatementRestDays", new{sid=data.SidEmployee, dateStart=data.DateStart, daysCount=data.DaysCount, cause=data.Cause});
            var leftPartUrl = String.Format("{0}://{1}:{2}", Request.RequestContext.HttpContext.Request.Url.Scheme, Request.RequestContext.HttpContext.Request.Url.Host, Request.RequestContext.HttpContext.Request.Url.Port);
            url = String.Format("{1}{0}", url, leftPartUrl);
            PdfDocument doc = converter.ConvertUrl(url);
            MemoryStream stream = new MemoryStream();
            doc.Save(stream);
            return File(stream.ToArray(), "application/pdf");
        }

       

        public ActionResult StatementRestDays(string sid, DateTime? dateStart, int? daysCount, string cause)
        {
            var data = new StatementRestFewDays();
            data.SidEmployee = sid;
            data.DateStart = dateStart.Value;
            data.DaysCount = daysCount.Value;
            data.Cause = cause;
            data.Configure();

            return View("Statement", data);
        }

        [HttpGet]
        public ActionResult StatementFormFired()
        {
            return View("StatementFormFired", new StatementFired(CurUser.Sid));
        }

        [HttpPost]
        public ActionResult StatementFormFired(StatementFired data)
        {
            try
            {
                data.Configure();
            }
            catch (Exception ex)
            {
                TempData["ServerError"] = ex.Message;
                return View("StatementFormFired", data);
            }
            HtmlToPdf converter = new HtmlToPdf();

            string url = Url.Action("StatementFiredPdf", new { sid = data.SidEmployee, dateFired = data.DateFired });
            var leftPartUrl = String.Format("{0}://{1}:{2}", Request.RequestContext.HttpContext.Request.Url.Scheme, Request.RequestContext.HttpContext.Request.Url.Host, Request.RequestContext.HttpContext.Request.Url.Port);
            url = String.Format("{1}{0}", url, leftPartUrl);
            PdfDocument doc = converter.ConvertUrl(url);
            MemoryStream stream = new MemoryStream();
            doc.Save(stream);
            return File(stream.ToArray(), "application/pdf");
        }

        public ActionResult StatementFiredPdf(string sid, DateTime? dateFired)
        {
            var data = new StatementFired();
            data.SidEmployee = sid;
            data.DateFired = dateFired.Value;
            data.Configure();

            return View("StatementFired", data);
        }

        [HttpGet]
        public ActionResult StatementFormRest()
        {
            return View("StatementFormRest", new StatementRest(CurUser.Sid));
        }

        [HttpPost]
        public ActionResult StatementFormRest(StatementRest data)
        {
            try
            {
                data.Configure();
            }
            catch (Exception ex)
            {
                TempData["ServerError"] = ex.Message;
                return View("StatementFormRest", data);
            }
            HtmlToPdf converter = new HtmlToPdf();

            string url = Url.Action("StatementRestPdf", new { sid = data.SidEmployee, dateStart = data.DateStart, dateEnd = data.DateEnd, daysCount=data.DaysCount });
            var leftPartUrl = String.Format("{0}://{1}:{2}", Request.RequestContext.HttpContext.Request.Url.Scheme, Request.RequestContext.HttpContext.Request.Url.Host, Request.RequestContext.HttpContext.Request.Url.Port);
            url = String.Format("{1}{0}", url, leftPartUrl);
            PdfDocument doc = converter.ConvertUrl(url);
            MemoryStream stream = new MemoryStream();
            doc.Save(stream);
            return File(stream.ToArray(), "application/pdf");
        }

        public ActionResult StatementRestPdf(string sid, DateTime? dateStart, DateTime? dateEnd, int? daysCount)
        {
            var data = new StatementRest();
            data.SidEmployee = sid;
            data.DateStart = dateStart.Value;
            data.DateEnd = dateEnd.Value;
            data.DaysCount = daysCount.Value;
            data.Configure();

            return View("StatementRest", data);
        }

        public ActionResult DocumentHeader(int? idOrg)
        {
            if (!idOrg.HasValue) return HttpNotFound();
            var org = new Organization(idOrg.Value);

            return PartialView(org);
        }

        [HttpGet]
        public ActionResult DocumentEmployeeList()
        {
            var user = DisplayCurUser();
            if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");

            HtmlToPdf converter = new HtmlToPdf();

            string url = Url.Action("DocEmployeeList");
            var leftPartUrl = String.Format("{0}://{1}:{2}", Request.RequestContext.HttpContext.Request.Url.Scheme, Request.RequestContext.HttpContext.Request.Url.Host, Request.RequestContext.HttpContext.Request.Url.Port);
            url = String.Format("{1}{0}", url, leftPartUrl);
            PdfDocument doc = converter.ConvertUrl(url);
            MemoryStream stream = new MemoryStream();
            doc.Save(stream);
            return File(stream.ToArray(), "application/pdf");

            //return View("DocumentEmployeeList", new DocumentEmployeeList());
        }

        public ActionResult DocEmployeeList()
        {
            return View("DocumentEmployeeList", new DocumentEmployeeList());
        }

        public ActionResult Statements()
        {
            return View();
        }
    }
}