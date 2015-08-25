using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Models.Stuff;
using DataProvider.Objects;

namespace DataProvider.Models.Service
{
    public class ClassifierCaterory:DbModel
    {
        public int Id { get; set; }
        public int IdParent { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Descr { get; set; }
        public int Complexity { get; set; }
        //public int Time { get; set; }
        //public decimal Price { get; set; }
        //public decimal CostPeople { get; set; }
        //public decimal CostCompany { get; set; }

        public ClassifierCaterory() { }

        public ClassifierCaterory(string number)
        {
            SqlParameter pNumber = new SqlParameter() { ParameterName = "number", SqlValue = number, SqlDbType = SqlDbType.NVarChar };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_classifier_category", pNumber);

            if (dt.Rows.Count > 0)
            {
                FillSelf(dt.Rows[0]);
            }
        }

        public ClassifierCaterory(DataRow row)
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            IdParent = Db.DbHelper.GetValueIntOrDefault(row, "id_parent");
            Name = Db.DbHelper.GetValueString(row, "name");
            Number = Db.DbHelper.GetValueString(row, "number");
            Complexity= Db.DbHelper.GetValueIntOrDefault(row, "complexity");
            //Time = Db.DbHelper.GetValueIntOrDefault(row, "time");
            //Price = Db.DbHelper.GetValueDecimalOrDefault(row, "price");
            //CostPeople = Db.DbHelper.GetValueDecimalOrDefault(row, "cost_people");
            //CostCompany = Db.DbHelper.GetValueDecimalOrDefault(row, "cost_company");
        }

        public static IEnumerable<ClassifierCaterory> GetLowerList()
        {
            //SqlParameter pIdAdmin = new SqlParameter() { ParameterName = "id_admin", SqlValue = idAdmin, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_lower_classifier_category_list");

            var lst = new List<ClassifierCaterory>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new ClassifierCaterory(row);
                lst.Add(model);
            }

            return lst;
        }

        public void Save()
        {
            //if (IdParent <= 0)
                IdParent = GetParentCategory(Number).Id;
            Number = Number.Trim();
            if (Name.StartsWith(Number)) Name = Name.Remove(0, Number.Length + 1);
            Name = Name.Trim();

            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            SqlParameter pIdParent = new SqlParameter() { ParameterName = "id_parent", SqlValue = IdParent, SqlDbType = SqlDbType.Int };
            SqlParameter pNumber = new SqlParameter() { ParameterName = "number", SqlValue = Number, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pName = new SqlParameter() { ParameterName = "name", SqlValue = Name, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pDescr = new SqlParameter() { ParameterName = "descr", SqlValue = Descr, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pComplexity = new SqlParameter() { ParameterName = "complexity", SqlValue = Complexity, SqlDbType = SqlDbType.Int };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };

            var dt = Db.Service.ExecuteQueryStoredProcedure("save_classifier_category", pId, pIdParent,pName, pNumber, pDescr, pComplexity, pCreatorAdSid);
            int id = 0;
            if (dt.Rows.Count > 0)
            {
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;
            }
        }

        public static ClassifierCaterory GetParentCategory(string number)
        {
            var parent = new ClassifierCaterory();

            if (number.Length > 0)
            {
                //Преобразовывает номер к номеру родителя
                int pointIndex = number.LastIndexOf(".", StringComparison.Ordinal);
                string parentNumber = number.Substring(0, pointIndex);
                parent = new ClassifierCaterory(parentNumber);
            }
            return parent;
        }
    }
}