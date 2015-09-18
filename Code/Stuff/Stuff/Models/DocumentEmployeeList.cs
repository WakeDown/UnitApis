using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stuff.Models
{
    public class DocumentEmployeeList:Statement
    {
        public IEnumerable<Employee> Employees { get; set; }

        public DocumentEmployeeList()
        {
            Employees = Employee.GetList(26, showHidden:false);
            Organization = new Organization("UPR");
        }
    }
}