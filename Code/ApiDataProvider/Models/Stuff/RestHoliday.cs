using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using DataProvider.Helpers;
using DataProvider.Objects;
using DataProvider._TMPLTS;

namespace DataProvider.Models.Stuff
{
    public class RestHoliday:DbModel
    {
        public int Id { get; set; }
        public string EmployeeSid { get; set; }
        public string EmployeeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Year { get; set; }
        public int Duration { get; set; }
        public bool CanEdit { get; set; }
        public  bool Confirmed { get; set; }

        public static int RestHolidaysMaxCount
        {
            get
            {
                int restHolidaysMaxCount;
                int.TryParse(ConfigurationManager.AppSettings["restHolidaysMaxCount"], out restHolidaysMaxCount);
                if (restHolidaysMaxCount == 0)restHolidaysMaxCount = 28;
                return restHolidaysMaxCount;
            }
        }

        public RestHoliday() { }

        public RestHoliday(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("rest_holiday_get", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
        }

        public RestHoliday(DataRow row)
            : this()
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            EmployeeSid = Db.DbHelper.GetValueString(row, "employee_sid");
            EmployeeName = Db.DbHelper.GetValueString(row, "employee_name");
            StartDate = Db.DbHelper.GetValueDateTimeOrDefault(row, "start_date");
            EndDate = Db.DbHelper.GetValueDateTimeOrDefault(row, "end_date");
            Year = Db.DbHelper.GetValueIntOrDefault(row, "year");
            Duration = Db.DbHelper.GetValueIntOrDefault(row, "duration");
            CanEdit = Db.DbHelper.GetValueBool(row, "can_edit");
            Confirmed = Db.DbHelper.GetValueBool(row, "confirmed");
        }

        public void Save()
        {
            if (String.IsNullOrEmpty(EmployeeSid))EmployeeSid = CurUserAdSid;
            if (Year <= 0)Year = StartDate.Year;

            
            
            //Дней отпуска осталось
            int daysExists = GetYears4Employee(EmployeeSid, year: Year).FirstOrDefault().Value;

            if (daysExists <= 0 || daysExists - Duration < 0)
            {
                throw new ArgumentException($"Количество дней отпуска в {Year} году превышает {RestHolidaysMaxCount} дней. Период не был сохранен.");
            }

            //Должен быть один период создаржащий указанное количество дней
            int oneMinPeriodDays = 14;

            //Проверка есть ли хотябы один период не менее 14 дней
            if (!GetList(EmployeeSid, Year).Any(x => x.Duration >= oneMinPeriodDays) && daysExists - Duration  < oneMinPeriodDays && Duration < oneMinPeriodDays) throw new ArgumentException("Необходимо указать хотябы один период не менее 14 дней. Период не был сохранен.");

            SqlParameter pEmployeeSid = new SqlParameter() { ParameterName = "employee_sid", SqlValue = EmployeeSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pStartDate = new SqlParameter() { ParameterName = "start_date", SqlValue = StartDate, SqlDbType = SqlDbType.Date };
            SqlParameter pEndDate = new SqlParameter() { ParameterName = "end_date", SqlValue = EndDate, SqlDbType = SqlDbType.Date };
            SqlParameter pYear = new SqlParameter() { ParameterName = "year", SqlValue = Year, SqlDbType = SqlDbType.Int };
            SqlParameter pDuration = new SqlParameter() { ParameterName = "duration", SqlValue = Duration, SqlDbType = SqlDbType.Int };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };

            //Проверка есть ли пересечения
            var dtCheck = Db.Stuff.ExecuteQueryStoredProcedure("rest_holiday_check_period_cross", pEmployeeSid, pStartDate, pEndDate);
            bool hasCrosses = true;

            if (dtCheck.Rows.Count > 0)
            {
                hasCrosses = Db.DbHelper.GetValueBool(dtCheck.Rows[0], "result");
            }

            if (hasCrosses) throw new ArgumentException("Найдены пересечения с другими периодами. Период не был сохранен.");
            
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("rest_holiday_save", pEmployeeSid, pStartDate, pEndDate, pYear, pDuration, pCreatorAdSid);
            int id = 0;
            if (dt.Rows.Count > 0)
            {
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;
            }
        }
        /// <summary>
        /// Подтверждениеи обратная ему операция для списка периодов отпусков
        /// </summary>
        /// <param name="idArray">массив id периодов</param>
        /// <param name="canEdit">пользователь может редактировать или нет</param>
        /// <param name="confirmed">Утверждение, после этого никакой редактирование невозможно и даже если canEdit = true</param>
        public static void Confirm(string creatorSid, int[] idArray, bool? canEdit = null, bool? confirmed = null)
        {
            SqlParameter pIdArray = new SqlParameter() { ParameterName = "@id_array", SqlValue = String.Join(",", idArray), SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCanEdit = new SqlParameter() { ParameterName = "can_edit", SqlValue = canEdit, SqlDbType = SqlDbType.Bit };
            SqlParameter pConfirmed = new SqlParameter() { ParameterName = "@confirmed", SqlValue = confirmed, SqlDbType = SqlDbType.Bit };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = creatorSid, SqlDbType = SqlDbType.VarChar };

            var dt = Db.Stuff.ExecuteQueryStoredProcedure("rest_holiday_list_confirm", pIdArray, pCanEdit, pConfirmed, pCreatorAdSid);
        }
        /// <summary>
        /// Подтверждениеи обратная ему операция для списка периодов отпусков
        /// </summary>
        /// <param name="creatorSid"></param>
        /// <param name="employeeSid"></param>
        /// <param name="year"></param>
        /// <param name="canEdit"></param>
        /// <param name="confirmed"></param>
        public static void Confirm(string creatorSid, string employeeSid, int? year, bool? canEdit = null, bool? confirmed = null)
        {
            SqlParameter pEmployeeSid = new SqlParameter() { ParameterName = "employee_sid", SqlValue = employeeSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pYear = new SqlParameter() { ParameterName = "year", SqlValue = year, SqlDbType = SqlDbType.Int };
            SqlParameter pCanEdit = new SqlParameter() { ParameterName = "can_edit", SqlValue = canEdit, SqlDbType = SqlDbType.Bit };
            SqlParameter pConfirmed = new SqlParameter() { ParameterName = "@confirmed", SqlValue = confirmed, SqlDbType = SqlDbType.Bit };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = creatorSid, SqlDbType = SqlDbType.VarChar };

            var dt = Db.Stuff.ExecuteQueryStoredProcedure("rest_holiday_list_confirm", pEmployeeSid, pYear, pCanEdit, pConfirmed, pCreatorAdSid);
        }

        /// <summary>
        /// Список периодов отпусков
        /// </summary>
        /// <param name="employeeSid">Фильтр по конкретному сотруднику</param>
        /// <param name="year">Фильтр по году</param>
        /// <returns></returns>
        public static IEnumerable<RestHoliday> GetList(string employeeSid = null, int? year = null)
        {
            SqlParameter pEmployeeSid = new SqlParameter() { ParameterName = "employee_sid", SqlValue = employeeSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pYear = new SqlParameter() { ParameterName = "year", SqlValue = year, SqlDbType = SqlDbType.Int };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("rest_holiday_list", pEmployeeSid, pYear);

            var lst = new List<RestHoliday>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new RestHoliday(row);
                lst.Add(model);
            }

            return lst;
        }

        public static IEnumerable<EmployeeRestHoliday> GetEmployeeList(int? year = null)
        {
            SqlParameter pYear = new SqlParameter() { ParameterName = "year", SqlValue = year, SqlDbType = SqlDbType.Int };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("rest_holiday_employee_list", pYear);

            var lst = new List<EmployeeRestHoliday>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new EmployeeRestHoliday(row);
                model.Residue = RestHolidaysMaxCount - model.DurationSum;
                lst.Add(model);
            }

            return lst;
        }

        public static void Close(int id, string deleterSid)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            SqlParameter pDeleterSid = new SqlParameter() { ParameterName = "deleter_sid", SqlValue = deleterSid, SqlDbType = SqlDbType.VarChar };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("rest_holiday_close", pId, pDeleterSid);
        }

        /// <summary>
        /// Список годов для сотрудника для сохранения и просмотра графика отпусков + текущий и будущий если там нет записей)
        /// </summary>
        /// <param name="employeeSid"></param>
        /// <param name="topRows">Ограничение по количеству выводимых годов</param>
        /// <param name="year">Годя для которого нужно получить количество оставшихся свободных дней отпуска</param>
        /// <returns>Год и остатоу дней отпуска в году</returns>
        public static IEnumerable<KeyValuePair<int, int>> GetYears4Employee(string employeeSid = null, int? topRows = null, int? year = null)
        {
            SqlParameter pEmployeeSid = new SqlParameter() { ParameterName = "employee_sid", SqlValue = employeeSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pTopRows = new SqlParameter() { ParameterName = "top_rows", SqlValue = topRows, SqlDbType = SqlDbType.Int };
            SqlParameter pYear = new SqlParameter() { ParameterName = "year", SqlValue = year, SqlDbType = SqlDbType.Int };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("rest_holiday_years_list", pEmployeeSid, pTopRows, pYear);

            int restHolidaysMaxCount;
            int.TryParse(ConfigurationManager.AppSettings["restHolidaysMaxCount"], out restHolidaysMaxCount);

            var lst = new List<KeyValuePair<int, int>>();

            foreach (DataRow row in dt.Rows)
            {
                int daysCount = Db.DbHelper.GetValueIntOrDefault(row, "days_count");
                var model = new KeyValuePair<int, int>(Db.DbHelper.GetValueIntOrDefault(row, "year"), restHolidaysMaxCount - daysCount);
                lst.Add(model);
            }

            return lst;
        }
    }
}