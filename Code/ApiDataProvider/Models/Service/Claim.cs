﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Models.Stuff;
using DataProvider.Objects;
using DataProvider.Objects.Interfaces;

namespace DataProvider.Models.Service
{
    public class Claim : DbModel
    {
        public int Id { get; set; }
        public string Sid { get; set; }
        public Contractor Contractor { get; set; }
        public Contract Contract { get; set; }
        public Device Device { get; set; }
        public string ContractorName { get; set; }
        public string ContractName { get; set; }
        public string DeviceName { get; set; }
        public EmployeeSm Admin { get; set; }
        public EmployeeSm Engeneer { get; set; }
        public ClaimState State { get; set; }
        public DateTime DateStateChange { get; set; }
        public int? IdWorkType { get; set; }
        public WorkType WorkType { get; set; }
        public string SpecialistSid { get; set; }
        public EmployeeSm Specialist { get; set; }
        public ServiceSheet ServiceSheet4Save { get; set; }
        public DateTime DateCreate { get; set; }
        public ServiceIssue ServiceIssue4Save { get; set; }
        public string ClientSdNum { get; set; }
        public string Descr { get; set; }
        public string ChangerSid { get; set; }
        public string CurEngeneerSid { get; set; }
        public string CurAdminSid { get; set; }
        public string CurTechSid { get; set; }
        public string CurManagerSid { get; set; }

        public Claim() { }

        public Claim(int id, bool getNames = false)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_claim", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
                if (getNames) GetNames();
            }

            //if (!UserCanViewClaimNow(user))
            //{
            //    throw new AccessDenyException($"В настоящий момент у вас нет доступа к заявке №{id}.");
            //}
        }

        public Claim(int id, AdUser user, bool getNames = false) : this(id, getNames)
        {
            //SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            //var dt = Db.Service.ExecuteQueryStoredProcedure("get_claim", pId);
            //if (dt.Rows.Count > 0)
            //{
            //    var row = dt.Rows[0];
            //    FillSelf(row);
            //    if (getNames) GetNames();
            //}

            if (!UserCanViewClaimNow(user))
            {
                throw new AccessDenyException($"В настоящий момент у вас нет доступа к заявке №{id}.");
            }
        }

        /// <summary>
        /// Проверка есть ли у пользователя доступ к заявке
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userSid"></param>
        /// <returns></returns>
        private bool UserCanViewClaimNow(AdUser user)
        {
            bool result = false;
            if (State == null) State = new ClaimState();
            SqlParameter pIdClaimState = new SqlParameter() { ParameterName = "id_claim_state", SqlValue = State.Id, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_claim_state_user_group_list", pIdClaimState);

            var grpList = new List<AdGroup>();
            foreach (DataRow dr in dt.Rows)
            {
                string groupSid = Db.DbHelper.GetValueString(dr, "user_group_sid");
                var grp = AdUserGroup.GetAdGroupBySid(groupSid);
                grpList.Add(grp);
            }
            grpList.Add(AdGroup.ServiceControler);
            result = AdHelper.UserInGroup(user.User, grpList.ToArray());
            return result;
        }

        public Claim(DataRow row, bool getNames = false)
            : this()
        {
            FillSelf(row);
            if (getNames) GetNames();
        }

        private void GetNames()
        {
            Contractor = new Contractor(Contractor.Id);
            Contract = new Contract(Contract.Id);
            Device = new Device(Device.Id);
        }

        private void FillSelf(DataRow row)
        {
            Sid = Db.DbHelper.GetValueString(row, "sid");
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            Contractor = new Contractor() { Id = Db.DbHelper.GetValueIntOrDefault(row, "id_contractor") };
            Contract = new Contract() { Id = Db.DbHelper.GetValueIntOrDefault(row, "id_contract") };
            Device = new Device() { Id = Db.DbHelper.GetValueIntOrDefault(row, "id_device") };
            ContractorName = Db.DbHelper.GetValueString(row, "contractor_name");
            ContractName = Db.DbHelper.GetValueString(row, "contract_name");
            DeviceName = Db.DbHelper.GetValueString(row, "device_name");
            Admin = new EmployeeSm(Db.DbHelper.GetValueIntOrDefault(row, "id_admin"));
            Engeneer = new EmployeeSm(Db.DbHelper.GetValueIntOrDefault(row, "id_engeneer"));
            State = new ClaimState(Db.DbHelper.GetValueIntOrDefault(row, "id_claim_state"));
            IdWorkType = Db.DbHelper.GetValueIntOrNull(row, "id_work_type");
            if (IdWorkType.HasValue && IdWorkType.Value > 0) WorkType = new WorkType(IdWorkType.Value);
            SpecialistSid = Db.DbHelper.GetValueString(row, "specialist_sid");
            Specialist = new EmployeeSm(SpecialistSid);
            DateCreate = Db.DbHelper.GetValueDateTimeOrDefault(row, "date_create");
            DateStateChange = Db.DbHelper.GetValueDateTimeOrDefault(row, "date_state_change");
            ClientSdNum = Db.DbHelper.GetValueString(row, "client_sd_num");
            ChangerSid = Db.DbHelper.GetValueString(row, "changer_sid");
            CurEngeneerSid = Db.DbHelper.GetValueString(row, "cur_engeneer_sid");
            CurAdminSid = Db.DbHelper.GetValueString(row, "cur_admin_sid");
            CurTechSid = Db.DbHelper.GetValueString(row, "cur_tech_sid");
            CurManagerSid = Db.DbHelper.GetValueString(row, "cur_manager_sid");
        }

        public int Save()
        {
            //if (State == null) State = ClaimState.GetFirstState();
            if (Admin == null) Admin = new EmployeeSm();
            if (Engeneer == null) Engeneer = new EmployeeSm();
            if (Contract == null) Contract = new Contract();
            if (Contractor == null) Contractor = new Contractor();
            if (Device == null) Device = new Device();

            //string wtReplace = "%work_type%";
            //if (Descr.IndexOf(wtReplace, StringComparison.Ordinal) > 0)
            //{
            //    var wt = new WorkType(IdWorkType);
            //    Descr = Descr.Replace(wtReplace, $"{wt.SysName} ({wt.Name})");
            //}

            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            SqlParameter pIdContractor = new SqlParameter() { ParameterName = "id_contractor", SqlValue = Contractor.Id, SqlDbType = SqlDbType.Int };
            SqlParameter pIdContract = new SqlParameter() { ParameterName = "id_contract", SqlValue = Contract.Id, SqlDbType = SqlDbType.Int };
            SqlParameter pIdDevice = new SqlParameter() { ParameterName = "id_device", SqlValue = Device.Id, SqlDbType = SqlDbType.Int };
            SqlParameter pContractorName = new SqlParameter() { ParameterName = "contractor_name", SqlValue = ContractorName, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pContractName = new SqlParameter() { ParameterName = "contract_number", SqlValue = ContractName, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pDeviceName = new SqlParameter() { ParameterName = "device_name", SqlValue = DeviceName, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pIdAdmin = new SqlParameter() { ParameterName = "id_admin", SqlValue = Admin.Id, SqlDbType = SqlDbType.Int };
            SqlParameter pIdEngeneer = new SqlParameter() { ParameterName = "id_engeneer", SqlValue = Engeneer.Id, SqlDbType = SqlDbType.Int };
            //Статус сохраняем отдельной процедурой так как надо хранить историю
            //SqlParameter pIdClaimState = new SqlParameter() { ParameterName = "id_claim_state", SqlValue = State.Id, SqlDbType = SqlDbType.Int };
            SqlParameter pIdWorkType = new SqlParameter() { ParameterName = "id_work_type", SqlValue = IdWorkType, SqlDbType = SqlDbType.Int };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pSpecialistSid = new SqlParameter() { ParameterName = "specialist_sid", SqlValue = SpecialistSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pClientSdNum = new SqlParameter() { ParameterName = "client_sd_num", SqlValue = ClientSdNum, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pEngeneerSid = new SqlParameter() { ParameterName = "cur_engeneer_sid", SqlValue = CurEngeneerSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pAdminSid = new SqlParameter() { ParameterName = "cur_admin_sid", SqlValue = CurAdminSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pTechSid = new SqlParameter() { ParameterName = "cur_tech_sid", SqlValue = CurTechSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pManagerSid = new SqlParameter() { ParameterName = "cur_manager_sid", SqlValue = CurManagerSid, SqlDbType = SqlDbType.VarChar };
            DataTable dt = new DataTable();
            //using (var conn = Db.Service.connection)
            //{
            //    if (conn.State == ConnectionState.Closed) conn.Open();
            //    using (SqlTransaction tran = conn.BeginTransaction())
            //    {
            //        try
            //        {

            //Если заявка уже сохранена то основная информаци не будет перезаписана
            dt = Db.Service.ExecuteQueryStoredProcedure("save_claim", pId, pIdContractor, pIdContract, pIdDevice,
                pContractorName, pContractName, pDeviceName, pIdAdmin, pIdEngeneer, pCreatorAdSid, pIdWorkType, pSpecialistSid, pClientSdNum, pEngeneerSid, pAdminSid, pTechSid, pManagerSid);

            int id = 0;
            if (dt.Rows.Count > 0)
            {
                Int32.TryParse(dt.Rows[0]["id"].ToString(), out id);
            }

            if (Id == 0)
            {
                Id = id;
                var st = ClaimState.GetFirstState();
                SaveStateStep(st.Id, Descr);
            }

            Id = id;

            ////////Сохранение статуса заявки, может быть передан новый статус тогда сохраняем с новым статусом
            //////if (Id > 0)
            //////{
            //////    int stateId = 0;
            //////    if (nextStateId.HasValue)
            //////    {
            //////        stateId = nextStateId.Value;
            //////    }
            //////    else
            //////    {
            //////        stateId = GetClaimCurrentState(Id).Id;
            //////    }
            //////    if (stateId > 0) SaveStateStep(stateId, sysDescr, saveStateInfo);
            //////}

            return Id;
        }

        public static void RemoteStateChange(int idClaim, string stateSysName, string creatorSid, string descr=null)
        {
            var claim = new Claim(idClaim);
            claim.CurUserAdSid =creatorSid;
            if (claim.Id > 0)
            {
                var state = new ClaimState(stateSysName);
                claim.SaveStateStep(state.Id, descr);
            }
        }

        public void SaveStateStep(int stateId, string descr = null, bool saveStateInfo = true)
        {
            if (stateId == 0) throw new ArgumentException("Не указан статус для сохранения в лестнице статусов.");
            var c2Cs = new Claim2ClaimState();
            c2Cs.IdClaim = Id;
            c2Cs.IdClaimState = stateId;
            c2Cs.CurUserAdSid = CurUserAdSid;
            if (saveStateInfo)
            {
                //c2Cs.Descr = Descr;
                //if (String.IsNullOrEmpty(Descr))
                //{
                //    Descr += "\r\n";
                //}
                c2Cs.Descr = descr;
                if (IdWorkType.HasValue) c2Cs.IdWorkType = IdWorkType.Value;
                c2Cs.SpecialistSid = SpecialistSid;
                if (ServiceSheet4Save != null && ServiceSheet4Save.Id > 0)
                {
                    c2Cs.IdServiceSheet = ServiceSheet4Save.Id;
                }
            }
            c2Cs.Save();
        }

        public void Clear(bool specialist = false, bool workType = false, bool engeneer = false, bool admin = false, bool tech = false, bool manager = false)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            SqlParameter pClearSpecialistSid = new SqlParameter() { ParameterName = "clear_specialist_sid", SqlValue = specialist, SqlDbType = SqlDbType.Bit };
            SqlParameter pClearIdWorkType = new SqlParameter() { ParameterName = "clear_id_work_type", SqlValue = workType, SqlDbType = SqlDbType.Bit };
            SqlParameter pClearEngeneerSid = new SqlParameter() { ParameterName = "clear_engeneer_sid", SqlValue = engeneer, SqlDbType = SqlDbType.Bit };
            SqlParameter pClearAdminSid = new SqlParameter() { ParameterName = "clear_admin_sid", SqlValue = admin, SqlDbType = SqlDbType.Bit };
            SqlParameter pClearTechSid = new SqlParameter() { ParameterName = "clear_tech_sid", SqlValue = tech, SqlDbType = SqlDbType.Bit };
            SqlParameter pClearManagerSid = new SqlParameter() { ParameterName = "clear_manager_sid", SqlValue = manager, SqlDbType = SqlDbType.Bit };
            if (specialist)
            {
                pClearSpecialistSid.Value = true;
            }
            DataTable dt = new DataTable();

            dt = Db.Service.ExecuteQueryStoredProcedure("clear_claim", pId, pClearSpecialistSid, pClearIdWorkType, pClearEngeneerSid, pClearAdminSid, pClearTechSid, pClearManagerSid);
        }

        public void Go(bool confirm = true)
        {
            if (Id <= 0) throw new ArgumentException("Невозможно предать заявку. Не указан ID заявки.");

            //Save();
            string descr = String.Empty;
            var currState = GetClaimCurrentState(Id);
            var nextState = new ClaimState();
            bool saveStateInfo = true;

            switch (currState.SysName.ToUpper())
            {
                //case "NEW":
                //    nextState = new ClaimState("NEWADD");
                //    break;
                case "NEW":
                    int? wtId = null;
                    if (!IdWorkType.HasValue)
                    {
                        wtId = new Claim(Id).IdWorkType;
                        if (!wtId.HasValue) throw new ArgumentException("Невозможно определить следующий статус. Тип работ заявки не указан.");
                    }
                    else
                    {
                        wtId = IdWorkType;
                    }
                    var wtSysName = new WorkType(wtId.Value).SysName;
                    descr = $"{Descr}\r\nУстановлен тип работ {wtSysName}\r\nНазначен специалист {AdHelper.GetUserBySid(SpecialistSid).FullName}";

                    switch (wtSysName)
                    {
                        case "ДНО":
                        case "НПР":
                        case "ТЭО":
                        case "УТЗ":
                            nextState = new ClaimState("TECHSET");
                            CurTechSid = SpecialistSid;
                            break;
                        case "РТО":
                        case "МТС":
                        case "УРМ":
                        case "ЗРМ":
                        case "МДО":
                        case "ИПТ":
                        case "РЗРД":
                        case "ЗНЗЧ":
                            nextState = new ClaimState("SERVADMSET");
                            CurAdminSid = SpecialistSid;
                            break;
                    }
                    break;
                case "TECHSET":
                    if (confirm)
                    {
                        nextState = new ClaimState("TECHWORK");
                    }
                    else
                    {
                        descr = $"Отклонено\r\n{Descr}";
                        nextState = new ClaimState("NEW");
                        //Очищаем выбранного специалиста так как статус заявки поменялся
                        Clear(specialist: true, workType: true, tech:true);
                    }
                    break;
                case "TECHWORK":
                    if (confirm)
                    {
                        ServiceSheet4Save.IdClaim = Id;
                        if (ServiceSheet4Save == null || ServiceSheet4Save.IdClaim == 0)
                        {
                            throw new ArgumentException("Сервисный лист отсутствует. Операция не завершена!");
                        }

                        ////if (!ServiceSheet4Save.NoTechWork)
                        ////{
                        ServiceSheet4Save.CurUserAdSid = CurUserAdSid;
                        ServiceSheet4Save.Save();
                        if (ServiceSheet4Save.ProcessEnabled && ServiceSheet4Save.DeviceEnabled)
                        {
                            nextState = new ClaimState("TECHDONE");
                            //Сначала сохраняем промежуточный статус
                            SaveStateStep(nextState.Id);
                            saveStateInfo = false;
                            nextState = new ClaimState("DONE");

                        }
                        else if ((!ServiceSheet4Save.ProcessEnabled || !ServiceSheet4Save.DeviceEnabled) && ServiceSheet4Save.ZipClaim.HasValue && ServiceSheet4Save.ZipClaim.Value)
                        {
                            nextState = new ClaimState("TECHPROCESSED");
                            //Сначала сохраняем промежуточный статус
                            SaveStateStep(nextState.Id);
                            saveStateInfo = false;
                            nextState = new ClaimState("ZIPCHECK");
                        }
                        else if ((!ServiceSheet4Save.ProcessEnabled || !ServiceSheet4Save.DeviceEnabled) && (!ServiceSheet4Save.ZipClaim.HasValue || !ServiceSheet4Save.ZipClaim.Value))
                        {
                            
                            nextState = new ClaimState("TECHNODONE");
                            //Сначала сохраняем промежуточный статус
                            SaveStateStep(nextState.Id);
                            saveStateInfo = false;
                            nextState = new ClaimState("SERVADMSETWAIT");
                        }
                    }
                    else
                    {
                        descr = $"{Descr}";
                        nextState = new ClaimState("TECHNOCONTACT");
                        //Сначала сохраняем промежуточный статус
                        SaveStateStep(nextState.Id, descr);
                        saveStateInfo = false;
                        nextState = new ClaimState("SERVADMSETWAIT");
                        Clear(specialist: true);
                    }
                    ////}
                    ////else if (ServiceSheet4Save.NoTechWork)
                    ////{
                    ////    nextState = new ClaimState("NEW");
                    ////    //Очищаем выбранного специалиста так как статус заявки поменялся
                    ////    Clear(specialist: true);
                    ////}
                    break;
                case "SERVADMSETWAIT":
                    nextState = new ClaimState("SERVADMSET");
                    break;
                case "SERVADMSET":
                    if (confirm)
                    {
                        nextState = new ClaimState("SRVADMWORK");
                        CurAdminSid = SpecialistSid;
                    }
                    else
                    {
                        descr = $"Отклонено\r\n{Descr}";
                        nextState = new ClaimState("NEW");
                        //Очищаем выбранного специалиста так как статус заявки поменялся
                        Clear(specialist: true, admin:true);
                    }
                    break;
                case "SRVADMWORK":
                    ServiceIssue4Save.IdClaim = Id;
                    ServiceIssue4Save.Descr = Descr;
                    ServiceIssue4Save.SpecialistSid = SpecialistSid;
                    ServiceIssue4Save.CurUserAdSid = CurUserAdSid;
                    ServiceIssue4Save.Save();
                    descr = $"Назначен специалист {AdHelper.GetUserBySid(SpecialistSid).FullName}\r\nДата выезда {ServiceIssue4Save.DatePlan:dd.MM.yyyy}\r\n{Descr}";
                    nextState = new ClaimState("SRVENGSET");
                    CurEngeneerSid = SpecialistSid;
                    break;
                case "SERVENGSETWAIT":
                    nextState = new ClaimState("SRVENGSET");
                    break;
                case "SRVENGSET":
                    if (confirm)
                    {
                        nextState = new ClaimState("SRVENGGET");
                        //Сначала сохраняем промежуточный статус
                        SaveStateStep(nextState.Id);
                        saveStateInfo = false;
                        nextState = new ClaimState("SERVENGOUTWAIT");
                    }
                    else
                    {
                        descr = $"Отклонено\r\n{Descr}";
                        nextState = new ClaimState("SRVENGCANCEL");
                        //Сначала сохраняем промежуточный статус
                        SaveStateStep(nextState.Id, descr);
                        saveStateInfo = false;
                        nextState = new ClaimState("SRVADMWORK");
                        Clear(specialist: true, engeneer:true);
                    }
                    break;

                case "SERVENGOUTWAIT":
                    nextState = new ClaimState("SRVENGWENT");
                    break;
                case "SRVENGWENT":
                    nextState = new ClaimState("SRVENGWORK");
                    break;
                case "SRVENGWORK":
                    ServiceSheet4Save.IdClaim = Id;
                    if (ServiceSheet4Save == null || ServiceSheet4Save.IdClaim == 0)
                    { throw new ArgumentException("Сервисный лист отсутствует. Операция не завершена!"); }

                    ServiceSheet4Save.CurUserAdSid = CurUserAdSid;
                    ServiceSheet4Save.Save();

                    if (ServiceSheet4Save.ProcessEnabled && ServiceSheet4Save.DeviceEnabled)
                    {
                        nextState = new ClaimState("DONE");

                    }
                    else if ((!ServiceSheet4Save.ProcessEnabled || !ServiceSheet4Save.DeviceEnabled) && ServiceSheet4Save.ZipClaim.HasValue &&
                             ServiceSheet4Save.ZipClaim.Value)
                    {
                        nextState = new ClaimState("ZIPCHECK");
                    }
                    else if ((!ServiceSheet4Save.ProcessEnabled || !ServiceSheet4Save.DeviceEnabled) && (!ServiceSheet4Save.ZipClaim.HasValue || !ServiceSheet4Save.ZipClaim.Value))
                    {
                        nextState = new ClaimState("ZIPCHECK");
                    }

                    break;
                case "ZIPCHECK"://В настоящий момент по этому статусу происходит заказ ЗИП специалистом Тех поддержки
                    nextState = new ClaimState("ZIPCHECKED");
                    break;
                case "ZIPCHECKED":
                    nextState = new ClaimState("ZIPCONFIRMED");
                    break;
                case "ZIPCONFIRMED":
                    nextState = new ClaimState("ZIPORDERED");
                    break;
                case "ZIPORDERED":
                    if (!confirm)
                    {
                        nextState = new ClaimState("ZIPBUYCANCEL");
                        break;
                    }
                    else
                    {
                        nextState = new ClaimState("ZIPINSTWAIT");
                        //Сначала сохраняем промежуточный статус
                        SaveStateStep(nextState.Id);
                        saveStateInfo = false;
                        nextState = new ClaimState("SERVENGSETWAIT");
                        break;
                    }
                case "DONE":
                    nextState = new ClaimState("END");
                    break;
                default:
                    nextState = currState;
                    break;
            }
            Save();
            SaveStateStep(nextState.Id, descr, saveStateInfo);
            //SaveStateStep(nextState.Id);
        }



        public static IEnumerable<Claim> GetList(out int cnt, int? idAdmin = null, int? idEngeneer = null, DateTime? dateStart = null, DateTime? dateEnd = null, int? topRows = 30)
        {
            SqlParameter pIdAdmin = new SqlParameter() { ParameterName = "id_admin", SqlValue = idAdmin, SqlDbType = SqlDbType.Int };
            SqlParameter pIdEngeneer = new SqlParameter() { ParameterName = "id_engeneer", SqlValue = idEngeneer, SqlDbType = SqlDbType.Int };
            SqlParameter pDateStart = new SqlParameter() { ParameterName = "date_start", SqlValue = dateStart, SqlDbType = SqlDbType.Date };
            SqlParameter pDateEnd = new SqlParameter() { ParameterName = "date_end", SqlValue = dateEnd, SqlDbType = SqlDbType.Date };
            SqlParameter pTopRows = new SqlParameter() { ParameterName = "top_rows", SqlValue = topRows, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_claim_list", pIdAdmin, pIdEngeneer, pDateStart, pDateEnd, pTopRows);

            var lst = new List<Claim>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new Claim(row, true);
                lst.Add(model);
            }

            //Общее количество
            var dtCnt = Db.Service.ExecuteQueryStoredProcedure("get_claim_list_count", pIdAdmin, pIdEngeneer, pDateStart, pDateEnd);
            cnt = 0;
            if (dtCnt.Rows.Count > 0)
            {
                cnt = Db.DbHelper.GetValueIntOrDefault(dtCnt.Rows[0], "cnt");
            }
            return lst;
        }

        public static void Close(string sid, string deleterSid)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "sid", SqlValue = sid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pDeleterSid = new SqlParameter() { ParameterName = "deleter_sid", SqlValue = deleterSid, SqlDbType = SqlDbType.VarChar };
            var dt = Db.Service.ExecuteQueryStoredProcedure("close_claim", pId, pDeleterSid);
        }

        //public static void Close(int id, string deleterSid)
        //{
        //    SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
        //    SqlParameter pDeleterSid = new SqlParameter() { ParameterName = "deleter_sid", SqlValue = deleterSid, SqlDbType = SqlDbType.VarChar };
        //    var dt = Db.Stuff.ExecuteQueryStoredProcedure("close_claim", pId, pDeleterSid);
        //}

        public static ClaimState GetClaimCurrentState(int idClaim)
        {
            SqlParameter pIdClaim = new SqlParameter() { ParameterName = "id_claim", SqlValue = idClaim, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_claim_current_state", pIdClaim);
            var st = new ClaimState();
            if (dt.Rows.Count > 0)
            {
                st = new ClaimState(dt.Rows[0]);
                st = new ClaimState(st.Id);
            }
            else
            {
                st = ClaimState.GetFirstState();
            }

            return st;
        }

        public static ClaimState GetClaimPrevState(int idClaim)
        {
            var currSt = GetClaimCurrentState(idClaim);
            var st = ClaimState.GetPrev(currSt.Id, idClaim);
            return st;
        }


        public static IEnumerable<KeyValuePair<string, string>> GetWorkTypeSpecialistSelectionList(int idWorkType)
        {
            var list = new List<KeyValuePair<string, string>>();
            var wtSysName = new WorkType(idWorkType).SysName;

            switch (wtSysName)
            {
                case "ДНО":
                case "НПР":
                case "ТЭО":
                case "УТЗ":
                    list = AdHelper.GetUserListByAdGroup(AdGroup.ServiceTech).ToList();
                    break;
                case "РТО":
                case "МТС":
                case "УРМ":
                case "ЗРМ":
                case "МДО":
                case "ИПТ":
                case "РЗРД":
                case "ЗНЗЧ":
                    list = AdHelper.GetUserListByAdGroup(AdGroup.ServiceAdmin).ToList();
                    break;
            }

            return list;
        }

        /// <summary>
        /// Список доступных для назначения специалистов для заявки в текущем статусе
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> GetSpecialistList(int idClaim)
        {
            var list = new List<KeyValuePair<string, string>>();

            var state = GetClaimCurrentState(idClaim);
            switch (state.SysName)
            {
                case "NEWADD":
                    var wtId = new Claim(idClaim).IdWorkType;
                    if (!wtId.HasValue) throw new ArgumentException("Невозможно выбрать список специалистов. Тип работ не указан.");
                    var wtSysName = new WorkType(wtId.Value).SysName;

                    switch (wtSysName)
                    {
                        case "ДНО":
                        case "НПР":
                        case "ТЭО":
                        case "УТЗ":
                            list = AdHelper.GetUserListByAdGroup(AdGroup.ServiceTech).ToList();
                            break;
                        case "РТО":
                        case "МТС":
                        case "УРМ":
                        case "ЗРМ":
                        case "МДО":
                        case "ИПТ":
                        case "РЗРД":
                        case "ЗНЗЧ":
                            list = AdHelper.GetUserListByAdGroup(AdGroup.ServiceAdmin).ToList();
                            break;
                    }
                    break;
                case "SERVADMSETWAIT":
                    list = AdHelper.GetUserListByAdGroup(AdGroup.ServiceAdmin).ToList();
                    break;
                case "SERVENGSETWAIT":
                case "SRVADMWORK":
                    list = AdHelper.GetUserListByAdGroup(AdGroup.ServiceEngeneer).ToList();
                    break;
            }


            return list;
        }

        public static ServiceSheet GetLastServiceSheet(int idClaim)
        {
            SqlParameter pIdClaim = new SqlParameter() { ParameterName = "id_claim", SqlValue = idClaim, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_last_service_sheet_id", pIdClaim);
            ServiceSheet sheet = new ServiceSheet();
            if (dt.Rows.Count > 0)
            {
                int lastIdSerSheet = Db.DbHelper.GetValueIntOrDefault(dt.Rows[0], "id_service_sheet");
                sheet= new ServiceSheet(lastIdSerSheet);
            }
            return sheet;
        }
    }
}