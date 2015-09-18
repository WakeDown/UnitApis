using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stuff.Models;

namespace Stuff.Tests.Models
{
    [TestClass]
    public class EmployeeTest
    {
        [TestMethod]
        public void TestModel()
        {
            Employee emp = new Employee();

            string surname51 = (new char[51]).ToString();
            Assert.IsTrue(surname51.Length == 51);
            emp.Surname = surname51;
            Assert.IsTrue(emp.Surname.Length==50);
        }
    }
}
