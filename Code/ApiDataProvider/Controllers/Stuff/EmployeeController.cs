using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Channels;
using System.Web.Http;
using System.Web.Http.OData;
using DataProvider.Helpers;
using DataProvider.Models.Stuff;
using DataProvider.Objects;
using Objects;

namespace DataProvider.Controllers.Stuff
{
    public class EmployeeController : BaseApiController
    {
        //[HttpGet]
        //public void RefillManager()
        //{
        //    Employee.RefillManager();
        //}
        
        public IEnumerable<Employee> GetList(int? idDepartment = null, int? idCity = null, bool showHidden = true)
        {
            bool userCanViewHiddenEmps = AdHelper.UserInGroup(GetCurUser().User, AdGroup.PersonalManager, AdGroup.SuperAdmin);
            return Employee.GetList(idDepartment, false, idCity, null, userCanViewHiddenEmps, showHidden);
        }

        public IEnumerable<Employee> GetFiredList(int? idDepartment = null)
        {
            bool userCanViewHiddenDeps = AdHelper.UserInGroup(GetCurUser().User, AdGroup.PersonalManager, AdGroup.SuperAdmin);
            return Employee.GetFiredList(idDepartment, userCanViewHiddenDeps);
        }

        public IEnumerable<Employee> GetDecreeList(int? idDepartment = null)
        {
            bool userCanViewHiddenEmps = AdHelper.UserInGroup(GetCurUser().User, AdGroup.PersonalManager, AdGroup.SuperAdmin);
            return Employee.GetDecreeList(idDepartment, userCanViewHiddenEmps);
        }

        public IEnumerable<Employee> GetStNewbieList(int? idDepartment = null)
        {
            bool userCanViewHiddenEmps = AdHelper.UserInGroup(GetCurUser().User, AdGroup.PersonalManager, AdGroup.SuperAdmin);
            return Employee.GetStNewbieList(idDepartment, userCanViewHiddenEmps);
        }

        public Employee Get(int id)
        {
            bool userCanViewHiddenEmps = AdHelper.UserInGroup(GetCurUser().User, AdGroup.PersonalManager, AdGroup.SuperAdmin);
            var emp = new Employee(id);
            if (emp.IsHidden && !userCanViewHiddenEmps) return new Employee();
            return emp;
        }

        public Employee Get(string adSid)
        {
            bool userCanViewHiddenEmps = AdHelper.UserInGroup(GetCurUser().User, AdGroup.PersonalManager, AdGroup.SuperAdmin);
            var emp = new Employee(adSid);
            if (emp.IsHidden && !userCanViewHiddenEmps) return new Employee();
            return emp;
        }

        public Employee GetDirector()
        {
            var emp = Employee.GetDirector();
            return emp;
        }

        public Employee GetDepartmentDirector(string employeeSid)
        {
            var emp = Employee.GetDepartmentDirector(employeeSid);
            return emp;
        }

        [AllowAnonymous]
        public HttpResponseMessage GetPhoto(string adSid)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();

            var emp = new Employee(adSid, true);
            if (emp.Photo == null || (emp.Photo != null && emp.Photo.Length == 0))
            {
                if (emp.Male)
                {
                    emp.Photo = File.ReadAllBytes(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/images/no_photo_male.jpg"));
                }
                else { emp.Photo = File.ReadAllBytes(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/images/no_photo_female.png")); }
            }

            httpResponseMessage.Content = new ByteArrayContent(emp.Photo);

            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("image/gif");
            httpResponseMessage.StatusCode = HttpStatusCode.OK;

            return httpResponseMessage;
        }

        [AllowAnonymous]
        public HttpResponseMessage GetPhoto(int id)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();

            var emp = new Employee(id, true);
            if (emp.Photo == null || (emp.Photo != null && emp.Photo.Length == 0))
            {
                if (emp.Male)
                {
                    emp.Photo = File.ReadAllBytes(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/images/no_photo_male.jpg"));
                }
                else { emp.Photo = File.ReadAllBytes(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/images/no_photo_female.png")); }
            }

            httpResponseMessage.Content = new ByteArrayContent(emp.Photo);

            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("image/gif");
            httpResponseMessage.StatusCode = HttpStatusCode.OK;

            return httpResponseMessage;
        }

        [AuthorizeAd(Groups =new[]{AdGroup.PersonalManager} )]
        public HttpResponseMessage Save(Employee emp)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                emp.CurUserAdSid = GetCurUser().Sid;
                emp.Save();
                response.Content = new StringContent(String.Format("{{\"id\":{0}}}", emp.Id));
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));
            }
            return response;
        }
        [AuthorizeAd(Groups = new[] { AdGroup.PersonalManager })]
        public HttpResponseMessage Close(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                Employee.Close(id);
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }
        [AuthorizeAd(Groups = new[] { AdGroup.PersonalManager })]
        public HttpResponseMessage SetStateFired(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                Employee.SetStateFired(id);
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }
        [AuthorizeAd(Groups = new[] { AdGroup.PersonalManager })]
        public HttpResponseMessage SetStateDecree(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                Employee.SetStateDecree(id);
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }
        [AuthorizeAd(Groups = new[] { AdGroup.PersonalManager })]
        public HttpResponseMessage SetStateStuff(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                Employee.SetStateStuff(id);
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));

            }
            return response;
        }
        public IEnumerable<Employee> GetTodayBirthdayList()
        {
            return Employee.GetDayBirthdayList(DateTime.Now);
        }

        public IEnumerable<Employee> GetNextMonthBirthdayList()
        {
            return Employee.GetMonthBirthdayList(DateTime.Now.AddMonths(1).Month);
        }

        public IEnumerable<string> GetFullRecipientList(string citySysName)
        {
            int? idCity = null;
            if (!String.IsNullOrEmpty(citySysName)) idCity = new City(citySysName).Id;
            return Employee.GetFullRecipientList(idCity: idCity);
        }

        public IEnumerable<string> GetHolidayWorkDeliveryRecipientList()
        {
            return Employee.GetHolidayWorkDeliveryRecipientList();
        }

        public IEnumerable<Employee> GetNewbieList()
        {
            return Employee.GetNewbieList(DateTime.Now.AddDays(-1));
        }

        public string GetCurrentUserSid()
        {
            return GetCurUser().Sid;
        }

        public string GetCurrentUserName()
        {
            return GetCurUser().User.Identity.Name;
        }

        public HttpResponseMessage SetDeliverySend(Employee[] emps)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                foreach (Employee emp in emps)
                {
                    Employee.SetDeliverySend(emp.Id);
                }
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(MessageHelper.ConfigureExceptionMessage(ex));
            }
            return response;
        }
        [HttpGet]
        public bool IsChief(string sid)
        {
            bool result = new Employee(sid).IsChief;
            return result;
        }

        public IEnumerable<KeyValuePair<string,string>> GetSubordinatesSimple(string sid)
        {
            var list = Employee.GetSubordinatesSimple(sid);

            return list;
        } 
    }
}
