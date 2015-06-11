using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Stuff.Helpers;
using Stuff.Models;
using Stuff.Objects;

namespace Stuff.Controllers
{
    public class EmployeeController : BaseController
    {
        public ActionResult Index(int? id)
        {
            AdUser curUser = GetCurUser();
            if (curUser == new AdUser()) return View("AccessDeny");
            ViewBag.CurUser = curUser;

            if (id.HasValue && id.Value > 0)
            {
                var emp = new Employee(id.Value);

                return View(emp);
            }
            else
            {
                return View("Error");
            }
        }

        public ActionResult Edit()
        {
            AdUser curUser = GetCurUser();
            if (curUser == new AdUser()) return View("AccessDeny");
            ViewBag.CurUser = curUser;

            return View();
        }

        [HttpGet]
        public ActionResult New()
        {
            AdUser curUser = GetCurUser();
            if (curUser == new AdUser()) return View("AccessDeny");
            ViewBag.CurUser = curUser;

            var emp = new Employee();

            return View(emp);
        }

        [HttpPost]
        public ActionResult New(Employee emp)
        {
            AdUser curUser = GetCurUser();
            if (curUser == new AdUser()) return View("AccessDeny");
            ViewBag.CurUser = curUser;

            if (!ModelState.IsValid) return View("New");
            var photo = Request.Files;
            //Save employee
            try
            {
                emp.Save();
               return RedirectToAction("Index","Employee", new {id=emp.Id});
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = MessageHelper.ErrorMessage(ex.Message);
                return View();
            }
        }

        public ActionResult List()
        {
            return View();
        }
    }
}