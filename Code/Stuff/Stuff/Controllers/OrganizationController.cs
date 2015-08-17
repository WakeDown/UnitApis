using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Stuff.Models;
using Stuff.Objects;

namespace Stuff.Controllers
{
    public class OrganizationController : BaseController
    {
        public ActionResult Index()
        {
            var user = DisplayCurUser();
            if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");
            return View(Organization.GetList());
        }
        
        [HttpGet]
        public ActionResult New()
        {
            var user = DisplayCurUser();
            if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");
            return View();
        }

        [HttpPost]
        public ActionResult New(Organization org)
        {
            var user = DisplayCurUser();
            if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");

            try
            {
                ResponseMessage responseMessage;
                bool complete = SaveOrganization(org, out responseMessage);
                if (!complete) throw new Exception(responseMessage.ErrorMessage);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ServerError"] = ex.Message;
                return View("New", org);
            }
        }

        [NonAction]
        public bool SaveOrganization(Organization org, out ResponseMessage responseMessage)
        {

            if (Request.Files.Count > 0)
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    if (file != null)
                    {
                        byte[] data = null;
                        using (var br = new BinaryReader(file.InputStream))
                        {
                            data = br.ReadBytes(file.ContentLength);
                        }
                        org.StateImages.Add(new OrgStateImage(data));
                    }
                }
            }
            bool complete = org.Save(out responseMessage);
            return complete;
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            var user = DisplayCurUser();
            if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");
            return View(new Organization(id.Value));
        }

        [HttpPost]
        public ActionResult Edit(Organization org)
        {
            var user = DisplayCurUser();
            if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");

            try
            {
                ResponseMessage responseMessage;
                bool complete = SaveOrganization(org, out responseMessage);
                if (!complete) throw new Exception(responseMessage.ErrorMessage);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ServerError"] = ex.Message;
                return View("Edit", org);
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            var user = DisplayCurUser();
            if (!user.UserCanEdit()) RedirectToAction("AccessDenied", "Error");

            try
            {
                ResponseMessage responseMessage;
                bool complete = Organization.Delete(id, out responseMessage);
                if (!complete) throw new Exception(responseMessage.ErrorMessage);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            return null;
        }
	}
}