using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;
using DataProvider.Helpers;
using DataProvider.Objects;
using WebGrease.Css.Extensions;

namespace DataProvider.Models.Stuff
{
    public class Department:DbModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Department ParentDepartment { get; set; }
        public Employee Chief { get; set; }
        public int EmployeeCount { get; set; }
        //public Employee Creator { get; set; }
        public bool Hidden { get; set; }

        public IEnumerable<Department> ChildList { get; set; }
        public int OrgStructureLevel { get; set; }

        public IEnumerable<Employee> Stuff { get; set; } 


        public Department() { }

        public Department(DataRow row)
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueInt(row["id"]);
            Name = row["name"].ToString();
            ParentDepartment = new Department() { Id = Db.DbHelper.GetValueInt(row["id_parent"]), Name = row["parent"].ToString() };
            Chief = new Employee() { Id = Db.DbHelper.GetValueInt(row["id_chief"]), DisplayName = row["chief"].ToString() };
            EmployeeCount = Db.DbHelper.GetValueIntOrDefault(row["emp_count"]);
            Hidden = row.Table.Columns.Contains("hidden") && Db.DbHelper.GetValueBool(row["hidden"]);
        }

        public Department(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_department", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
        }

        public void Save()
        {
            using (var conn = Db.Stuff.connection)
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        //if (Creator == null) Creator = new Employee();
                        SqlParameter pId = new SqlParameter()
                        {
                            ParameterName = "id",
                            SqlValue = Id,
                            SqlDbType = SqlDbType.Int
                        };
                        SqlParameter pName = new SqlParameter()
                        {
                            ParameterName = "name",
                            SqlValue = Name,
                            SqlDbType = SqlDbType.NVarChar
                        };
                        SqlParameter pParentDepartment = new SqlParameter()
                        {
                            ParameterName = "id_parent",
                            SqlValue = ParentDepartment.Id,
                            SqlDbType = SqlDbType.Int
                        };
                        SqlParameter pChief = new SqlParameter()
                        {
                            ParameterName = "id_chief",
                            SqlValue = Chief.Id,
                            SqlDbType = SqlDbType.Int
                        };
                        SqlParameter pCreatorAdSid = new SqlParameter()
                        {
                            ParameterName = "creator_sid",
                            SqlValue = CurUserAdSid,
                            SqlDbType = SqlDbType.VarChar
                        };

                        var dt = Db.Stuff.ExecuteQueryStoredProcedure("save_department", pId, pName, pParentDepartment,
                            pChief, pCreatorAdSid);
                        int id=0;
                        if (dt.Rows.Count > 0)
                        {
                            int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                            Id = id;
                        }

                        //Пересохраняем руководителя у сотрудников
                        Employee.RefillManager();

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw;
                    }

                }
                conn.Close();
            }
        }

        public static IEnumerable<Department> GetList(bool getEmpCount = false, bool userCanViewHiddenDeps = false, bool? hasAdAccount = null)
        {
            SqlParameter pGetEmpCount = new SqlParameter() { ParameterName = "get_emp_count", SqlValue = getEmpCount, SqlDbType = SqlDbType.Bit };
            SqlParameter pHasAdAccount = new SqlParameter() { ParameterName = "has_ad_account", SqlValue = hasAdAccount, SqlDbType = SqlDbType.Bit };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_department", pGetEmpCount, pHasAdAccount);

            var lst = new List<Department>();

            foreach (DataRow row in dt.Rows)
            {
                var dep = new Department(row);
                if (dep.Hidden && !userCanViewHiddenDeps){continue;}
                lst.Add(dep);
            }

            return lst;
        }

        public static void Close(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("close_department", pId);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idDepartment">Подразделение для которого строим иерархическую ветку</param>
        /// <returns></returns>
        public static IEnumerable<Department> GetOrgStructure(int? idDepartment=null, bool userCanViewHiddenDeps = false, bool? hasAdAccount = null)
        {
            var deps = GetList(true, userCanViewHiddenDeps, hasAdAccount).ToList();
            var result = new List<Department>();
            int level = 1;
            //Отделяем подразделения без гавных
            result = deps.Where(d => d.ParentDepartment == null || d.ParentDepartment == new Department() || (d.ParentDepartment != null && d.ParentDepartment.Id == 0)).ToList();
            deps.RemoveAll(d => d.ParentDepartment == null || d.ParentDepartment == new Department());
            result.ForEach(d => d.OrgStructureLevel = level);
            level++;
            foreach (Department dep in result)
            {
                //if (getStuff) dep.Stuff = Employee.GetList(idDepartment: dep.Id);
                var childs = GetDepartmentChilds(dep.Id, ref deps, level);
                childs.ForEach(d => d.OrgStructureLevel = level);
                dep.ChildList = childs;
            }

            foreach (Department dep in result)
            {
                dep.EmployeeCount += GetChildEmpCount(dep.ChildList);
            }

            return result;
        }

        private static int GetChildEmpCount(IEnumerable<Department> childList)
        {
            int result = 0;

            foreach (Department dep in childList)
            {
                if (dep.ChildList.Any())
                {
                    dep.EmployeeCount += GetChildEmpCount(dep.ChildList);
                }

                result += dep.EmployeeCount;
            }

            return result;
        }

        public static IEnumerable<Department> GetDepartmentChilds(int id, ref List<Department> deps, int level)
        {
            level++;
            var result = new List<Department>();
            result = deps.Where(d => d.ParentDepartment.Id == id).ToList();
            deps.RemoveAll(d => d.ParentDepartment.Id == id);

            foreach (Department dep in result)
            {
                //if (getStuff) dep.Stuff = Employee.GetList(idDepartment: dep.Id);
                var childs = GetDepartmentChilds(dep.Id, ref deps, level);
                childs.ForEach(d => d.OrgStructureLevel = level);
                dep.ChildList = childs;

            }

            return result;
        }

        public static IEnumerable<Department> GetDepartmentChildList(int id, bool userCanViewHiddenDeps = false, bool hasAdAccount = true)
        {
            var deps = GetList(true, userCanViewHiddenDeps, hasAdAccount).ToList();

            var result= new Department(id).ChildList = GetDepartmentChilds(id, ref deps, 0);

            return result;
        }

        public static bool CheckUserIsChief(int idDepartment, int idEmployee)
        {
            bool result = false;
            SqlParameter pIdDepartment = new SqlParameter() { ParameterName = "id_department", SqlValue = idDepartment, SqlDbType = SqlDbType.Int };
            SqlParameter pIdEmployee = new SqlParameter() { ParameterName = "id_employee", SqlValue = idEmployee, SqlDbType = SqlDbType.Int };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("check_employee_is_chief", pIdDepartment, pIdEmployee);
            if (dt.Rows.Count > 0)
            {
                result = dt.Rows[0]["result"].ToString().Equals("1");
            }
            return result;
        }
    }
}