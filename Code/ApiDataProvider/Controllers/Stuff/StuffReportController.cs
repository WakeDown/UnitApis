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
using WebGrease.Css.Extensions;

namespace DataProvider.Controllers.Stuff
{
    public class StuffReportController : ApiController
    {
        [AuthorizeAd(Groups = new[] { AdGroup.SystemUser,AdGroup.SystemUser })]
        public IEnumerable<ItBudgetReportItem> GetItBudgetList(float? peopleCost = 350F)
        {
            if (!peopleCost.HasValue) peopleCost = 350F;
            var listAll = Budget.GetList().ToList();
            var result = new List<ItBudgetReportItem>();
            var firstLine = listAll.Where(x => !x.IdParent.HasValue);
            int level = 0;

            foreach (Budget bud in firstLine)
            {
                var item = FillItBudgetItem(bud, peopleCost.Value, level);
                result.Add(item);
                var budChildList = listAll.Where(x => x.IdParent == bud.Id).ToList();
                if (budChildList.Any())
                    ItBudgetFillBudChilds(ref result, budChildList, peopleCost.Value, listAll, level);
            }

            //Заполняем правильное количество людей учитывая вложенные бюджеты
            //var list = listAll;
            foreach (Budget item in firstLine)
            {
                var bud = result.Single(x => x.BudgetName.Equals(item.Name));
                int count = bud.PeopleCount;
                if (listAll.Any(x => x.IdParent == item.Id))
                {
                    count += GetChildPeopleCount(ref listAll, ref result, item.Id);
                }
                bud.PeopleCount = count;
            }

            return result;
        }

        public int GetChildPeopleCount(ref List<Budget> listAll, ref List<ItBudgetReportItem> result, int parentId)
        {
            int count = 0;
            var list = listAll.Where(x => x.IdParent == parentId).ToList();
            foreach (Budget item in list)
            {
                var bud = result.Single(x => x.BudgetName.Equals(item.Name));

                if (listAll.Any(x => x.IdParent == item.Id))
                {
                    bud.PeopleCount += GetChildPeopleCount(ref listAll, ref result, item.Id);
                }
                count += bud.PeopleCount;
            }
            return count;
        }

        [AuthorizeAd(Groups = new[] { AdGroup.SystemUser, AdGroup.SystemUser })]
        public void ItBudgetFillBudChilds(ref List<ItBudgetReportItem> result, IEnumerable<Budget> childList, float peopleCost, List<Budget> listAll, int level)
        {
            level++;
            foreach (Budget bud in childList)
            {
                var childItem = FillItBudgetItem(bud, peopleCost, level);
                result.Add(childItem);
                var budChildList = listAll.Where(x => x.IdParent == bud.Id).ToList();
                if (budChildList.Any())
                    ItBudgetFillBudChilds(ref result, budChildList, peopleCost, listAll, level);
            }
        }

        [AuthorizeAd(Groups = new[] { AdGroup.SystemUser, AdGroup.SystemUser })]
        public ItBudgetReportItem FillItBudgetItem(Budget bud, float peopleCost, int level)
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
           item.Level = level;
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
