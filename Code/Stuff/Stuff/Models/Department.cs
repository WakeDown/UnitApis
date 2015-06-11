using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.OData.Client;
using Newtonsoft.Json;
using Stuff.Objects;

namespace Stuff.Models
{
    public class Department:DbModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Employee Chief { get; set; }
        public Department ParentDepartment { get; set; }

        public IEnumerable<Department> ChildList { get; set; }

        public Department() { }

        public Department(int id)
        {
            Uri uri = new Uri(String.Format("{0}/Department/Get?id={1}", OdataServiceUri, id));
            string jsonString = GetJson(uri);
            var dep = JsonConvert.DeserializeObject<Department>(jsonString);
            FillSelf(dep);
            //Id = 1;
            //ParentDepartment = new Department() { Id = 2 };
            //Chief = new Employee() { FullName = "Гималтдинов Ильдар Разифович" };
        }

        private void FillSelf(Department dep)
        {
            Id = dep.Id;
            Name = dep.Name;
            ParentDepartment = dep.ParentDepartment;
            Chief = dep.Chief;
        }

        public bool Save(out string errorMessage)
        {
            Uri uri = new Uri(String.Format("{0}/Department/Save", OdataServiceUri));
            string json = JsonConvert.SerializeObject(this);
            bool result = SendJson(uri, json, out errorMessage);
            return result;
        }

        public static IEnumerable<Department> GetList()
        {
            Uri uri = new Uri(String.Format("{0}/Department/GetList", OdataServiceUri));
            string jsonString = GetJson(uri);

            var deps = JsonConvert.DeserializeObject<IEnumerable<Department>>(jsonString);

            return deps;
        }

        public static IEnumerable<Department> GetSelectionList()
        {
            Uri uri = new Uri(String.Format("{0}/Department/GetList", OdataServiceUri));
            string jsonString = GetJson(uri);

            var deps = JsonConvert.DeserializeObject<IEnumerable<Department>>(jsonString);

            return deps;
        }

        public List<Employee> GetStuff()
        {
            return new List<Employee>() {new Employee(){DisplayName = "Сотрудник 1"}, new Employee(){DisplayName = "Сотрудник 2"}, new Employee(){DisplayName = "Сотрудник 3"}, new Employee(){DisplayName = "Сотрудник 4"}, new Employee(){DisplayName = "Сотрудник 5"}};
        }

        public static List<Department> GetOrgStructure()
        {
            var deps = GetList().ToList();
            var result = new List<Department>();

            //Отделяем подразделения без Парентов
            result = deps.Where(d => d.ParentDepartment == null || d.ParentDepartment == new Department()).ToList();
            deps.RemoveAll(d => d.ParentDepartment == null || d.ParentDepartment == new Department());

            foreach (Department dep in result)
            {
                dep.ChildList = GetDepartmentChilds(dep.Id, ref deps);
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