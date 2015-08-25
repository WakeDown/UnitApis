using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Objects;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DataProvider.Models.Service
{
    public class ClassifierAttributes : DbModel
    {
        public decimal Wage { get; set; }
        public decimal Overhead { get; set; }

        public ClassifierAttributes() { }

        public ClassifierAttributes(DataRow row)
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Wage = Db.DbHelper.GetValueDecimalOrDefault(row, "wage");
            Overhead = Db.DbHelper.GetValueDecimalOrDefault(row, "overhead");
        }

        public static ClassifierAttributes Get()
        {
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_classifier_attributes");
            var model = new ClassifierAttributes(dt.Rows[0]);
            return model;
        }

        public void Save()
        {
            SqlParameter pWage = new SqlParameter() { ParameterName = "wage", SqlValue = Wage, SqlDbType = SqlDbType.Decimal };
            SqlParameter pOverhead = new SqlParameter() { ParameterName = "overhead", SqlValue = Overhead, SqlDbType = SqlDbType.Decimal };

            var dt = Db.Service.ExecuteQueryStoredProcedure("save_classifier_attributes", pWage, pOverhead);
        }
    }
}