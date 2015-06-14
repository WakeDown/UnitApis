using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Stuff.Models;

namespace Stuff.Objects
{
    public class DbList
    {


        public static SelectList GetOrganizationList()
        {
            //Departments = new SelectList(Department.GetList(), "Id", "Name");
            //Positions = new SelectList(Position.GetList(), "Id", "Name");
            return new SelectList(Organization.GetSelectionList(), "Id", "Name");
            //Cities = new SelectList(City.GetList(), "Id", "Name");
        }
        public static SelectList GetDepartmentList()
        {
            return new SelectList(Department.GetSelectionList(), "Id", "Name");
        }
        public static SelectList GetPositionList()
        {
            return new SelectList(Position.GetSelectionList(), "Id", "Name");
        }
        public static SelectList GetCityList()
        {
            return new SelectList(City.GetSelectionList(), "Id", "Name");
        }
        public static SelectList GetEmployeeList()
        {
            return new SelectList(Employee.GetSelectionList(), "Id", "DisplayName");
        }
        public static SelectList GetEmpStateList()
        {
            return new SelectList(EmpState.GetSelectionList(), "Id", "Name");
        }
    }
}