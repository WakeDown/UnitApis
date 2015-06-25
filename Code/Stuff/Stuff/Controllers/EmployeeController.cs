using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
            DisplayCurUser();

            if (id.HasValue && id.Value > 0)
            {
                var emp = new Employee(id.Value);

                return View(emp);
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            var user = DisplayCurUser();
            if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");

            if (id.HasValue)
            {
                var emp = new Employee(id.Value);
                return View(emp);
            }
            else
            {
                return View("New");
            }
        }

        [HttpPost]
        public ActionResult Edit(Employee emp)
        {
            var user = DisplayCurUser();
            if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");

            //Save employee
            try
            {
                ResponseMessage responseMessage;
                bool complete = SaveEmployee(emp, out responseMessage);
                if (!complete) throw new Exception(responseMessage.ErrorMessage);
                return RedirectToAction("Index", "Employee", new { id = responseMessage.Id });
            }
            catch (Exception ex)
            {
                ViewData["ServerError"] = ex.Message;
                return View("Edit", emp);
            }
        }

        [HttpGet]
        public ActionResult New(bool? test)
        {
            var user = DisplayCurUser();
            if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");

            var emp = new Employee() {HasAdAccount = true};

            if (test.HasValue && test.Value)
            {
                emp.Name = "Тест";
                emp.Surname = "Тестов";
                emp.Patronymic = "Тестович";
                emp.MobilNum = "9536001000";
                emp.WorkNum = "111";
                emp.Email = "test.testov@unitgroup.ru";
                emp.City = new City() { Id = 26 };
                emp.Organization = new Organization() { Id = 45 };
                emp.Position = new Position() { Id = 46 };
                emp.PositionOrg = new Position() { Id = 46 };
                emp.Department = new Department() { Id = 10 };
            }

            return View(emp);
        }

        [HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "Create")]
        public ActionResult New(Employee emp)
        {
            var user = DisplayCurUser();
            if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");

            //Save employee
            try
            {
                ResponseMessage responseMessage;
                bool complete = SaveEmployee(emp, out responseMessage);
                if (!complete) throw new Exception(responseMessage.ErrorMessage);
                return RedirectToAction("Index", "Employee", new { id = responseMessage.Id });
            }
            catch (Exception ex)
            {
                ViewData["ServerError"] = ex.Message;
                return View("New", emp);
            }
        }

        [NonAction]
        public bool SaveEmployee(Employee emp, out ResponseMessage responseMessage)
        {

            if (Request.Files.Count > 0 && Request.Files[0] != null && Request.Files[0].ContentLength > 0)
            {
                var file = Request.Files[0];

                string ext = Path.GetExtension(file.FileName).ToLower();

                if (ext != ".png" && ext != ".jpeg" && ext != ".jpg" && ext != ".gif") throw new Exception("Формат фотографии должен быть .png .jpeg .gif");

                byte[] picture = null;
                using (var br = new BinaryReader(file.InputStream))
                {
                    picture = br.ReadBytes(file.ContentLength);
                }
                emp.Photo = picture;
            }
            emp.Creator = new Employee(){AdSid = GetCurUser().Sid};
            //var chkCreateAdUser = Request.Form["chkCreateAdUser"];
            //bool createAdUser = chkCreateAdUser != "false";
            bool complete = emp.Save(out responseMessage);
            return complete;
        }

        public ActionResult List()
        {
            DisplayCurUser();

            var emps = Employee.GetList();
            return View(emps);
        }

        public JsonResult GetDepartmentChief(int idDepartment)
        {
            string result = "--отсутствует--";
            var dep = new Department(idDepartment);
            if (dep.Chief != null && dep.Chief.Id > 0)
            {
                result = dep.Chief.DisplayName;
            }

            return Json(new { name = result });
        }

        [HttpPost]
        public void Delete(int id)
        {
            var user = DisplayCurUser();
            if (!user.UserCanEdit()) RedirectToAction("AccessDenied", "Error");
            try
            {
                ResponseMessage responseMessage;
                bool complete = Employee.Delete(id, out responseMessage);
                if (!complete) throw new Exception(responseMessage.ErrorMessage);
            }
            catch (Exception ex)
            {
                ViewData["ServerError"] = ex.Message;
            }
        }

        [HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "GenEmail")]
        public ActionResult GenEmailAddressByName(string surname, string name)
        {
            var user = DisplayCurUser();
            if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");
            //DisplayCurUser();
            string email = String.Empty;
            try
            {
                email = Ad.GenEmailAddressByName(surname, name);

            }
            catch (Exception ex)
            {
                ViewData["ServerError"] = ex.Message;
            }

            return Json(new { address = email });
            //return PartialView("New", new Employee() {Email=email});
        }

        ////public ActionResult GetPhoto(int? id)
        ////{


        ////    return base.File()
        ////}
    }
}