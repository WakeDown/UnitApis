using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Objects;
using DataProvider._TMPLTS;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DataProvider.Models.Stuff
{
    public class StatementPrint:DbModel
    {
        public int Id { get; set; }
        public int IdStatementType { get; set; }
        public string EmployeeSid { get; set; }
        public DateTime? DateBegin { get; set; }
        public DateTime? DateEnd { get; set; }
        public int? DurationHours { get; set; }
        public int? DurationDays { get; set; }
        public string Cause { get; set; }
        public int? IdDepartment { get; set; }
        public int? IdOrganization { get; set; }
        public bool Confirmed { get; set; }
        public DateTime? DateConfirm { get; set; }

        public EmployeeSm Employee { get; set; }

        public StatementPrint() { }

        //public StatementPrint(int id)
        //{
        //    SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
        //    var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_model", pId);
        //    if (dt.Rows.Count > 0)
        //    {
        //        var row = dt.Rows[0];
        //        FillSelf(row);
        //    }
        //}

        public StatementPrint(DataRow row)
            : this()
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            IdStatementType = Db.DbHelper.GetValueIntOrDefault(row, "id_statement_type");
            EmployeeSid = Db.DbHelper.GetValueString(row, "employee_sid");
            Employee =
            new EmployeeSm()
            {
                AdSid = Db.DbHelper.GetValueString(row,"employee_sid"),
                DisplayName = Db.DbHelper.GetValueString(row, "employee_display_name")
            };
            DateBegin = Db.DbHelper.GetValueDateTimeOrNull(row, "date_begin");
            DateEnd = Db.DbHelper.GetValueDateTimeOrNull(row, "date_end");
            DurationHours = Db.DbHelper.GetValueIntOrDefault(row, "duration_hours");
            DurationDays = Db.DbHelper.GetValueIntOrDefault(row, "duration_days");
            Cause = Db.DbHelper.GetValueString(row, "cause");
            IdDepartment = Db.DbHelper.GetValueIntOrDefault(row, "id_department");
            IdOrganization = Db.DbHelper.GetValueIntOrDefault(row, "id_organization");
            Confirmed = Db.DbHelper.GetValueBool(row, "confirmed");
            DateConfirm = Db.DbHelper.GetValueDateTimeOrNull(row, "date_confirm");
        }

        public void Save()
        {
            SqlParameter pIdStatementType = new SqlParameter() { ParameterName = "id_statement_type", SqlValue = IdStatementType, SqlDbType = SqlDbType.Int };
            SqlParameter pEmployeeSid = new SqlParameter() { ParameterName = "employee_sid", SqlValue = EmployeeSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pDateBegin = new SqlParameter() { ParameterName = "date_begin", SqlValue = DateBegin, SqlDbType = SqlDbType.DateTime };
            SqlParameter pDateEnd = new SqlParameter() { ParameterName = "date_end", SqlValue = DateEnd, SqlDbType = SqlDbType.DateTime };
            SqlParameter pDurationHours = new SqlParameter() { ParameterName = "duration_hours", SqlValue = DurationHours, SqlDbType = SqlDbType.Int };
            SqlParameter pDurationDays = new SqlParameter() { ParameterName = "duration_days", SqlValue = DurationDays, SqlDbType = SqlDbType.Int };
            SqlParameter pCause = new SqlParameter() { ParameterName = "cause", SqlValue = Cause, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pIdDepartment = new SqlParameter() { ParameterName = "id_department", SqlValue = IdDepartment, SqlDbType = SqlDbType.Int };
            SqlParameter pIdOrganization = new SqlParameter() { ParameterName = "id_organization", SqlValue = IdOrganization, SqlDbType = SqlDbType.Int };
            SqlParameter pConfirmed = new SqlParameter() { ParameterName = "confirmed", SqlValue = Confirmed, SqlDbType = SqlDbType.Bit };
            SqlParameter pDateConfirm = new SqlParameter() { ParameterName = "date_confirm", SqlValue = DateConfirm, SqlDbType = SqlDbType.DateTime };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };

            var dt = Db.Stuff.ExecuteQueryStoredProcedure("statement_printed_save", pIdStatementType, pEmployeeSid, pDateBegin, pDateEnd, pDurationHours, pDurationDays, pCause, pIdDepartment, pIdOrganization, pConfirmed, pDateConfirm, pCreatorAdSid);
            int id = 0;
            if (dt.Rows.Count > 0)
            {
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;
            }
        }

        /// <summary>
        /// Список принятых заявлений
        /// </summary>
        /// <param name="employeeSid"></param>
        /// <returns></returns>
        public static IEnumerable<StatementPrint> GetList(string employeeSid = null)
        {
            SqlParameter pEmployeeSid = new SqlParameter() { ParameterName = "employee_sid", SqlValue = employeeSid, SqlDbType = SqlDbType.VarChar };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("statement_printed_get_list", pEmployeeSid);

            var lst = new List<StatementPrint>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new StatementPrint(row);
                lst.Add(model);
            }

            return lst;
        }

        //public static void Close(int id, string deleterSid)
        //{
        //    SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
        //    SqlParameter pDeleterSid = new SqlParameter() { ParameterName = "deleter_sid", SqlValue = deleterSid, SqlDbType = SqlDbType.VarChar };
        //    var dt = Db.Stuff.ExecuteQueryStoredProcedure("close_model", pId, pDeleterSid);
        //}

        public static void Confirm(int id, string confirmatorSid, bool confirm = true)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            SqlParameter pConfirmatorSid = new SqlParameter() { ParameterName = "confirmator_sid", SqlValue = confirmatorSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pConfirm = new SqlParameter() { ParameterName = "confirm", SqlValue = confirm, SqlDbType = SqlDbType.Bit };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("statement_printed_confirm", pId, pConfirmatorSid, pConfirm);
        }
    }
}