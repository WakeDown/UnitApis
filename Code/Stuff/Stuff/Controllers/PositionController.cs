using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Stuff.Models;
using Stuff.Objects;

namespace Stuff.Controllers
{
    public class PositionController : BaseController
    {
        public ActionResult Index()
        {
            var user = DisplayCurUser();
            if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");
            return View(Position.GetList());
        }
        //[HttpPost]
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Index(Position pos)
        //{

        //    var user = DisplayCurUser();
        //    if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");

        //    try
        //    {
        //        ResponseMessage responseMessage;
        //        //pos.Creator = new Employee() { AdSid = GetCurUser().Sid };
        //        bool complete = pos.Save(out responseMessage);
        //        if (!complete) throw new Exception(responseMessage.ErrorMessage);
        //        TempData["ServerSuccess"] = "Должность успешно добавлена";
        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ServerError"] = ex.Message;
        //        return View("Index", pos);
        //    }
        //}

        [HttpGet]
        public ActionResult New()
        {
            var user = DisplayCurUser();
            if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");
            return View();
        }

        [HttpPost]
        public ActionResult New(Position model)
        {
            var user = DisplayCurUser();
            if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");

            try
            {
                ResponseMessage responseMessage;
                bool complete = model.Save(out responseMessage);
                if (!complete) throw new Exception(responseMessage.ErrorMessage);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ServerError"] = ex.Message;
                return View("New", model);
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            var user = DisplayCurUser();
            if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");
            return View(new Position(id.Value));
        }

        [HttpPost]
        public ActionResult Edit(Position model)
        {
            var user = DisplayCurUser();
            if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");

            try
            {
                ResponseMessage responseMessage;
                bool complete = model.Save(out responseMessage);
                if (!complete) throw new Exception(responseMessage.ErrorMessage);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ServerError"] = ex.Message;
                return View("Edit", model);
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
                bool complete = Position.Delete(id, out responseMessage);
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