using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using DataProvider.Models.Stuff;
using DataProvider.Objects;

namespace DataProvider.Controllers.Stuff
{
    public class StuffReportController : ApiController
    {
        [AuthorizeAd(Groups = new[] { AdGroup.SystemUser,AdGroup.SystemUser })]
        public IEnumerable<ItBudgetReportItem> GetItBudgetList(float? peopleCost = 350F)
        {
            if (!peopleCost.HasValue) peopleCost = 350F;
            var list = Budget.GetList();
            var result = new List<ItBudgetReportItem>();

            foreach (Budget bud in list)
            {
                var item = FillItBudgetItem(bud, peopleCost.Value);
                result.Add(item);
            }

            return result;
        }

        

        [AuthorizeAd(Groups = new[] { AdGroup.SystemUser, AdGroup.SystemUser })]
        public ItBudgetReportItem FillItBudgetItem(Budget bud, float peopleCost)
        {
            var item = new ItBudgetReportItem();
            item.BudgetName = bud.Name;
            var empList = Employee.GetList(idBudget: bud.Id, showHidden: false);
            var peopleList = new List<ItBudgetReportItemPeople>();
            foreach (Employee emp in empList)
            {
                if (emp.HasAdAccount)
                {
                    var p = new ItBudgetReportItemPeople();
                    p.FullName = emp.FullName;
                    p.PositionName = emp.Position.Name;
                    p.Cost = peopleCost;
                    peopleList.Add(p);
                }
            }
            item.Peoples = peopleList;
            if (bud.EmpCount.HasValue)
            { item.PeopleCount = bud.EmpCount.Value;}
            else
            { item.PeopleCount = 0;}
           item.CostSum = item.PeopleCount * peopleCost;
            //item.Level = bud.OrgStructureLevel;
            return item;
        }

        //[AuthorizeAd(Groups = new[] { AdGroup.SystemUser, AdGroup.SystemUser })]
        //public IEnumerable<ItBudgetReportItem> GetItBudgetList(float? peopleCost = 350F)
        //{
        //    if (!peopleCost.HasValue) peopleCost = 350F;
        //    var list = Department.GetOrgStructure(userCanViewHiddenDeps: false, hasAdAccount: true);
        //    var result = new List<ItBudgetReportItem>();

        //    foreach (Department dep in list)
        //    {
        //        var item = FillItBudgetItem(dep, peopleCost.Value);
        //        result.Add(item);
        //        if (dep.ChildList.Any())
        //            ItBudgetFillDepChilds(ref result, dep.ChildList, peopleCost.Value);
        //        //foreach (Department childDep in dep.ChildList)
        //        //{
        //        //    var childItem = FillItBudgetItem(dep, peopleCost);
        //        //    result.Add(childItem);
        //        //}
        //    }

        //    return result;
        //}

        //[AuthorizeAd(Groups = new[] { AdGroup.SystemUser, AdGroup.SystemUser })]
        //public void ItBudgetFillDepChilds(ref List<ItBudgetReportItem> result, IEnumerable<Department> list, float peopleCost)
        //{
        //    foreach (Department dep in list)
        //    {
        //        var childItem = FillItBudgetItem(dep, peopleCost);
        //        result.Add(childItem);
        //        if (dep.ChildList.Any())
        //            ItBudgetFillDepChilds(ref result, dep.ChildList, peopleCost);
        //    }
        //}

        //[AuthorizeAd(Groups = new[] { AdGroup.SystemUser, AdGroup.SystemUser })]
        //public ItBudgetReportItem FillItBudgetItem(Department dep, float peopleCost)
        //{
        //    var item = new ItBudgetReportItem();
        //    item.DepartmentName = dep.Name;
        //    var empList = Employee.GetList(dep.Id, showHidden: false);
        //    var peopleList = new List<ItBudgetReportItemPeople>();
        //    foreach (Employee emp in empList)
        //    {
        //        if (emp.HasAdAccount)
        //        {
        //            var p = new ItBudgetReportItemPeople();
        //            p.FullName = emp.FullName;
        //            p.PositionName = emp.Position.Name;
        //            p.Cost = peopleCost;
        //            peopleList.Add(p);
        //        }
        //    }
        //    item.Peoples = peopleList;
        //    item.PeopleCount = dep.EmployeeCount;
        //    item.CostSum = item.PeopleCount * peopleCost;
        //    item.Level = dep.OrgStructureLevel;
        //    return item;
        //}

        //public string CreateItBudgetTable(IEnumerable<Department> list, double peopleCost)
        //{
        //    StringBuilder tbl = new StringBuilder();
        //    if (list.Any())
        //    {
        //        tbl.Append("<table>");
        //        tbl.Append("<tr><td>#</td><td>FIO</td><td></td></tr>");
        //        foreach (Department dep in list)
        //        {
        //            var empList = Employee.GetList(dep.Id, showHidden: false);
        //            int i = 0;
        //            foreach (Employee emp in empList)
        //            {
        //                tbl.Append($"<tr><td>{i}</td><td>{emp.FullName}</td><td>{peopleCost}</td></tr>");
        //            }
        //        }
        //        tbl.Append("</table>");
        //    }

        //    return tbl.ToString();
        //}
    }
}
