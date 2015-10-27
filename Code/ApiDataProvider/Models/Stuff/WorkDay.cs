using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;

namespace DataProvider.Models.Stuff
{
    public class WorkDay
    {
        public static HolidayResult CheckIsPreHoliday(DateTime date)
        {
            var result = new HolidayResult() { SendDelivery = false };

            SqlParameter pDate = new SqlParameter() { ParameterName = "date", SqlValue = date, SqlDbType = SqlDbType.Date };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("check_is_work_day", pDate);

            if (dt.Rows.Count > 0)
            {
                bool isWorkDay = Db.DbHelper.GetValueBool(dt.Rows[0], "result");
                if (isWorkDay)
                {
                    SqlParameter pDate1 = new SqlParameter()
                    {
                        ParameterName = "date",
                        SqlValue = date,
                        SqlDbType = SqlDbType.Date
                    };
                    var dt1 = Db.Stuff.ExecuteQueryStoredProcedure("get_next_work_day", pDate1);
                    if (dt1.Rows.Count > 0)
                    {
                        DateTime? nextWorkDate = Db.DbHelper.GetValueDateTimeOrNull(dt1.Rows[0], "date");
                        if (nextWorkDate.HasValue && nextWorkDate.Value.Date != date.AddDays(1).Date)
                        {
                            result.SendDelivery = true;
                            result.DateStart = date.AddDays(1).Date;
                            result.DateEnd = nextWorkDate.Value.Date.AddDays(-1);
                            result.IsSundayOnly = (result.DateStart.AddDays(1).Date == result.DateEnd.Date) &&
                                                  result.DateEnd.DayOfWeek == DayOfWeek.Sunday;
                        }
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Считает количество ПРАЗДНИЧНЫХ дней в периоде НЕ ВЫХОДНЫХ
        /// </summary>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <returns></returns>
        public static int GetHolidaysCountInPeriod(DateTime dateStart, DateTime dateEnd)
        {
            SqlParameter pDateStart = new SqlParameter() { ParameterName = "@date_start", SqlValue = dateStart, SqlDbType = SqlDbType.Date };
            SqlParameter pDateEnd = new SqlParameter() { ParameterName = "@date_end", SqlValue = dateEnd, SqlDbType = SqlDbType.Date };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("hilodays_count_in_period", pDateStart, pDateEnd);

            int count = dt.Rows.Count;

            return count;
        }


    }
}