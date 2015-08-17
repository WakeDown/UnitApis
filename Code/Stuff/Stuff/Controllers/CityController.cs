using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Stuff.Models;
using Stuff.Objects;

namespace Stuff.Controllers
{
    public class CityController : BaseController
    {
        public ActionResult Index()
        {
            var user = DisplayCurUser();
            if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");
            return View();
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(City cit)
        {

            var user = DisplayCurUser();
            if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");

            try
            {
                ResponseMessage responseMessage;
                bool complete = cit.Save(out responseMessage);
                if (!complete) throw new Exception(responseMessage.ErrorMessage);
                TempData["ServerSuccess"] = "Город успешно добавлен";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ServerError"] = ex.Message;
                return View("Index", cit);
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
                bool complete = City.Delete(id, out responseMessage);
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