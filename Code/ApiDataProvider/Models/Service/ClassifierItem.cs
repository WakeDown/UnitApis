using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Objects;

namespace DataProvider.Models.Service
{
    public class ClassifierItem:DbModel
    {
        public int Id { get; set; }
        public int IdCategory { get; set; }
        public int IdWorkType { get; set; }
        public int? Time { get; set; }
        public decimal? Price { get; set; }
        public decimal? CostPeople { get; set; }//Стоимость для инженера
        public decimal? CostCompany { get; set; }//Стоимость для компании

        public ClassifierItem() { }

        public ClassifierItem(string number)
        {
            SqlParameter pNumber = new SqlParameter() { ParameterName = "number", SqlValue = number, SqlDbType = SqlDbType.NVarChar };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_classifier_category", pNumber);

            if (dt.Rows.Count > 0)
            {
                FillSelf(dt.Rows[0]);
            }
        }

        public ClassifierItem(DataRow row)
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            IdCategory = Db.DbHelper.GetValueIntOrDefault(row, "id_category");
            IdWorkType = Db.DbHelper.GetValueIntOrDefault(row, "id_work_type");
            Time = Db.DbHelper.GetValueIntOrDefault(row, "time");
            Price = Db.DbHelper.GetValueDecimalOrDefault(row, "price");
            CostPeople = Db.DbHelper.GetValueDecimalOrDefault(row, "cost_people");
            CostCompany = Db.DbHelper.GetValueDecimalOrDefault(row, "cost_company");
        }

        public void Save()
        {
            SqlParameter pIdCategory = new SqlParameter() { ParameterName = "id_category", SqlValue = IdCategory, SqlDbType = SqlDbType.Int };
            SqlParameter pIdWorkType = new SqlParameter() { ParameterName = "id_work_type", SqlValue = IdWorkType, SqlDbType = SqlDbType.Int };
            SqlParameter pTime = new SqlParameter() { ParameterName = "time", SqlValue = Time, SqlDbType = SqlDbType.Int };
            SqlParameter pPrice = new SqlParameter() { ParameterName = "price", SqlValue = Price, SqlDbType = SqlDbType.Decimal };
            SqlParameter pCostPeople = new SqlParameter() { ParameterName = "cost_people", SqlValue = CostPeople, SqlDbType = SqlDbType.Decimal };
            SqlParameter pCostCompany = new SqlParameter() { ParameterName = "cost_company", SqlValue = CostCompany, SqlDbType = SqlDbType.Decimal }; 
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };

            var dt = Db.Service.ExecuteQueryStoredProcedure("save_classifier_item", pIdCategory, pIdWorkType, pTime, pPrice, pCostPeople, pCostCompany, pCreatorAdSid);
            int id = 0;
            if (dt.Rows.Count > 0)
            {
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;
            }
        }
    }
}