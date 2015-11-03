using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Objects;
using DataProvider._TMPLTS;

namespace DataProvider.Models.Stuff
{
    public class RestHolidayTransferDays:DbModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Descr { get; set; }

        public RestHolidayTransferDays() { }

        public RestHolidayTransferDays(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_rest_holiday_transfer_days", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
        }

        public RestHolidayTransferDays(DataRow row)
            : this()
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            Date = Db.DbHelper.GetValueDateTimeOrDefault(row, "date");
            Descr = Db.DbHelper.GetValueString(row, "descr");
        }

        public void Save()
        {
            SqlParameter pDate = new SqlParameter() { ParameterName = "date", SqlValue = Date, SqlDbType = SqlDbType.Date };
            SqlParameter pDescr = new SqlParameter() { ParameterName = "descr", SqlValue = Descr, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };

            var dt = Db.Stuff.ExecuteQueryStoredProcedure("save_rest_holiday_transfer_days", pDate, pDescr, pCreatorAdSid);
            int id = 0;
            if (dt.Rows.Count > 0)
            {
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;
            }
        }

        public static IEnumerable<RestHolidayTransferDays> GetList(int? year = null)
        {
            SqlParameter pYear = new SqlParameter() { ParameterName = "year", SqlValue = year, SqlDbType = SqlDbType.Int };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_rest_holiday_transfer_days_list", pYear);

            var lst = new List<RestHolidayTransferDays>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new RestHolidayTransferDays(row);
                lst.Add(model);
            }

            return lst;
        }

        public static void Close(int id, string deleterSid)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            SqlParameter pDeleterSid = new SqlParameter() { ParameterName = "deleter_sid", SqlValue = deleterSid, SqlDbType = SqlDbType.VarChar };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("close_rest_holiday_transfer_days_list", pId, pDeleterSid);
        }

        public static IEnumerable<int> GetYearList(int topRows = 3)
        {
            SqlParameter pTopRows = new SqlParameter() { ParameterName = "top_rows", SqlValue = topRows, SqlDbType = SqlDbType.Int };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_rest_holiday_transfer_days_years_list", pTopRows);

            var lst = new List<int>();

            foreach (DataRow row in dt.Rows)
            {
                lst.Add(Db.DbHelper.GetValueIntOrDefault(row, "year"));
            }

            return lst;
        }

        public static void Clone(int yearFrom, string creatorSid, int? yearTo = null)
        {
            if (!yearTo.HasValue)yearTo = DateTime.Now.Year;

            SqlParameter pYearFrom = new SqlParameter() { ParameterName = "year_from", SqlValue = yearFrom, SqlDbType = SqlDbType.Int };
            SqlParameter pYearTo = new SqlParameter() { ParameterName = "year_from", SqlValue = yearTo, SqlDbType = SqlDbType.Int };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = creatorSid, SqlDbType = SqlDbType.VarChar };

            var dt = Db.Stuff.ExecuteQueryStoredProcedure("clone_rest_holiday_transfer_days", pYearFrom, pYearTo, pCreatorAdSid);
        }
    }
}