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
            string errorMessage = String.Empty;
            try
            {
                bool complete = dep.Save(out errorMessage);
                if (!complete)throw new Exception(errorMessage);
                return RedirectToAction("Edit", "Department", new { id = dep.Id });
            }
            catch (Exception ex)
            {
                ViewData["ServerError"] = ex.Message;
                return View("New", dep);
            }
        }

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
	}
}