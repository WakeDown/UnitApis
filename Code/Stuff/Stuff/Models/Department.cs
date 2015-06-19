using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.OData.Client;
using Newtonsoft.Json;
using Stuff.Objects;
using WebGrease.Css.Extensions;

namespace Stuff.Models
{
    public class Department:DbModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Employee Chief { get; set; }
        public Department ParentDepartment { get; set; }

        public IEnumerable<Department> ChildList { get; set; }
        public int OrgStructureLevel { get; set; }

        public Department() { }

        public Department(int id)
        {
            Uri uri = new Uri(String.Format("{0}/Department/Get?id={1}", OdataServiceUri, id));
            string jsonString = GetJson(uri);
            var dep = JsonConvert.DeserializeObject<Department>(jsonString);
            FillSelf(dep);
        }

        private void FillSelf(Department dep)
        {
            Id = dep.Id;
            Name = dep.Name;
            ParentDepartment = dep.ParentDepartment;
            Chief = dep.Chief;
        }

        public bool Save(out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/Department/Save", OdataServiceUri));
            string json = JsonConvert.SerializeObject(this);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }

        public static IEnumerable<Department> GetList()
        {
            Uri uri = new Uri(String.Format("{0}/Department/GetList", OdataServiceUri));
            string jsonString = GetJson(uri);

            var deps = JsonConvert.DeserializeObject<IEnumerable<Department>>(jsonString);

            return deps;
        }

        public static bool Delete(int id, out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/Department/Close?id={1}", OdataServiceUri, id));
            string json = String.Empty;//String.Format("{{\"id\":{0}}}",id);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }

        public static IEnumerable<Department> GetSelectionList()
        {
            Uri uri = new Uri(String.Format("{0}/Department/GetList", OdataServiceUri));
            string jsonString = GetJson(uri);

            var deps = JsonConvert.DeserializeObject<IEnumerable<Department>>(jsonString);

            return deps;
        }

        public IEnumerable<Employee> GetStuff()
        {
            Uri uri = new Uri(String.Format("{0}/Employee/GetList?idDepartment={1}", OdataServiceUri, Id));
            string jsonString = GetJson(uri);

            var emps = JsonConvert.DeserializeObject<IEnumerable<Employee>>(jsonString);

            return emps;
        }

        public static List<Department> GetOrgStructure()
        {
            var deps = GetList().ToList();
            var result = new List<Department>();

            //Отделяем подразделения без Парентов
            result = deps.Where(d => d.ParentDepartment == null || d.ParentDepartment == new Department()|| (d.ParentDepartment != null && d.ParentDepartment.Id == 0)).ToList();
            deps.RemoveAll(d => d.ParentDepartment == null || d.ParentDepartment == new Department());
            result.ForEach(d => d.OrgStructureLevel = 1);

            foreach (Department dep in result)
            {
                var childs = GetDepartmentChilds(dep.Id, ref deps);
                childs.ForEach(d => d.OrgStructureLevel = 2);
                dep.ChildList = childs;
            }

            return result;
        }

        private static IEnumerable<Department> GetDepartmentChilds(int id, ref List<Department> deps)
        {
            var result = new List<Department>();
            result = deps.Where(d => d.ParentDepartment.Id == id).ToList();
            deps.RemoveAll(d => d.ParentDepartment.Id == id);

            foreach (Department dep in result)
            {
                dep.ChildList = GetDepartmentChilds(dep.Id, ref deps);
            }

            return result; 
        }

        
    }
}