﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Objects;

namespace DataProvider.Models.Service
{
    public class ServiceIssuePlan:DbModel
    {
        public int Id { get; set; }
        public int IdServiceIssue { get; set; }
        public int IdServiceIssueType { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public string CreatorSid { get; set; }



        public ServiceIssuePlan() { }

        public ServiceIssuePlan(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_service_issue_plan", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
        }

        public ServiceIssuePlan(int idServiceIssue, int idServiceIssueType)
        {
            SqlParameter pIdServiceIssue = new SqlParameter() { ParameterName = "id_service_issue", SqlValue = idServiceIssue, SqlDbType = SqlDbType.Int };
            SqlParameter pIdServiceIssueType = new SqlParameter() { ParameterName = "id_service_issue_type", SqlValue = idServiceIssueType, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_service_issue_plan", pIdServiceIssue, pIdServiceIssueType);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
        }

        public ServiceIssuePlan(DataRow row)
            : this()
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            IdServiceIssue = Db.DbHelper.GetValueIntOrDefault(row, "id_service_issue");
            IdServiceIssueType = Db.DbHelper.GetValueIntOrDefault(row, "id_service_issue_type");
            PeriodStart = Db.DbHelper.GetValueDateTimeOrDefault(row, "period_start");
            PeriodEnd = Db.DbHelper.GetValueDateTimeOrDefault(row, "period_end");
            CreatorSid = Db.DbHelper.GetValueString(row, "creator_sid");
        }

        public void Save()
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            SqlParameter pIdServiceIssue = new SqlParameter() { ParameterName = "id_service_issue", SqlValue = IdServiceIssue, SqlDbType = SqlDbType.Int };
            SqlParameter pIdServiceIssueType = new SqlParameter() { ParameterName = "id_service_issue_type", SqlValue = IdServiceIssueType, SqlDbType = SqlDbType.Int };
            SqlParameter pPeriodStart = new SqlParameter() { ParameterName = "period_start", SqlValue = PeriodStart, SqlDbType = SqlDbType.Date };
            SqlParameter pPeriodEnd = new SqlParameter() { ParameterName = "period_end", SqlValue = PeriodEnd, SqlDbType = SqlDbType.Date };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };

            var dt = Db.Service.ExecuteQueryStoredProcedure("save_service_issue_plan", pId, pIdServiceIssue, pIdServiceIssueType, pPeriodStart, pPeriodEnd, pCreatorAdSid);
            int id = 0;
            if (dt.Rows.Count > 0)
            {
                Int32.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;

                if (Db.DbHelper.GetValueBool(dt.Rows[0], "exists"))//Если запись существует
                {
                    FillSelf(dt.Rows[0]);
                    throw new ItemExistsException($"Заявка №{IdServiceIssue} уже влючена в план на период {PeriodStart:dd.MM.yy} - {PeriodEnd:dd.MM.yy} пользователем {AdHelper.GetUserBySid(CreatorSid).DisplayName}");
                }
            }
        }

        public static IEnumerable<ServiceIssuePlan> GetList(DateTime periodStart, DateTime periodEnd)
        {
            SqlParameter pPeriodStart = new SqlParameter() { ParameterName = "period_start", SqlValue = periodStart, SqlDbType = SqlDbType.Date };
            SqlParameter pPeriodEnd = new SqlParameter() { ParameterName = "period_end", SqlValue = periodEnd, SqlDbType = SqlDbType.Date };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_service_issue_plan_list", pPeriodStart, pPeriodEnd);

            var lst = new List<ServiceIssuePlan>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new ServiceIssuePlan(row);
                lst.Add(model);
            }

            return lst;
        }
        /// <summary>
        /// Текущий
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public static IEnumerable<ServiceIssuePeriodItem> GetPeriodMonthList(int year, int month)
        {
            var list = new List<ServiceIssuePeriodItem>();

            DateTime lastDayOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            //DateTime lastDay = lastDayOfMonth;//DateTimeHelper.GetNextWeekday(lastDayOfMonth, DayOfWeek.Monday);
            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            DateTime fDay = firstDayOfMonth;//DateTimeHelper.GetPrevWeekday(firstDayOfMonth, DayOfWeek.Monday).AddDays(-1);
            bool isFirst = true;
            DateTime lDay = new DateTime();
            while (lDay < lastDayOfMonth)
            {
                if (!isFirst)
                {
                    fDay = DateTimeHelper.GetNextWeekday(lDay, DayOfWeek.Monday);
                }
                else
                {
                    isFirst = false;
                    fDay = firstDayOfMonth;
                }

                lDay = DateTimeHelper.GetNextWeekday(fDay, DayOfWeek.Sunday);
                if (lDay > lastDayOfMonth) lDay = lastDayOfMonth;
                list.Add(new ServiceIssuePeriodItem(fDay, lDay));
            }

            return list;
        }
        /// <summary>
        /// Текущий, прошлый и будущий
        /// </summary>
        /// <param name="curYear"></param>
        /// <param name="curMonth"></param>
        /// <returns></returns>
        public static IEnumerable<ServiceIssuePeriodItem> GetPeriodMonthCurPrevNextList(int curYear, int curMonth)
        {
            var list = new List<ServiceIssuePeriodItem>();
            DateTime curDate = new DateTime(curYear, curMonth, 1);
            DateTime prevDate = curDate.AddMonths(-1);
            DateTime nextDate = curDate.AddMonths(1);
            list.AddRange(GetPeriodMonthList(prevDate.Year, prevDate.Month));
            list.AddRange(GetPeriodMonthList(curDate.Year, curDate.Month));
            list.AddRange(GetPeriodMonthList(nextDate.Year, nextDate.Month));

            return list;
        }

    }
}