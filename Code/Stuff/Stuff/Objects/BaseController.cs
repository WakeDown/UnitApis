using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Stuff.Models;

namespace Stuff.Objects
{
    public class BaseController:Controller
    {
        //protected ActionResult ViewPdf(string viewName, object model)
        //{
        //    // Create the iTextSharp document.
        //    var doc = new iTextSharp.text.Document();
        //    // Set the document to write to memory.
        //    MemoryStream memStream = new MemoryStream();
        //    PdfWriter writer = PdfWriter.GetInstance(doc, memStream);
        //    writer.CloseStream = false;
        //    doc.Open();

        //    // Render the view xml to a string, then parse that string into an XML dom.
        //    string xmltext = this.RenderActionResultToString(this.View(viewName, model));
        //    XmlDocument xmldoc = new XmlDocument();
        //   xmltext= xmltext.Replace("&laquo;", "\"").Replace("&raquo;", "\"");
        //    xmldoc.InnerXml = xmltext.Trim();

        //    // Parse the XML into the iTextSharp document.
        //    ITextHandler textHandler = new ITextHandler(doc);
        //    textHandler.Parse(xmldoc);

        //    // Close and get the resulted binary data.
        //    doc.Close();
        //    byte[] buf = new byte[memStream.Position];
        //    memStream.Position = 0;
        //    memStream.Read(buf, 0, buf.Length);

        //    // Send the binary data to the browser.
        //    return new BinaryContentResult(buf, "application/pdf");
        //}

        protected string RenderActionResultToString(ActionResult result)
        {
            // Create memory writer.
            var sb = new StringBuilder();
            var memWriter = new StringWriter(sb);

            // Create fake http context to render the view.
            var fakeResponse = new HttpResponse(memWriter);
            var fakeContext = new HttpContext(System.Web.HttpContext.Current.Request,
                fakeResponse);
            var fakeControllerContext = new ControllerContext(
                new HttpContextWrapper(fakeContext),
                this.ControllerContext.RouteData,
                this.ControllerContext.Controller);
            var oldContext = System.Web.HttpContext.Current;
            System.Web.HttpContext.Current = fakeContext;

            // Render the view.
            result.ExecuteResult(fakeControllerContext);

            // Restore old context.
            System.Web.HttpContext.Current = oldContext;

            // Flush memory and return output.
            memWriter.Flush();
            return sb.ToString();
        }

        protected AdUser CurUser { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            DisplayCurUser();
            base.OnActionExecuting(filterContext);
        }

        private static NetworkCredential nc = GetAdUserCredentials();
        public static NetworkCredential GetAdUserCredentials()
        {
            string accUserName = @"UN1T\adUnit_prog";
            string accUserPass = "1qazXSW@";

            string domain = "UN1T";//accUserName.Substring(0, accUserName.IndexOf("\\"));
            string name = "adUnit_prog";//accUserName.Substring(accUserName.IndexOf("\\") + 1);

            NetworkCredential nc = new NetworkCredential(name, accUserPass, domain);

            return nc;
        }

        [NonAction]
        public AdUser GetCurUser()
        {
            AdUser user = new AdUser();
            try
            {
                using (WindowsImpersonationContextFacade impersonationContext
                    = new WindowsImpersonationContextFacade(
                        nc))
                {
                    var wi = (WindowsIdentity) base.User.Identity;
                    if (wi.User != null)
                    {
                        var domain = new PrincipalContext(ContextType.Domain);
                        string sid = wi.User.Value;
                        user.Sid = sid;
                        var login = wi.Name.Remove(0, wi.Name.IndexOf("\\", StringComparison.CurrentCulture) + 1);
                        user.Login = login;
                        var userPrincipal = UserPrincipal.FindByIdentity(domain, login);
                        if (userPrincipal != null)
                        {
                            var mail = userPrincipal.EmailAddress;
                            var name = userPrincipal.DisplayName;
                            user.Email = mail;
                            user.FullName = name;
                            //user.AdGroups = new List<AdGroup>();
                            //var wp = new WindowsPrincipal(wi);
                            //foreach (var role in AdUserGroup.GetList())
                            //{
                            //    var grpSid = new SecurityIdentifier(role.Sid);
                            //    if (wp.IsInRole(grpSid))
                            //    {
                            //        user.AdGroups.Add(role.Group);
                            //    }
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return user;
        }

        protected AdUser DisplayCurUser()
        {
            CurUser = GetCurUser();
            if (CurUser == new AdUser()) RedirectToAction("AccessDenied", "Error");
            ViewBag.CurUser = CurUser;
            return CurUser;
        }
    }
}