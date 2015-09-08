using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Objects;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DataProvider.Models.Service
{
    public class ServiceIssue:DbModel
    {
        public int Id { get; set; }
        public int IdClaim { get; set; }
        public string SpecialistSid { get; set; }
        public string Descr { get; set; }
        public DateTime DatePlan { get; set; }

        public ServiceIssue() { }

        public ServiceIssue(int id)
        {
            //SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            //var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_model", pId);
            //if (dt.Rows.Count > 0)
            //{
            //    var row = dt.Rows[0];
            //    FillSelf(row);
            //}
        }

        public ServiceIssue(DataRow row)
            : this()
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            IdClaim = Db.DbHelper.GetValueIntOrDefault(row, "id_claim");
            SpecialistSid = Db.DbHelper.GetValueString(row, "specialist_sid");
            Descr = Db.DbHelper.GetValueString(row, "descr");
            DatePlan = Db.DbHelper.GetValueDateTimeOrDefault(row, "date_plan");
        }

        public void Save()
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            SqlParameter pIdClaim = new SqlParameter() { ParameterName = "id_claim", SqlValue = IdClaim, SqlDbType = SqlDbType.Int };
            SqlParameter pSpecialistSid = new SqlParameter() { ParameterName = "specialist_sid", SqlValue = SpecialistSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pDescr = new SqlParameter() { ParameterName = "descr", SqlValue = Descr, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pDatePlan = new SqlParameter() { ParameterName = "date_plan", SqlValue = DatePlan, SqlDbType = SqlDbType.DateTime };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };

            var dt = Db.Service.ExecuteQueryStoredProcedure("save_service_issue", pId, pIdClaim, pSpecialistSid, pDescr, pDatePlan, pCreatorAdSid);
            int id = 0;
            if (dt.Rows.Count > 0)
            {
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;
            }
        }

        public static IEnumerable<ServiceIssue> GetList()
        {
            //SqlParameter pSome = new SqlParameter() { ParameterName = "some", SqlValue = some, SqlDbType = SqlDbType.NVarChar };
            //var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_model_list", pSome);

            var lst = new List<ServiceIssue>();

            //foreach (DataRow row in dt.Rows)
            //{
            //    var model = new ServiceIssue(row);
            //    lst.Add(model);
            //}

            return lst;
        }
    }
}