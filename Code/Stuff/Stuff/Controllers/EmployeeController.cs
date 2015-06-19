using System;
using System.Collections.Generic;
using System.IO;
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
        private void DisplayCurUser()
        {
            AdUser curUser = GetCurUser();
            if (curUser == new AdUser()) RedirectToAction("AccessDeny");
            ViewBag.CurUser = curUser;
        }

        public ActionResult AccessDeny()
        {
            return View("AccessDeny");
        }

        public ActionResult Index(int? id)
        {
            DisplayCurUser();

            if (id.HasValue && id.Value > 0)
            {
                var emp = new Employee(id.Value, true);

                return View(emp);
            }
            else
            {
                return View("Error");
            }
        }
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            DisplayCurUser();

            if (id.HasValue)
            {
                var emp = new Employee(id.Value, true);
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
            DisplayCurUser();

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
        public ActionResult New()
        {
            DisplayCurUser();

            var emp = new Employee();

            return View(emp);
        }

        [HttpPost]
        public ActionResult New(Employee emp)
        {
            DisplayCurUser();

            
            //Save employee
            try
            {
                ResponseMessage responseMessage;
                bool complete = SaveEmployee(emp, out responseMessage);
                if (!complete) throw new Exception(responseMessage.ErrorMessage);
                return RedirectToAction("Index", "Employee", new { id = responseMessage.Id });
            }
            catch(Exception ex)
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
        public JsonResult GenEmailAddressByName(string surname, string name)
        {
            string email = String.Empty;
            try
            {
                email = Ad.GenEmailAddressByName(surname, name);

            }
            catch (Exception ex)
            {
                ViewData["ServerError"] = ex.Message;
            }

            return Json(new {address = email});
        }
    }
}