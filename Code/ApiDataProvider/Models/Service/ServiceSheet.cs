using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Models.Stuff;
using DataProvider.Objects;
using DataProvider._TMPLTS;

namespace DataProvider.Models.Service
{
    public class ServiceSheet:DbModel
    {
        public int Id { get; set; }
        public int IdServiceIssue { get; set; }
        public int IdClaim { get; set; }
        public int IdClaim2ClaimState { get; set; }
        public bool ProcessEnabled { get; set; }
        public bool DeviceEnabled { get; set; }
        public bool? ZipClaim { get; set; }
        public string ZipClaimNumber { get; set; }
        public int? CounterMono { get; set; }
        public int? CounterColor { get; set; }
        public int? CounterTotal { get; set; }
        public bool? NoCounter { get; set; }
        public string Descr { get; set; }
        public bool? CounterUnavailable { get; set; }
        public string CounterDescr { get; set; }
        public string CreatorSid { get; set; }
        public string EngeneerSid { get; set; }
        public EmployeeSm Admin { get; set; }
        public EmployeeSm Creator { get; set; }
        public EmployeeSm Engeneer { get; set; }
        public string AdminSid { get; set; }
        //public string DeviceSerialNum { get; set; }
        public int DeviceId { get; set; }
        public Device Device { get; set; }
        public ClassifierCaterory DeviceClassifierCaterory { get; set; }
        public int WorkTypeId { get; set; }
        public WorkType WorkType { get; set; }
        //Время на работу в минутах (от статуса В работе до создания заявки
        public int? TimeOnWorkMinutes { get; set; }
        public string ClientSdNum { get; set; }
        public DateTime DateCreate { get; set; }
        public string NotInstalledComment { get; set; }
        public int? UnitProgZipClaimId { get; set; }
        /// <summary>
        /// установлен ЗИП предоставленый клиентом
        /// </summary>
        public bool ZipClientGivenInstall { get; set; }
        /// <summary>
        /// Оплачен или нет
        /// </summary>
        public bool? IsPayed { get; set; }
        public string NotPayedComment { get; set; }
        public string IsPayedCreatorSid { get; set; }
        /// <summary>
        /// Для сохранения серийного номера аппарата если он без номера (БН)
        /// </summary>
        public string RealSerialNum { get; set; }
        /// <summary>
        /// Признак чтобы привязвать оборудвание без номра (БН) к существующему3 ели такой уже есть
        /// </summary>
        public bool? ForceSaveRealSerialNum { get; set; }

        public int? RealDeviceModel { get; set; }

        public ServiceSheet() { }

        public ServiceSheet(int id)
        {
            if (id <= 0) throw new ArgumentException("Не указан номер Сервисного листа");
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_service_sheet", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row, true, true);
            }
        }

        public ServiceSheet(DataRow row, bool fillObj = false)
            : this()
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row, bool fillObj = false, bool fillNames = false)
        {
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            IdClaim = Db.DbHelper.GetValueIntOrDefault(row, "id_claim");
            IdClaim2ClaimState = Db.DbHelper.GetValueIntOrDefault(row, "id_claim2claim_state");
            ProcessEnabled = Db.DbHelper.GetValueBool(row, "process_enabled");
            DeviceEnabled = Db.DbHelper.GetValueBool(row, "device_enabled");
            ZipClaim = Db.DbHelper.GetValueBoolOrNull(row, "zip_claim");
            ZipClaimNumber = Db.DbHelper.GetValueString(row, "zip_claim_number");
            CounterMono = Db.DbHelper.GetValueIntOrNull(row, "counter_mono");
            CounterColor = Db.DbHelper.GetValueIntOrNull(row, "counter_color");
            CounterTotal = Db.DbHelper.GetValueIntOrNull(row, "counter_total");
            NoCounter = Db.DbHelper.GetValueBoolOrNull(row, "no_counter");
            Descr = Db.DbHelper.GetValueString(row, "descr");
            CounterUnavailable = Db.DbHelper.GetValueBoolOrNull(row, "counter_unavailable");
            CounterDescr = Db.DbHelper.GetValueString(row, "counter_descr");
            CreatorSid = Db.DbHelper.GetValueString(row, "creator_sid");
            EngeneerSid = Db.DbHelper.GetValueString(row, "engeneer_sid");
            AdminSid = Db.DbHelper.GetValueString(row, "admin_sid");
            DeviceId = Db.DbHelper.GetValueIntOrDefault(row, "id_device");
            WorkTypeId = Db.DbHelper.GetValueIntOrDefault(row, "id_work_type");
            TimeOnWorkMinutes = Db.DbHelper.GetValueIntOrNull(row, "time_on_work_minutes");
            ClientSdNum = Db.DbHelper.GetValueString(row, "client_sd_num");
            DateCreate = Db.DbHelper.GetValueDateTimeOrDefault(row, "date_create");
            NotInstalledComment = Db.DbHelper.GetValueString(row, "not_installed_comment");
            UnitProgZipClaimId = Db.DbHelper.GetValueIntOrNull(row, "unit_prog_zip_claim_id");
            ZipClientGivenInstall = Db.DbHelper.GetValueBool(row, "zip_client_given_install");
            IsPayed = Db.DbHelper.GetValueBoolOrNull(row, "is_payed");
            NotPayedComment = Db.DbHelper.GetValueString(row, "not_payed_comment");
            IsPayedCreatorSid = Db.DbHelper.GetValueString(row, "is_payed_creator_sid");
            RealSerialNum = Db.DbHelper.GetValueString(row, "real_serial_num");
            ForceSaveRealSerialNum = Db.DbHelper.GetValueBoolOrNull(row, "force_save_real_serial_num");
            RealDeviceModel = Db.DbHelper.GetValueIntOrNull(row, "real_device_model_id");

            if (fillNames)
            {
                Admin = new EmployeeSm(AdminSid);
                Engeneer = new EmployeeSm(EngeneerSid);
                Creator = new EmployeeSm(CreatorSid);
            }

            if (fillObj)
            {
                Device = new Device(DeviceId);
                WorkType = new WorkType(WorkTypeId);
                DeviceClassifierCaterory = new ClassifierCaterory(Device.ClassifierCategoryId);
            }
        }

        public void Save(string lastStateSysName)
        {
            //TimeOnWorkMinutes = время от статуса в работу до создания заявки
            if (!TimeOnWorkMinutes.HasValue)
            {
                var stateInWork = Claim.GetLastState(IdClaim, lastStateSysName);
                if (stateInWork != null)
                {
                    TimeOnWorkMinutes = (int) (DateTime.Now - stateInWork.DateCreate).TotalMinutes;
                    if (TimeOnWorkMinutes == 0) TimeOnWorkMinutes = 1;
                }
            }

            //Получаем ткущий тип работ по заявке
            if (WorkTypeId <= 0)
            {
                int? wtId = new Claim(IdClaim).IdWorkType;
                if (wtId.HasValue) WorkTypeId = wtId.Value;
            }
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            SqlParameter pIdClaim = new SqlParameter() { ParameterName = "id_claim", SqlValue = IdClaim, SqlDbType = SqlDbType.Int };
            SqlParameter pWorkTypeId = new SqlParameter() { ParameterName = "id_work_type", SqlValue = WorkTypeId, SqlDbType = SqlDbType.Int };
            SqlParameter pIdServiceIssue = new SqlParameter() { ParameterName = "id_service_issue", SqlValue = IdServiceIssue, SqlDbType = SqlDbType.Int };
            SqlParameter pProcessEnabled = new SqlParameter() { ParameterName = "process_enabled", SqlValue = ProcessEnabled, SqlDbType = SqlDbType.Bit };
            SqlParameter pDeviceEnabled = new SqlParameter() { ParameterName = "device_enabled", SqlValue = DeviceEnabled, SqlDbType = SqlDbType.Bit };
            SqlParameter pZipClaim = new SqlParameter() { ParameterName = "zip_claim", SqlValue = ZipClaim, SqlDbType = SqlDbType.Int };
            SqlParameter pZipClaimNumber = new SqlParameter() { ParameterName = "zip_claim_number", SqlValue = ZipClaimNumber, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCounterMono = new SqlParameter() { ParameterName = "counter_mono", SqlValue = CounterMono, SqlDbType = SqlDbType.BigInt };
            SqlParameter pCounterColor = new SqlParameter() { ParameterName = "counter_color", SqlValue = CounterColor, SqlDbType = SqlDbType.BigInt };
            SqlParameter pCounterTotal = new SqlParameter() { ParameterName = "counter_total", SqlValue = CounterTotal, SqlDbType = SqlDbType.BigInt };
            SqlParameter pNoCounter = new SqlParameter() { ParameterName = "no_counter", SqlValue = NoCounter, SqlDbType = SqlDbType.Bit };
            SqlParameter pCounterUnavailable = new SqlParameter() { ParameterName = "counter_unavailable", SqlValue = CounterUnavailable, SqlDbType = SqlDbType.Bit };
            SqlParameter pDescr = new SqlParameter() { ParameterName = "descr", SqlValue = Descr, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pCounterDescr = new SqlParameter() { ParameterName = "counter_descr", SqlValue = CounterDescr, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pEngeneerSid = new SqlParameter() { ParameterName = "engeneer_sid", SqlValue = EngeneerSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pAdminSid = new SqlParameter() { ParameterName = "admin_sid", SqlValue = AdminSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pTimeOnWorkMinutes = new SqlParameter() { ParameterName = "time_on_work_minutes", SqlValue = TimeOnWorkMinutes, SqlDbType = SqlDbType.Int };
            SqlParameter pZipClientGivenInstall = new SqlParameter() { ParameterName = "zip_client_given_install", SqlValue = ZipClientGivenInstall, SqlDbType = SqlDbType.Bit };
            SqlParameter pRealSerialNum = new SqlParameter() { ParameterName = "real_serial_num", SqlValue = RealSerialNum, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pForceSaveRealSerialNum = new SqlParameter() { ParameterName = "force_save_real_serial_num", SqlValue = ForceSaveRealSerialNum, SqlDbType = SqlDbType.Bit };
            SqlParameter pRealDeviceModelId = new SqlParameter() { ParameterName = "real_device_model_id", SqlValue = RealDeviceModel, SqlDbType = SqlDbType.Int };

            var dt = Db.Service.ExecuteQueryStoredProcedure("save_service_sheet", pId, pProcessEnabled, pDeviceEnabled, pZipClaim, pZipClaimNumber, pCounterMono, pCounterColor, pCounterTotal, pNoCounter, pCounterUnavailable, pDescr, pCreatorAdSid, pCounterDescr, pEngeneerSid, pAdminSid, pIdServiceIssue, pIdClaim, pTimeOnWorkMinutes, pWorkTypeId, pZipClientGivenInstall, pRealSerialNum, pForceSaveRealSerialNum, pRealDeviceModelId);
            int id = 0;
            if (dt.Rows.Count > 0)
            {
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;
            }
        }

        public static IEnumerable<ServiceSheet> GetList(int? idClaim=null, int? idClaim2ClaimState = null)
        {
            SqlParameter pIdClaim = new SqlParameter() { ParameterName = "id_claim", SqlValue = idClaim, SqlDbType = SqlDbType.Int };
            SqlParameter pIdClaim2ClaimState = new SqlParameter() { ParameterName = "id_claim2claim_state", SqlValue = idClaim2ClaimState, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_service_sheet", pIdClaim, pIdClaim2ClaimState);

            var lst = new List<ServiceSheet>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new ServiceSheet(row);
                lst.Add(model);
            }

            return lst;
        }

        public IEnumerable<ServiceSheetZipItem> GetIssuedZipItemList()
        {
            return ServiceSheetZipItem.GetIssuedList(Id);
        }

        public IEnumerable<ServiceSheetZipItem> GetOrderedZipItemList(bool? realyOrdered = null)
        {
            return ServiceSheetZipItem.GetOrderedList(Id, realyOrdered);
        }

        public void SetOrderedZipItemListRealyOrdered()
        {
            ServiceSheetZipItem.SetOrderedListRealyOrdered(Id, CurUserAdSid);
        }

        public void SaveNotInstalledComment()
        {
            SqlParameter pNotInstalledComment = new SqlParameter() { ParameterName = "not_installed_comment", SqlValue = NotInstalledComment, SqlDbType = SqlDbType.Int };
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("service_sheet_update", pNotInstalledComment, pId);
        }

        public void SaveUnitProgZipClaimId()
        {
            SqlParameter pNotInstalledComment = new SqlParameter() { ParameterName = "unit_prog_zip_claim_id", SqlValue = UnitProgZipClaimId, SqlDbType = SqlDbType.Int };
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("service_sheet_update", pNotInstalledComment, pId);
        }

        public void SavePayed(AdUser user)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            SqlParameter pIsPayed = new SqlParameter() { ParameterName = "is_payed", SqlValue = IsPayed, SqlDbType = SqlDbType.Bit };
            SqlParameter pNotPayedComment = new SqlParameter() { ParameterName = "not_payed_comment", SqlValue = NotPayedComment, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pIsPayedCreatorSid = new SqlParameter() { ParameterName = "is_payed_creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };
            var dt = Db.Service.ExecuteQueryStoredProcedure("service_sheet_update", pId, pIsPayed, pNotPayedComment, pIsPayedCreatorSid);

            //Отправка уведомления инженеру
            Claim.ServiceSheetIsPayWraped(user, Id, CurUserAdSid);
        }

        //public static IEnumerable<ServiceSheet> GetClaimServiceSheetList(int idClaim)
        //{
        //    SqlParameter pIdClaim = new SqlParameter() { ParameterName = "id_claim", SqlValue = idClaim, SqlDbType = SqlDbType.Int };
        //    var dt = Db.Service.ExecuteQueryStoredProcedure("get_claim_service_sheet_list", pIdClaim);

        //    return (from DataRow row in dt.Rows select new ServiceSheet(row)).ToList();
        //}

        //public static void Close(int id, string deleterSid)
        //{
        //    SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
        //    SqlParameter pDeleterSid = new SqlParameter() { ParameterName = "deleter_sid", SqlValue = deleterSid, SqlDbType = SqlDbType.VarChar };
        //    var dt = Db.Stuff.ExecuteQueryStoredProcedure("close_model", pId, pDeleterSid);
        //}
    }
}