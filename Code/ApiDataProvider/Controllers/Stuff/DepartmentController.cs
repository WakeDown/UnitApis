using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using DataProvider.Helpers;
using DataProvider.Models.Stuff;
using DataProvider.Objects;
using Objects;

namespace DataProvider.Controllers.Stuff
{
    public class DepartmentController : BaseApiController
    {
       
        public IEnumerable<Department> GetList()
        {
            bool userCanViewHiddenDeps = AdHelper.UserInGroup(GetCurUser().User, AdGroup.PersonalManager, AdGroup.SuperAdmin);
            return Department.GetList(userCanViewHiddenDeps: userCanViewHiddenDeps);
        }

        public IEnumerable<Department> GetAllTimeList()
        {
            bool userCanViewHiddenDeps = AdHelper.UserInGroup(GetCurUser().User, AdGroup.PersonalManager, AdGroup.SuperAdmin);
            return Department.GetAllTimeList(userCanViewHiddenDeps: userCanViewHiddenDeps);
        }

        public IEnumerable<Department> GetOrgStructure()
        {
            bool userCanViewHiddenDeps = AdHelper.UserInGroup(GetCurUser().User, AdGroup.PersonalManager, AdGroup.SuperAdmin);
            return Department.GetOrgStructure(userCanViewHiddenDeps: userCanViewHiddenDeps);
        }
        
        public Department Get(int id)
        {
            var dep = new Department(id);
            return dep;
        }

        [AuthorizeAd(Groups = new[] { AdGroup.PersonalManager })]
        public HttpResponseMessage Save(Department model)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                model.Save();
                response.Content = new StringContent(String.Format("{{\"id\":{0}}}", model.Id));
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(String.Format("{{\"errorMessage\":\"{0}\"}}", ex.Message));

            }
            return response;
        }

        [AuthorizeAd(Groups = new[] { AdGroup.PersonalManager })]
        public HttpResponseMessage Close(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created);

            try
            {
                Department.Close(id);
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(String.Format("{{\"errorMessage\":\"{0}\"}}", ex.Message));

            }
            return response;
        }
    }
}
