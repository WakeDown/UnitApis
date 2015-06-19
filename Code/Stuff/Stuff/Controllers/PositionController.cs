using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Stuff.Models;
using Stuff.Objects;

namespace Stuff.Controllers
{
    public class PositionController : BaseController
    {
        //
        // GET: /Position/
        public ActionResult Index()
        {
            DisplayCurUser();
            return View();
        }
        [HttpPost]
        public ActionResult Index(Position pos)
        {
            DisplayCurUser();

            //Save department
            try
            {
                ResponseMessage responseMessage;
                bool complete = pos.Save(out responseMessage);
                if (!complete) throw new Exception(responseMessage.ErrorMessage);

                return View("Index");
            }
            catch (Exception ex)
            {
                ViewData["ServerError"] = ex.Message;
                return View("Index", pos);
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
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