using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Stuff.Models;
using Stuff.Objects;

namespace Stuff.Controllers
{
    public class OrganizationController : BaseController
    {
        //
        // GET: /Organization/
        public ActionResult Index()
        {
            var user = DisplayCurUser();
            if (!user.UserIsPersonalManager()) return RedirectToAction("AccessDenied", "Error");
            return View();
        }

        [HttpPost]
        public ActionResult Index(Organization org)
        {
            var user = DisplayCurUser();
            if (!user.UserIsPersonalManager()) return RedirectToAction("AccessDenied", "Error");
                    
            //Save department
            try
            {
                ResponseMessage responseMessage;
                org.Creator = new Employee() { AdSid = GetCurUser().Sid };
                bool complete = org.Save(out responseMessage);
                if (!complete) throw new Exception(responseMessage.ErrorMessage);

                return View("Index", new Organization());
            }
            catch (Exception ex)
            {
                ViewData["ServerError"] = ex.Message;
                return View("Index", org);
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            var user = DisplayCurUser();
            if (!user.UserIsPersonalManager()) RedirectToAction("AccessDenied", "Error");

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