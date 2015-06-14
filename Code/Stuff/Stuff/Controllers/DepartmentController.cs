using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Stuff.Helpers;
using Stuff.Models;
using Stuff.Objects;

namespace Stuff.Controllers
{
    public class DepartmentController : BaseController
    {
        //
        // GET: /OrgStructure/
        public ActionResult Index()
        {
            AdUser curUser = GetCurUser();
            if (curUser == new AdUser()) return View("AccessDeny");
            ViewBag.CurUser = curUser;

            var deps = Department.GetOrgStructure();

            return View(deps);
        }

        [HttpGet]
        public ActionResult New()
        {
            AdUser curUser = GetCurUser();
            if (curUser == new AdUser()) return View("AccessDeny");
            ViewBag.CurUser = curUser;

            return View();
        }
        [HttpPost]
        public ActionResult New(Department dep)
        {
            AdUser curUser = GetCurUser();
            if (curUser == new AdUser()) return View("AccessDeny");
            ViewBag.CurUser = curUser;

            //Save department
            try
            {
                ResponseMessage responseMessage;
                bool complete = dep.Save(out responseMessage);
                if (!complete) throw new Exception(responseMessage.ErrorMessage);

                return RedirectToAction("Edit", "Department", new { id = responseMessage.Id });
            }
            catch (Exception ex)
            {
                ViewData["ServerError"] = ex.Message;
                return View("New", dep);
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            AdUser curUser = GetCurUser();
            if (curUser == new AdUser()) return View("AccessDeny");
            ViewBag.CurUser = curUser;

            if (id.HasValue)
            {
                var dep = new Department(id.Value);
                return View(dep);
            }
            else
            {
                return View("New");
            }
        }
        [HttpPost]
        public ActionResult Edit(Department dep)
        {
            AdUser curUser = GetCurUser();
            if (curUser == new AdUser()) return View("AccessDeny");
            ViewBag.CurUser = curUser;

            try
            {
                ResponseMessage responseMessage;
                bool complete = dep.Save(out responseMessage);
                if (!complete) throw new Exception(responseMessage.ErrorMessage);

                return RedirectToAction("Edit", "Department", new { id = responseMessage.Id });
            }
            catch (Exception ex)
            {
                ViewData["ServerError"] = ex.Message;
                return RedirectToAction("Edit", "Department", new { id = dep.Id });
            }
        }

        
        public void Delete(int? id)
        {
            try
            {
                ResponseMessage responseMessage;
                bool complete = Department.Delete(id.Value, out responseMessage);
                if (!complete) throw new Exception(responseMessage.ErrorMessage);
            }
            catch (Exception ex)
            {
                ViewData["ServerError"] = ex.Message;
            }
        }
	}
}