using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Stuff.Models;
using Stuff.Objects;

namespace Stuff.Controllers
{
    public class VendorController : BaseController
    {
        public ActionResult Index()
        {
            if(!CurUser.HasAccess(AdGroup.VendorStateEditor))
            return new HttpStatusCodeResult(403);
            return View();
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(Vendor vnd)
        {

            var user = DisplayCurUser();
           // if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");

            try
            {
                ResponseMessage responseMessage;
                bool complete = vnd.Save(out responseMessage);
                if (!complete) throw new Exception(responseMessage.ErrorMessage);
                TempData["ServerSuccess"] = "Вендор успешно добавлен";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ServerError"] = ex.Message;
                return View("Index", vnd);
            }
        }

        [HttpPost]
        public void Delete(int id)
        {
            var user = DisplayCurUser();
      //      if (!user.UserCanEdit()) RedirectToAction("AccessDenied", "Error");
            try
            {
                ResponseMessage responseMessage;
                bool complete = Vendor.Delete(id, out responseMessage);
                if (!complete) throw new Exception(responseMessage.ErrorMessage);
            }
            catch (Exception ex)
            {
                TempData["ServerError"] = ex.Message;
            }
            
        }
	}
}