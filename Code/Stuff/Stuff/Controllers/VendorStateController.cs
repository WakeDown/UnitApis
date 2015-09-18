using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Stuff.Models;
using Stuff.Objects;

namespace Stuff.Controllers
{
    public class VendorStateController : BaseController
    {
        // GET: VendorState
        public ActionResult Index()
        {
            //DisplayCurUser();

            var vnd = VendorState.GetList();
            return View(vnd);

        }

        public ActionResult History(int id)
        {
           if (!CurUser.HasAccess(AdGroup.VendorStateEditor))
             return new HttpStatusCodeResult(403);
            var hislis = VendorState.GetHistoryList(id);
            return View(hislis);
        }
        
        public ActionResult GetImage(int id)
        {
            var vnd = new VendorState(id);
            byte[] imageData = vnd.Picture;
            
        return File(imageData, "image/jpeg"); // Might need to adjust the content type based on your actual image type
        }
        [HttpPost]
        public ActionResult New(VendorState vnd)
        {
            /*  var user = DisplayCurUser();
            if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");*/

            //Save employee
            try
            {
                ResponseMessage responseMessage;
                vnd.Id = null;
                bool complete = SaveVendorState(vnd, out responseMessage);
                if (!complete) throw new Exception(responseMessage.ErrorMessage);
                return RedirectToAction("index", "VendorState");
            }
            catch (Exception ex)
            {
                TempData["ServerError"] = ex.Message;
                return View("New");
            }
        }

        [HttpGet]
        public ActionResult Image(int? id)
        {
            var user = DisplayCurUser();
            /* if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");*/
            if (id.HasValue)
                {
                    var vnd = new VendorState(id.Value);
                    return View(vnd);
                }
                else
                {
                    return RedirectToAction("Index", "VendorState");
                }
            
        }

        [HttpGet]
        public ActionResult New()
        {
            if(!CurUser.HasAccess(AdGroup.VendorStateEditor))
               return new HttpStatusCodeResult(403);
            
            return View();
        }
        public void Delete(int id)
        {
            var user = DisplayCurUser();

            try
            {
                if (!user.HasAccess(AdGroup.VendorStateEditor))
                    throw new Exception("Нет доступа!");
                ResponseMessage responseMessage;
                bool complete = VendorState.Delete(id, out responseMessage);
                if (!complete) throw new Exception(responseMessage.ErrorMessage);
            }
            catch (Exception ex)
            {
                ViewData["ServerError"] = ex.Message;
            }
        }
    
        public bool SaveVendorState(VendorState vnd, out ResponseMessage responseMessage)
        {

            if (Request.Files.Count > 0 && Request.Files[0] != null && Request.Files[0].ContentLength > 0)
            {
                var file = Request.Files[0];

                string ext = Path.GetExtension(file.FileName).ToLower();

                if (ext != ".jpeg" && ext != ".jpg") throw new Exception("Формат фотографии должен быть jpeg");

                byte[] picture = null;
                using (var br = new BinaryReader(file.InputStream))
                {
                    picture = br.ReadBytes(file.ContentLength);
                }
                vnd.Picture = picture;
            }
            //var chkCreateAdUser = Request.Form["chkCreateAdUser"];
            //bool createAdUser = chkCreateAdUser != "false";
            bool complete = vnd.Save(out responseMessage);
            return complete;
        }
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            var user = DisplayCurUser();
           if (!user.HasAccess(AdGroup.VendorStateEditor))
                return new HttpStatusCodeResult(403);

            if (id.HasValue)
            {
                var vnd = new VendorState(id.Value);
                return View(vnd);
            }
            else
            {
                return View("New");
            }
        }
        [HttpPost]
        public ActionResult Edit(VendorState vnd)
        {
            /*var user = DisplayCurUser();
            if (!user.UserCanEdit()) return RedirectToAction("AccessDenied", "Error");*/

            //Save employee
            try
            {
                ResponseMessage responseMessage;
                bool complete = SaveVendorState(vnd, out responseMessage);
                if (!complete) throw new Exception(responseMessage.ErrorMessage);
                return RedirectToAction("Index", "VendorState");
            }
            catch (Exception ex)
            {
                TempData["ServerError"] = ex.Message;
                return View("Edit", vnd);
            }
        }

    }
}