﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Models.Stuff;
using DataProvider.Objects;
using DataProvider.Objects.Interfaces;
using static System.String;

namespace DataProvider.Models.Service
{
    public class Claim : DbModel
    {
        public int Id { get; set; }
        public string Sid { get; set; }
        public int IdContractor { get; set; }
        public Contractor Contractor { get; set; }
        public int IdContract { get; set; }
        public Contract Contract { get; set; }
        public int IdDevice { get; set; }
        public Device Device { get; set; }
        public string SerialNum { get; set; }
        public string ContractorName { get; set; }
        public string ContractName { get; set; }
        public string DeviceName { get; set; }
        public int IdState { get; set; }
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
        public string CurClientManagerSid { get; set; }
        public EmployeeSm Admin { get; set; }
        public EmployeeSm Engeneer { get; set; }
        public EmployeeSm Manager { get; set; }
        public EmployeeSm Tech { get; set; }
        public EmployeeSm ClientManager { get; set; }
        public EmployeeSm Changer { get; set; }
        public int? CurServiceIssueId { get; set; }
        public int? IdServiceCame { get; set; }
        public bool ContractUnknown { get; set; }
        public bool DeviceUnknown { get; set; }
        public string ClaimTypeSysName { get; set; }
        public int IdClaimType { get; set; }
        public int? IdCity { get; set; }
        public string CityName { get; set; }
        public string Address { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        /// <summary>
        /// псевдо id состав - {CityId}[|]{AddressName}
        /// </summary>
        public string AddressStrId { get; set; }
        /// <summary>
        /// Массовая заявка
        /// </summary>
        public bool DeviceCollective { get; set; }

        public Claim() { }

        public Claim(int id, bool loadObject = true, bool getNames = true)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_claim", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row, loadObject, getNames);
                //if (getNames) GetNames();
            }

            //if (!UserCanViewClaimNow(user))
            //{
            //    throw new AccessDenyException($"В настоящий момент у вас нет доступа к заявке №{id}.");
            //}
        }

        public Claim(int id, AdUser user, bool loadObject = true) : this(id, loadObject)
        {
            bool access = true;
            //bool access = false;
            //if (user.Is(AdGroup.ServiceControler))
            //{
            //    access = true;
            //}
            //else if (user.Is(AdGroup.SuperAdmin) || user.Is(AdGroup.ServiceManager))
            //{
            //    access = true;
            //}
            //else if (user.Is(AdGroup.ServiceEngeneer) && (CurEngeneerSid == user.Sid || SpecialistSid == user.Sid))
            //{
            //    access = true;
            //}
            //else if (user.Is(AdGroup.ServiceManager, AdGroup.AddNewClaim) && (CurManagerSid == user.Sid || SpecialistSid == user.Sid))
            //{
            //    access = true;
            //}
            //else if (user.Is(AdGroup.ServiceAdmin) && (CurAdminSid == user.Sid || SpecialistSid == user.Sid))
            //{
            //    access = true;
            //}
            //else if (user.Is(AdGroup.ServiceTech) && (CurTechSid == user.Sid || SpecialistSid == user.Sid))
            //{
            //    access = true;
            //}


            if (!access) throw new AccessDenyException("У вас нет доступа к заявке №{id}");

            //if (!UserCanViewClaimNow(user))
            //{
            //    throw new AccessDenyException($"В настоящий момент у вас нет доступа к заявке №{id}.");
            //}
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

        public Claim(DataRow row, bool loadObj = false, bool loadNames = true)
            : this()
        {
            FillSelf(row, loadObj, loadNames);
            //if (getNames) GetNames();
        }

        //private void GetNames()
        //{
        //    Contractor = new Contractor(Contractor.Id);
        //    Contract = new Contract(Contract.Id);
        //    Device = new Device(Device.Id);
        //}

        private void ReFillSelf(bool loadObj = true)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_claim", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row, loadObj);
            }
        }

        private void FillSelf(DataRow row, bool loadObj = true, bool loadNames = false)
        {
            Sid = Db.DbHelper.GetValueString(row, "sid");
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            IdContractor = Db.DbHelper.GetValueIntOrDefault(row, "id_contractor");
            IdContract = Db.DbHelper.GetValueIntOrDefault(row, "id_contract");
            IdDevice = Db.DbHelper.GetValueIntOrDefault(row, "id_device");
            ContractorName = Db.DbHelper.GetValueString(row, "contractor_name");
            ContractName = Db.DbHelper.GetValueString(row, "contract_name");
            DeviceName = Db.DbHelper.GetValueString(row, "device_name");
            IdWorkType = Db.DbHelper.GetValueIntOrNull(row, "id_work_type");
            SpecialistSid = Db.DbHelper.GetValueString(row, "specialist_sid");
            DateCreate = Db.DbHelper.GetValueDateTimeOrDefault(row, "date_create");
            DateStateChange = Db.DbHelper.GetValueDateTimeOrDefault(row, "date_state_change");
            ClientSdNum = Db.DbHelper.GetValueString(row, "client_sd_num");
            ChangerSid = Db.DbHelper.GetValueString(row, "changer_sid");
            CurEngeneerSid = Db.DbHelper.GetValueString(row, "cur_engeneer_sid");
            CurAdminSid = Db.DbHelper.GetValueString(row, "cur_admin_sid");
            CurTechSid = Db.DbHelper.GetValueString(row, "cur_tech_sid");
            CurManagerSid = Db.DbHelper.GetValueString(row, "cur_manager_sid");
            CurServiceIssueId = Db.DbHelper.GetValueIntOrNull(row, "cur_service_issue_id");
            IdServiceCame = Db.DbHelper.GetValueIntOrNull(row, "id_service_came");
            IdState = Db.DbHelper.GetValueIntOrDefault(row, "id_claim_state");
            ContractUnknown = Db.DbHelper.GetValueBool(row, "contract_unknown");
            DeviceUnknown = Db.DbHelper.GetValueBool(row, "device_unknown");
            Descr = Db.DbHelper.GetValueString(row, "descr");
            IdClaimType = Db.DbHelper.GetValueIntOrDefault(row, "id_claim_type");
            ClaimTypeSysName = Db.DbHelper.GetValueString(row, "claim_type_sys_name");
            IdCity = Db.DbHelper.GetValueIntOrNull(row, "id_city");
            Address = Db.DbHelper.GetValueString(row, "address");
            ContactName = Db.DbHelper.GetValueString(row, "contact_name");
            ContactPhone = Db.DbHelper.GetValueString(row, "contact_phone");
            CityName = Db.DbHelper.GetValueString(row, "city_name");
            DeviceCollective = Db.DbHelper.GetValueBool(row, "device_collective");
            CurClientManagerSid = Db.DbHelper.GetValueString(row, "cur_client_manager_sid");

            Contractor = new Contractor() { Id = Db.DbHelper.GetValueIntOrDefault(row, "id_contractor"), Name = Db.DbHelper.GetValueString(row, "contractor_name"), FullName = Db.DbHelper.GetValueString(row, "contractor_full_name") };
            if (ContractUnknown && IdContract <= 0)
            {
                Contract = new Contract()
                {
                    Id = 0,
                    Number = "неизвестно"
                };
            }
            else
            {
                Contract = new Contract()
                {
                    Id = Db.DbHelper.GetValueIntOrDefault(row, "id_contract"),
                    Number = Db.DbHelper.GetValueString(row, "contract_num")
                };
                
            }

            if (DeviceUnknown && IdDevice <= 0)
            {
                Device = new Device()
                {
                    Id = 0,
                    FullName = "неизвестно",
                    SerialNum = "неизвестно",
                    ObjectName = "неизвестно",
                    Address = "неизвестно",
                    ContactName = "неизвестно",
                    Descr = "неизвестно",
                    CityName = "неизвестно"
                };
               
            }
            else
            {
                Device = new Device()
                {
                    Id = Db.DbHelper.GetValueIntOrDefault(row, "id_device"),
                    FullName = Db.DbHelper.GetValueString(row, "device_name"),
                    SerialNum = Db.DbHelper.GetValueString(row, "device_serial_num"),
                    ObjectName = Db.DbHelper.GetValueString(row, "object_name"),
                    Address = Db.DbHelper.GetValueString(row, "device_address"),
                    ContactName = Db.DbHelper.GetValueString(row, "device_contact_name"),
                    Descr = Db.DbHelper.GetValueString(row, "c2d_comment"),
                    CityName = Db.DbHelper.GetValueString(row, "device_city_name")
                };
            }

            Manager = new EmployeeSm() { AdSid = CurManagerSid , DisplayName = Db.DbHelper.GetValueString(row, "manager_name") };
            Admin = new EmployeeSm() { AdSid = CurAdminSid, DisplayName = Db.DbHelper.GetValueString(row, "admin_name") };
            Tech = new EmployeeSm() { AdSid = CurTechSid, DisplayName = Db.DbHelper.GetValueString(row, "tech_name") };
            Engeneer = new EmployeeSm() { AdSid = CurEngeneerSid, DisplayName = Db.DbHelper.GetValueString(row, "engeneer_name") };
            Specialist = new EmployeeSm() { AdSid = SpecialistSid, DisplayName = Db.DbHelper.GetValueString(row, "specialist_name") };
            Changer = new EmployeeSm() { AdSid = ChangerSid, DisplayName = Db.DbHelper.GetValueString(row, "changer_name") };
            ClientManager = new EmployeeSm() { AdSid = CurClientManagerSid, DisplayName = Db.DbHelper.GetValueString(row, "client_manager_name") };

            if (IdWorkType.HasValue && IdWorkType.Value > 0)
                WorkType = new WorkType() {Id= IdWorkType.Value, Name = Db.DbHelper.GetValueString(row, "work_type_name"), SysName = Db.DbHelper.GetValueString(row, "work_type_sys_name"), ZipInstall = Db.DbHelper.GetValueBool(row, "work_type_zip_install"), ZipOrder = Db.DbHelper.GetValueBool(row, "work_type_zip_order") };

            State = new ClaimState() {Id=IdState, Name = Db.DbHelper.GetValueString(row, "claim_state_name"), SysName = Db.DbHelper.GetValueString(row, "claim_state_sys_name"), BackgroundColor = Db.DbHelper.GetValueString(row, "claim_state_background_color"), ForegroundColor = Db.DbHelper.GetValueString(row, "claim_state_foreground_color"), BorderColor = Db.DbHelper.GetValueString(row, "claim_state_border_color") };

            if (loadObj)
            {
                Contractor = new Contractor(Contractor.Id);
                if (!ContractUnknown)Contract = new Contract(Contract.Id);
                if (!DeviceUnknown) Device = new Device(Device.Id, Contract.Id);
                if (IdWorkType.HasValue && IdWorkType.Value > 0) WorkType = new WorkType(IdWorkType.Value);
                State = new ClaimState(Db.DbHelper.GetValueIntOrDefault(row, "id_claim_state"));
            }

            if (loadNames)
            {
                Manager = new EmployeeSm(CurManagerSid);
                Admin = new EmployeeSm(CurAdminSid);
                Tech = new EmployeeSm(CurTechSid);
                Engeneer = new EmployeeSm(CurEngeneerSid);
                Specialist = new EmployeeSm(SpecialistSid);
                Changer = new EmployeeSm(ChangerSid);
                ClientManager = new EmployeeSm(CurClientManagerSid);
            }

        }

        /// <summary>
        /// Для удаленного создания заявки на ЗИП из системы планирования (ДСУ)
        /// </summary>
        /// <param name="idServiceCame"></param>
        /// <param name="creatorSid"></param>
        /// <returns></returns>
        public static int SaveFromServicePlan4ZipClaim(AdUser user, int idServiceCame)
        {
            var came = new PlanServiceCame(idServiceCame);
            if (!came.NeedZip.HasValue || !came.NeedZip.Value) return 0;
            var claim = new Claim();
            claim.CurUserAdSid = came.CreatorSid;
            claim.IdDevice = came.IdDevice;
            claim.IdContract = came.IdContract;
            claim.IdContractor = came.IdContractor;
            claim.CurUserAdSid = came.CreatorSid;
            claim.IdServiceCame = idServiceCame;
            claim.IdWorkType = Service.WorkType.GetWorkTypeForZipClaim().Id;
            claim.CurAdminSid = came.CreatorSid;
            claim.CurEngeneerSid = came.ServiceEngeneerSid;
            //claim.Descr = came.ZipDescr;

            var sheet = new ServiceSheet();
            sheet.CounterDescr = came.Descr;
            sheet.Descr = came.ZipDescr;
            sheet.AdminSid = came.CreatorSid;
            sheet.EngeneerSid = came.ServiceEngeneerSid;
            sheet.CounterMono = came.Counter;
            sheet.CounterColor = came.CounterColor;
            sheet.ProcessEnabled = came.ProcessEnabled.HasValue ? came.ProcessEnabled.Value : false;
            sheet.DeviceEnabled = came.DeviceEnabled.HasValue ? came.DeviceEnabled.Value : false;
            sheet.ZipClaim = came.NeedZip;
            sheet.NoCounter = came.NoCounter;
            sheet.CounterUnavailable = came.CounterAvailable;
            sheet.IdServiceIssue = -999;

            claim.ServiceSheet4Save = sheet;

            var firstState = new ClaimState("SRVENGWORK");
            int id = claim.Save(firstState);
            claim.Go(user);
            return id;
        }
        /// <summary>
        /// Проверка существует ли активная заявка для данного аппарата
        /// </summary>
        /// <returns></returns>
        public bool ExistsActive()
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id_device", SqlValue = IdDevice, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("check_active_claims", pId);
            bool exists = false;
            if (dt.Rows.Count > 0)
            {
                exists = Db.DbHelper.GetValueBool(dt.Rows[0], "exists");
            }
            return exists;
        }

        /// <summary>
        /// Сохранение заявки
        /// </summary>
        /// <param name="firstState">Передается если нужно чтобы заявка сохранилась с другим первым статусом</param>
        /// <returns></returns>
        public int Save(ClaimState firstState = null)
        {
            bool isNew = Id == 0;
            //if (State == null) State = ClaimState.GetFirstState();
            //if (Admin == null) Admin = new EmployeeSm();
            //if (Engeneer == null) Engeneer = new EmployeeSm();

            if (!String.IsNullOrEmpty(ClaimTypeSysName))
            {
                IdClaimType = new ClaimType(ClaimTypeSysName).Id;

                if (ClaimTypeSysName.Equals("COLLECTIVE"))
                {
                    DeviceUnknown = true;
                }
            }

            if (DeviceUnknown || Device == null) { Device = new Device(); }
            else if (Device.Id > 0)
            {
                IdDevice = Device.Id;
            }

            //if (isNew && ExistsActive()) throw new Exception("Для данного аппарата существует незавершенная заявка. Сохранение заявки не было завершено!");

            if (ContractUnknown || Contract == null) { Contract = new Contract(); }
            else if (Contract.Id > 0)
            {
                IdContract = Contract.Id;
            }
            if (Contractor == null)
            {
                Contractor = new Contractor();
            }
            else if (Contractor.Id > 0)
            {
                IdContractor = Contractor.Id;
            }
            
            if (isNew && IdContractor > 0 && IsNullOrEmpty(ContractorName))//Загрузка названия контрагента из Эталон
            {
                Contractor = new Contractor(IdContractor);
                ContractorName = Contractor.Name;
            }
            if (isNew && String.IsNullOrEmpty(CurAdminSid))
            {
                CurAdminSid = Device.GetCurServiceAdminSid(IdDevice, IdContract);
            }

            //string wtReplace = "%work_type%";
            //if (Descr.IndexOf(wtReplace, StringComparison.Ordinal) > 0)
            //{
            //    var wt = new WorkType(IdWorkType);
            //    Descr = Descr.Replace(wtReplace, $"{wt.SysName} ({wt.Name})");
            //}
           
            if (isNew)
            {
                CurManagerSid = CurUserAdSid;
                if (IdDevice > 0) Device = new Device(IdDevice);

                
                if (!String.IsNullOrEmpty(AddressStrId))
                {
                    try
                    {
                        var arr = AddressStrId.Split(new[] { "[|]" }, StringSplitOptions.RemoveEmptyEntries);
                        IdCity = Convert.ToInt32(arr[0]);
                        if (arr.Length > 1)
                        {
                            Address = arr[1];
                        }
                    }
                    catch (Exception)
                    {
                        IdCity = null;
                        Address = null;
                    }
                }

                CurClientManagerSid = Contractor.GetCurrentClientManagerSid(IdContractor);
            }

            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            SqlParameter pIdContractor = new SqlParameter() { ParameterName = "id_contractor", SqlValue = IdContractor, SqlDbType = SqlDbType.Int };
            SqlParameter pIdContract = new SqlParameter() { ParameterName = "id_contract", SqlValue = IdContract, SqlDbType = SqlDbType.Int };
            SqlParameter pIdDevice = new SqlParameter() { ParameterName = "id_device", SqlValue = IdDevice, SqlDbType = SqlDbType.Int };
            SqlParameter pContractorName = new SqlParameter() { ParameterName = "contractor_name", SqlValue = ContractorName, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pContractName = new SqlParameter() { ParameterName = "contract_number", SqlValue = ContractName, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pDeviceName = new SqlParameter() { ParameterName = "device_name", SqlValue = DeviceName, SqlDbType = SqlDbType.NVarChar };
            //SqlParameter pIdAdmin = new SqlParameter() { ParameterName = "id_admin", SqlValue = Admin.Id, SqlDbType = SqlDbType.Int };
            //SqlParameter pIdEngeneer = new SqlParameter() { ParameterName = "id_engeneer", SqlValue = Engeneer.Id, SqlDbType = SqlDbType.Int };
            //Статус сохраняем отдельной процедурой так как надо хранить историю
            //SqlParameter pIdClaimState = new SqlParameter() { ParameterName = "id_claim_state", SqlValue = State.Id, SqlDbType = SqlDbType.Int };
            SqlParameter pIdWorkType = new SqlParameter() { ParameterName = "id_work_type", SqlValue = IdWorkType, SqlDbType = SqlDbType.Int };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pSpecialistSid = new SqlParameter() { ParameterName = "specialist_sid", SqlValue = SpecialistSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pClientSdNum = new SqlParameter() { ParameterName = "client_sd_num", SqlValue = ClientSdNum, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pEngeneerSid = new SqlParameter() { ParameterName = "cur_engeneer_sid", SqlValue = CurEngeneerSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pAdminSid = new SqlParameter() { ParameterName = "cur_admin_sid", SqlValue = CurAdminSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pTechSid = new SqlParameter() { ParameterName = "cur_tech_sid", SqlValue = CurTechSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pClientManagerSid = new SqlParameter() { ParameterName = "cur_client_manager_sid", SqlValue = CurClientManagerSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pManagerSid = new SqlParameter() { ParameterName = "cur_manager_sid", SqlValue = CurManagerSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pSerialNum = new SqlParameter() { ParameterName = "serial_num", SqlValue = Device.SerialNum, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCurServiceIssueId = new SqlParameter() { ParameterName = "cur_service_issue_id", SqlValue = CurServiceIssueId, SqlDbType = SqlDbType.Int };
            SqlParameter pIdServiceCame = new SqlParameter() { ParameterName = "id_service_came", SqlValue = IdServiceCame, SqlDbType = SqlDbType.Int };
            SqlParameter pDeviceUnknown = new SqlParameter() { ParameterName = "device_unknown", SqlValue = DeviceUnknown, SqlDbType = SqlDbType.Bit };
            SqlParameter pContractUnknown = new SqlParameter() { ParameterName = "contract_unknown", SqlValue = ContractUnknown, SqlDbType = SqlDbType.Bit };
            SqlParameter pIdCity = new SqlParameter() { ParameterName = "id_city", SqlValue = IdCity, SqlDbType = SqlDbType.Int };
            SqlParameter pAddress = new SqlParameter() { ParameterName = "address", SqlValue = Address, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pContactName = new SqlParameter() { ParameterName = "contact_name", SqlValue = ContactName, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pContactPhone = new SqlParameter() { ParameterName = "contact_phone", SqlValue = ContactPhone, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pDeviceCollective = new SqlParameter() { ParameterName = "device_collective", SqlValue = DeviceCollective, SqlDbType = SqlDbType.Bit };
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
                pContractorName, pContractName, pDeviceName, /*pIdAdmin, pIdEngeneer,*/ pCreatorAdSid, pIdWorkType, pSpecialistSid, pClientSdNum, pEngeneerSid, pAdminSid, pTechSid, pManagerSid, pSerialNum, pCurServiceIssueId, pIdServiceCame, pDeviceUnknown, pContractUnknown, pIdCity, pAddress, pContactName, pContactPhone, pDeviceCollective, pClientManagerSid);

            int id = 0;
            if (dt.Rows.Count > 0)
            {
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
            }

            if (isNew)
            {
                Id = id;
                var st = firstState ?? ClaimState.GetFirstState();
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

        public static void ChangeDeviceId(int claimId, int deviceid)
        {
            SqlParameter pIdClaim = new SqlParameter() { ParameterName = "id_claim", SqlValue = claimId, SqlDbType = SqlDbType.Int };
            SqlParameter pIdDevice = new SqlParameter() { ParameterName = "id_device", SqlValue = deviceid, SqlDbType = SqlDbType.Int };
            Db.Service.ExecuteQueryStoredProcedure("claim_change_device_id", pIdClaim, pIdDevice);
        }

        public static void ChangeContractId(int claimId, int contractId)
        {
            SqlParameter pIdClaim = new SqlParameter() { ParameterName = "id_claim", SqlValue = claimId, SqlDbType = SqlDbType.Int };
            SqlParameter pIdContract = new SqlParameter() { ParameterName = "id_contract", SqlValue = contractId, SqlDbType = SqlDbType.Int };
            Db.Service.ExecuteQueryStoredProcedure("claim_change_contract_id", pIdClaim, pIdContract);
        }

        public static void RemoteStateChange(int idClaim, string stateSysName, string creatorSid, string descr = null, int? idZipClaim = null)
        {
            var claim = new Claim(idClaim);
            claim.CurUserAdSid = creatorSid;
            if (claim.Id > 0)
            {
                var state = new ClaimState(stateSysName);
                bool saveClaimCurrentState = !stateSysName.Equals("ZIPCL-CLOSED");
                claim.SaveStateStep(state.Id, descr, idZipClaim: idZipClaim, saveClaimCurrentState: saveClaimCurrentState);
                //if (stateSysName.ToUpper().Equals("ZIPCL-FAIL"))
                //{
                //    var nextState = new ClaimState("ZIPBUYCANCEL");
                //    claim.SaveStateStep(nextState.Id);
                //}
            }
        }

        public void SaveStateStep(int stateId, string descr = null, bool saveStateInfo = true, int? idZipClaim = null, bool saveClaimCurrentState = true)
        {
            if (stateId == 0) throw new ArgumentException("Не указан статус для сохранения в лестнице статусов.");
            var c2Cs = new Claim2ClaimState();
            c2Cs.IdClaim = Id;
            c2Cs.IdClaimState = stateId;
            c2Cs.CurUserAdSid = CurUserAdSid;
            c2Cs.ZipClaimId = idZipClaim;
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
            c2Cs.Save(saveClaimCurrentState);
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

        //public enum ServiceClaimRight
        //{
        //    Save,
        //    Go,
        //    Close
        //}

        //private Claim cliam4check;

        //private void LoadClaim4Check()
        //{
        //    if (cliam4check == null || cliam4check.Id == 0)cliam4check = new Claim(Id, loadObject: false);
        //}

        private bool UserIsCurrentClaimRole(ServiceRole role, bool roleOrSpecialist = false)
        {
            if (IsNullOrEmpty(CurUserAdSid)) return false;
            bool result = false;

            var cl = new Claim(Id, loadObject: false);
            //LoadClaim4Check();

            if (role == ServiceRole.CurTech)
            {
                return (roleOrSpecialist && cl.SpecialistSid == CurUserAdSid) || cl.CurTechSid == CurUserAdSid;
            }
            else if (role == ServiceRole.CurAdmin)
            {
                return (roleOrSpecialist && cl.SpecialistSid == CurUserAdSid) || cl.CurAdminSid == CurUserAdSid;
            }
            else if (role == ServiceRole.CurManager)
            {
                return (roleOrSpecialist && cl.SpecialistSid == CurUserAdSid) || cl.CurManagerSid == CurUserAdSid;
            }
            else if (role == ServiceRole.CurEngeneer)
            {
                return (roleOrSpecialist && cl.SpecialistSid == CurUserAdSid) || cl.CurEngeneerSid == CurUserAdSid;
            }
            else if (role == ServiceRole.CurEngeneer)
            {
                return (roleOrSpecialist && cl.SpecialistSid == CurUserAdSid) || cl.SpecialistSid == CurUserAdSid;
            }
            else if (role == ServiceRole.CurSpecialist)
            {
                return cl.SpecialistSid == CurUserAdSid;
            }

            return result;
        }


        /// <summary>
        /// Перевод заявки на следующую стадию
        /// </summary>
        /// <param name="confirm">Подтвердить или отклонить назначение заявки</param>
        public void Go(AdUser user, bool confirm = true)
        {
            if (Id <= 0) throw new ArgumentException("Невозможно предать заявку. Не указан ID заявки.");

            //Save();
            string descr = Empty;
            var currState = GetClaimCurrentState(Id);
            var nextState = new ClaimState();
            bool saveStateInfo = true;
            bool sendNote = false;
            ServiceRole[] noteTo = { ServiceRole.CurSpecialist };
            string noteText = Empty;
            string noteSubject = Empty;
            bool goNext = false;
            bool saveClaim = false;//Метод вызывается из удаленных программ и поэтому не всегда нухно схранять статус
            //ReFillSelf(true);

            if (currState.SysName.ToUpper().Equals("NEW"))
            {
                if (!user.HasAccess(AdGroup.AddNewClaim, AdGroup.ServiceCenterManager)) return;
                goNext = true;
                saveClaim = true;
                int? wtId = null;
                if (!IdWorkType.HasValue)
                {
                    wtId = new Claim(Id).IdWorkType;
                    if (!wtId.HasValue)
                        throw new ArgumentException(
                            "Невозможно определить следующий статус. Тип работ заявки не указан.");
                }
                else
                {
                    wtId = IdWorkType;
                }
                var wtSysName = new WorkType(wtId.Value).SysName;
                descr =
                    $"{Descr}\r\nУстановлен тип работ {wtSysName}\r\nНазначен специалист {AdHelper.GetUserBySid(SpecialistSid).FullName}";

                switch (wtSysName)
                {
                    case "ДНО":
                    case "НПР":
                    case "ТЭО":
                    case "УТЗ":
                    case "Заказ":
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
                sendNote = true;
                noteTo = new[] { ServiceRole.CurSpecialist };
                noteText = $@"Вам назначена заявка №%ID% %LINK%";
                noteSubject = $"[Заявка №%ID%] Вам назначена заявка";

            }
            else if (currState.SysName.ToUpper().Equals("TECHSET"))
            {
                if (!user.HasAccess(AdGroup.ServiceTech)) return;
                goNext = true;
                saveClaim = true;
                if (confirm)
                {
                    nextState = new ClaimState("TECHWORK");
                    CurTechSid = CurUserAdSid;
                    SpecialistSid = CurUserAdSid;
                }
                else
                {
                    descr = $"Отклонено\r\n{Descr}";
                    nextState = new ClaimState("SERVADMSETWAIT");
                    //Очищаем выбранного специалиста так как статус заявки поменялся
                    Clear(specialist: true, tech: true);
                    //sendNote = true;
                    //noteTo = new[] { ServiceRole.CurAdmin };
                    //noteText = $@"Отклонено назначение заявки №%ID% %LINK%";
                    //noteSubject = $"[Заявка №%ID%] Отклонено назначение СТП";
                }
            }
            else if (currState.SysName.ToUpper().Equals("TECHWORK"))
            {
                if (!user.HasAccess(AdGroup.ServiceTech)) return;
                goNext = true;
                saveClaim = true;
                if (confirm)
                {
                    ServiceSheet4Save.IdClaim = Id;
                    if (ServiceSheet4Save == null || ServiceSheet4Save.IdClaim == 0)
                    {
                        throw new ArgumentException("Сервисный лист отсутствует. Операция не завершена!");
                    }
                    var cl = new Claim(Id);
                    ////if (!ServiceSheet4Save.NoTechWork)
                    ////{
                    ServiceSheet4Save.CurUserAdSid = CurUserAdSid;
                    ServiceSheet4Save.EngeneerSid = CurUserAdSid;
                    ServiceSheet4Save.IdServiceIssue = -999;
                    ServiceSheet4Save.Save("TECHWORK");
                    if (ServiceSheet4Save.ProcessEnabled && ServiceSheet4Save.DeviceEnabled)
                    {
                        nextState = new ClaimState("TECHDONE");
                        //Сначала сохраняем промежуточный статус
                        SaveStateStep(nextState.Id);
                        //saveStateInfo = false;
                        
                        nextState = new ClaimState("DONE");
                        
                        if (cl.DeviceCollective)
                        {
                            SpecialistSid = cl.CurClientManagerSid;//Contractor.GetCurrentClientManagerSid(cl.IdContractor);
                            nextState = new ClaimState("MANAGERNOTE");
                            sendNote = true;
                            noteTo = new[] { ServiceRole.CurSpecialist };
                            noteText = $@"Вам передан заказ по заявке №%ID% %LINK%";
                            noteSubject = $"[Заявка №%ID%] Вам передан заказ";
                        }

                        //Если есть хоть один неустановленный ЗИП 
                        if (GetOrderedZipItemList(Id, ServiceSheet4Save.Id).Any(x => !x.Installed))
                        {
                            nextState = new ClaimState("SERVADMSETWAIT");
                        }
                    }
                    else if ((!ServiceSheet4Save.ProcessEnabled || !ServiceSheet4Save.DeviceEnabled) &&
                             ServiceSheet4Save.ZipClaim.HasValue && ServiceSheet4Save.ZipClaim.Value)
                    {
                        //nextState = new ClaimState("TECHDONE");
                        ////Сначала сохраняем промежуточный статус
                        //SaveStateStep(nextState.Id);
                        //saveStateInfo = false;
                        //nextState = new ClaimState("ZIPORDER");
                        nextState = new ClaimState("ZIPISSUE");

                    }
                    else if ((!ServiceSheet4Save.ProcessEnabled || !ServiceSheet4Save.DeviceEnabled) &&
                             (!ServiceSheet4Save.ZipClaim.HasValue || !ServiceSheet4Save.ZipClaim.Value))
                    {

                        nextState = new ClaimState("TECHPROCESSED");
                        //Сначала сохраняем промежуточный статус
                        SaveStateStep(nextState.Id);
                        //saveStateInfo = false;
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
            }
            else if (currState.SysName.ToUpper().Equals("SERVADMSETWAIT"))
            {
                
                goNext = true;
                saveClaim = true;
                nextState = new ClaimState("SERVADMSET");
                sendNote = true;
                noteTo = new[] { ServiceRole.CurSpecialist };
                noteText = $@"Вам назначена заявка №%ID% %LINK%";
                noteSubject = $"[Заявка №%ID%] Вам назначена заявка";
            }
            else if (currState.SysName.ToUpper().Equals("SERVADMSET"))
            {
                goNext = true;
                if (confirm)
                {
                    nextState = new ClaimState("SRVADMWORK");
                    CurAdminSid = CurUserAdSid;
                    SpecialistSid = CurUserAdSid;
                }
                else
                {
                    descr = $"Отклонено\r\n{Descr}";
                    nextState = new ClaimState("SERVADMSETWAIT");
                    //Очищаем выбранного специалиста так как статус заявки поменялся
                    Clear(specialist: true, admin: true);
                    sendNote = true;
                    noteTo = new[] { ServiceRole.CurManager };
                    noteText = $@"Отклонено назначение заявки №%ID% %LINK%";
                    noteSubject = $"[Заявка №%ID%] Отклонено назначение СА";
                }
            }
            else if (currState.SysName.ToUpper().Equals("SRVADMWORK"))
            {
                if (!user.HasAccess(AdGroup.ServiceAdmin)) return;
                goNext = true;
                saveClaim = true;
                ServiceIssue4Save.IdClaim = Id;
                ServiceIssue4Save.Descr = Descr;
                ServiceIssue4Save.SpecialistSid = SpecialistSid;
                ServiceIssue4Save.CurUserAdSid = CurUserAdSid;
                int serviceIssueId = ServiceIssue4Save.Save();
                CurServiceIssueId = serviceIssueId; //Устанавливает текущий заявку на выезд
                WorkType wt = new WorkType();
                if (IdWorkType.HasValue) wt = new WorkType(IdWorkType.Value);
                descr =
                    $"Установлен тип работ {wt.Name}\r\nНазначен специалист {AdHelper.GetUserBySid(SpecialistSid).FullName}\r\nДата выезда {ServiceIssue4Save.DatePlan:dd.MM.yyyy}\r\n{Descr}";
                nextState = new ClaimState("SRVENGSET");
                CurEngeneerSid = SpecialistSid;
                sendNote = true;
                noteTo = new[] { ServiceRole.CurSpecialist };
                noteText = $@"Вам назначена заявка №%ID% %LINK%";
                noteSubject = $"[Заявка №%ID%] Вам назначена заявка";
            }
            else if (currState.SysName.ToUpper().Equals("SERVENGSETWAIT"))
            {
                goNext = true;
                saveClaim = true;
                nextState = new ClaimState("SRVENGSET");
            }
            else if (currState.SysName.ToUpper().Equals("SRVENGSET"))
            {
                if (!user.HasAccess(AdGroup.ServiceEngeneer)) return;
                goNext = true;
                saveClaim = true;

                if (confirm)
                {
                    nextState = new ClaimState("SRVENGGET");
                    //Сначала сохраняем промежуточный статус
                    //SaveStateStep(nextState.Id);
                    saveStateInfo = false;
                    //nextState = new ClaimState("SERVENGOUTWAIT");
                    CurEngeneerSid = CurUserAdSid;
                    SpecialistSid = CurUserAdSid;
                }
                else
                {
                    descr = $"Отклонено\r\n{Descr}";
                    nextState = new ClaimState("SRVENGCANCEL");
                    //Сначала сохраняем промежуточный статус
                    SaveStateStep(nextState.Id, descr);
                    saveStateInfo = false;
                    nextState = new ClaimState("SRVADMWORK");
                    Clear(specialist: true, engeneer: true);
                    sendNote = true;
                    noteTo = new[] {ServiceRole.CurAdmin};
                    noteText = $@"Отклонено назначение заявки №%ID% %LINK%";
                    noteSubject = $"[Заявка №%ID%] Отклонено назначение СИ";
                }
            }
            else if (currState.SysName.ToUpper().Equals("SRVENGGET"))
            {
                if (!user.HasAccess(AdGroup.ServiceEngeneer)) return;
                goNext = true;
                saveClaim = true;
                nextState = new ClaimState("SRVENGWENT");
            }
            else if (currState.SysName.ToUpper().Equals("SRVENGWENT"))
            {
                if (!user.HasAccess(AdGroup.ServiceEngeneer)) return;
                goNext = true;
                saveClaim = true;
                nextState = new ClaimState("SRVENGWORK");
                
                var cl = new Claim(Id);
                if (cl.IdWorkType.HasValue)
                {
                    SaveStateStep(nextState.Id, descr);
                    var wtSysName = new WorkType(cl.IdWorkType.Value).SysName;
                    if (cl.DeviceCollective && cl.WorkType.SysName.Equals("ЗРМ"))
                    {
                        nextState = new ClaimState("CARTRIDGELIST");
                        //var sheet = new ServiceSheet() { IdClaim = Id, DeviceEnabled = false, ProcessEnabled = false, ZipClaim = true, CreatorSid = CurUserAdSid, EngeneerSid = CurUserAdSid, AdminSid = cl.CurAdminSid, WorkTypeId = cl.IdWorkType ?? -1 };
                        //sheet.Save("SERVENGSETWAIT");
                        //nextState = new ClaimState("ZIPISSUE");
                    }
                }
            }
            else if (currState.SysName.ToUpper().Equals("CARTRIDGELIST"))
            {
                goNext = true;
                saveClaim = true;
                nextState = new ClaimState("CARTRIDGEREFILL");
            }
            else if (currState.SysName.ToUpper().Equals("CARTRIDGEREFILL"))
            {
                goNext = true;
                saveClaim = true;
                nextState = new ClaimState("DONE");
            }
            else if (currState.SysName.ToUpper().Equals("SRVENGWORK"))
            {
                if (!user.HasAccess(AdGroup.ServiceEngeneer)) return;
                goNext = true;
                saveClaim = true;
                ServiceSheet4Save.IdClaim = Id;
                if (ServiceSheet4Save == null || ServiceSheet4Save.IdClaim == 0)
                {
                    throw new ArgumentException("Сервисный лист отсутствует. Операция не завершена!");
                }
                var cl = new Claim(Id);
                //Если аппарат не бул указан и не указан до сих пор, то вычисляем его
                if (cl.DeviceUnknown && IdDevice <= 0)
                {
                    int? existsDeviceId;
                    if (Device.SerialNumIsExists(ServiceSheet4Save.RealSerialNum, out existsDeviceId))
                    {
                        if (!ServiceSheet4Save.ForceSaveRealSerialNum.HasValue ||
                            !ServiceSheet4Save.ForceSaveRealSerialNum.Value)
                        {
                            throw new ItemExistsException(
                                "Оборудование с таким серийным номером уже существует в списке оборудования. Сервисный лист не был сохранен!");
                        }
                        else
                        {
                            if (existsDeviceId.HasValue)
                            {
                                ChangeDeviceId(Id, existsDeviceId.Value);
                            }
                            else
                            {
                                throw new Exception(
                                    "Указанный аппарат не имеет идентификатора. Сервисный лист не был сохранен!");
                            }
                        }
                    }
                    else
                    {
                        if (ServiceSheet4Save.RealDeviceModel.HasValue)
                        {
                            int deviceId = Device.Create(ServiceSheet4Save.RealDeviceModel.Value,
                                ServiceSheet4Save.RealSerialNum,
                                CurUserAdSid);
                            if (deviceId > 0)
                            {
                                ChangeDeviceId(Id, deviceId);
                            }
                            else
                            {
                                throw new Exception(
                                    "Указанный аппарат не имеет идентификатора. Сервисный лист не был сохранен!");
                            }
                        }
                        else
                        {
                            throw new ArgumentException("Не указана модель аппарата. Сервисный лист не бул сохранен");
                        }
                    }
                }


                if (IsNullOrEmpty(ServiceSheet4Save.CurUserAdSid)) ServiceSheet4Save.CurUserAdSid = CurUserAdSid;
                if (IsNullOrEmpty(ServiceSheet4Save.EngeneerSid)) ServiceSheet4Save.EngeneerSid = CurUserAdSid;
                ServiceSheet4Save.AdminSid = cl.CurAdminSid;
                ServiceSheet4Save.IdServiceIssue = cl.CurServiceIssueId ?? -1;
                ServiceSheet4Save.Save("SRVENGWORK");

                bool hasNotInstalled = GetOrderedZipItemList(Id, ServiceSheet4Save.Id).Any(x => !x.Installed);

                if (ServiceSheet4Save.ProcessEnabled && ServiceSheet4Save.DeviceEnabled)
                {
                    //Если есть хоть один не установленный ЗИП 
                    if (hasNotInstalled)
                    {
                        nextState = new ClaimState("ZIPISSUE");
                    }
                    else
                    {
                        nextState = new ClaimState("DONE");
                    }
                }
                else if ((!ServiceSheet4Save.ProcessEnabled || !ServiceSheet4Save.DeviceEnabled) &&
                         ServiceSheet4Save.ZipClaim.HasValue &&
                         ServiceSheet4Save.ZipClaim.Value)
                {
                    nextState = new ClaimState("ZIPISSUE");
                }
                else if ((!ServiceSheet4Save.ProcessEnabled || !ServiceSheet4Save.DeviceEnabled) &&
                         (!ServiceSheet4Save.ZipClaim.HasValue || !ServiceSheet4Save.ZipClaim.Value))
                {

                    //Если есть хоть один не установленный ЗИП 
                    if (hasNotInstalled)
                    {
                        nextState = new ClaimState("ZIPISSUE");
                    }
                    else
                    {
                        nextState = new ClaimState("SERVADMSET");
                        SpecialistSid = CurAdminSid;
                        sendNote = true;
                        noteTo = new[] {ServiceRole.CurAdmin};
                        noteText =
                            $@"Инженер не восстановил работу аппарата по заявке №%ID% %LINK%.\r\Комментарий:{
                                ServiceSheet4Save.Descr}";
                        noteSubject = $"[Заявка №%ID%] Работа не восстановлена";
                    }
                }

                //Если статус завершения и номер договора неизвестен, то переход на установку договора
                if (cl.ContractUnknown && nextState.SysName.Equals("DONE"))
                {
                    nextState = new ClaimState("CONTRACTSET");
                    SpecialistSid = cl.CurClientManagerSid;//Contractor.GetCurrentClientManagerSid(cl.IdContractor);
                    sendNote = true;
                    noteTo = new[] {ServiceRole.CurSpecialist};
                    noteText = $@"Необходимо указать номер договора по заявке №%ID% %LINK%";
                    noteSubject = $"[Заявка №%ID%] Укажите номер договора";
                }
            }
            else if (currState.SysName.ToUpper().Equals("CONTRACTSET"))
            {
                if (!user.HasAccess(AdGroup.ServiceManager)) return;
                goNext = true;
                saveClaim = true;

                ChangeContractId(Id, IdContract);

                var lastServiceSheet = GetLastServiceSheet(Id);
                //var notInstalledList = GetOrderedZipItemList(Id, lastServiceSheet.Id).Where(x => !x.Installed);

                //Так как в статус изменения договора заявка может попасть только если онавыполнена либо надо заказывать ЗИП, то ограничиваем проверку сервисного листа
                if (lastServiceSheet.ProcessEnabled && lastServiceSheet.DeviceEnabled)
                {
                    nextState = new ClaimState("DONE");
                }

                if (lastServiceSheet.ZipClaim.HasValue && lastServiceSheet.ZipClaim.Value)
                {
                    nextState = new ClaimState("ZIPCHECK");

                    if (nextState.SysName.Equals("ZIPCHECK"))
                    {
                        sendNote = true;
                        noteTo = new[] {ServiceRole.AllTech};
                        noteText = $@"Необходимо заказать ЗИП по заявке №%ID% %LINK%";
                        noteSubject = $"[Заявка №%ID%] Необходимо заказать ЗИП";
                    }
                }

                var curCl = new Claim(Id);
                if (curCl.DeviceCollective)
                {
                    SpecialistSid = curCl.CurClientManagerSid;//Contractor.GetCurrentClientManagerSid(curCl.IdContractor);
                    nextState = new ClaimState("MANAGERNOTE");
                    sendNote = true;
                    noteTo = new[] {ServiceRole.CurSpecialist};
                    noteText = $@"Вам передан заказ по заявке №%ID% %LINK%";
                    noteSubject = $"[Заявка №%ID%] Вам передан заказ";
                }

                //nextState = new ClaimState("SRVENGWORK");
            }
            else if (currState.SysName.ToUpper().Equals("ZIPISSUE"))
            {
                if (!user.HasAccess(AdGroup.ServiceEngeneer, AdGroup.ServiceTech)) return;
                var lastServiceSheet = GetLastServiceSheet();
                var curCl = new Claim(Id);
                goNext = true;
                saveClaim = true;
                var notInstalledList = GetOrderedZipItemList(Id, lastServiceSheet.Id).Where(x => !x.Installed);

                //Если есть заказаный ЗИП то значит заявку надо продолжить
                if (lastServiceSheet.ZipClaim.HasValue && lastServiceSheet.ZipClaim.Value)
                {
                    if (!lastServiceSheet.GetIssuedZipItemList().Any())
                    {
                        throw new Exception("Необходимо заполнить список ЗИП. Сервисный лист не был передан.");
                    }
                    else
                    {
                        ServiceSheetZipItem.SaveOrderedZipItemsCopyFromIssued(lastServiceSheet.Id, CurUserAdSid);

                        if (notInstalledList.Any())
                            //Если есть не установленный ЗИП то сохраняем его
                        {
                            ServiceSheetZipItem.NotInstalledSaveList(notInstalledList.Select(x => x.Id).ToArray(),
                                lastServiceSheet.Id, CurUserAdSid);
                        }

                        SpecialistSid = CurUserAdSid;
                        nextState = new ClaimState("ZIPCHECK");

                        if (nextState.SysName.Equals("ZIPCHECK"))
                        {
                            sendNote = true;
                            noteTo = new[] {ServiceRole.AllTech};
                            noteText = $@"Необходимо заказать ЗИП по заявке №%ID% %LINK%";
                            noteSubject = $"[Заявка №%ID%] Необходимо заказать ЗИП";
                        }

                        if (curCl.DeviceCollective)
                        {
                            SpecialistSid = curCl.CurClientManagerSid;//Contractor.GetCurrentClientManagerSid(curCl.IdContractor);
                            nextState = new ClaimState("MANAGERNOTE");
                            sendNote = true;
                            noteTo = new[] {ServiceRole.CurSpecialist};
                            noteText = $@"Вам передан заказ по заявке №%ID% %LINK%";
                            noteSubject = $"[Заявка №%ID%] Вам передан заказ";
                        }
                    }
                }
                else
                {
                    if (notInstalledList.Any())
                        //Если есть не установленный ЗИП
                    {
                        ServiceSheetZipItem.NotInstalledSaveList(notInstalledList.Select(x => x.Id).ToArray(),
                            lastServiceSheet.Id, CurUserAdSid);
                        Descr = $"Не весь ЗИП установлен";
                        if (lastServiceSheet.ProcessEnabled && lastServiceSheet.DeviceEnabled)
                        {
                            nextState = new ClaimState("DONE");
                        }
                        else
                        {
                            nextState = new ClaimState("SERVADMSET");
                        }

                        SpecialistSid = curCl.CurAdminSid;
                        sendNote = true;
                        noteTo = new[] {ServiceRole.CurAdmin};
                        noteText = $@"Не весь ЗИП установлен №%ID% %LINK%";
                        noteSubject = $"[Заявка №%ID%] Не весь ЗИП установлен";
                    }
                    else
                    {
                        nextState = new ClaimState("DONE");
                        sendNote = true;
                        noteTo = new[] {ServiceRole.CurAdmin};
                        noteText = $@"ЗИП Установлен по заявке №%ID% %LINK%";
                        noteSubject = $"[Заявка №%ID%] ЗИП установлен";
                    }
                }


                //Если номер договора неизвестен, то переход на установку договора
                if (curCl.ContractUnknown)
                {
                    nextState = new ClaimState("CONTRACTSET");
                    SpecialistSid = curCl.CurClientManagerSid;//Contractor.GetCurrentClientManagerSid(curCl.IdContractor);
                    sendNote = true;
                    noteTo = new[] {ServiceRole.CurSpecialist};
                    noteText = $@"Необходимо указать номер договора по заявке №%ID% %LINK%";
                    noteSubject = $"[Заявка №%ID%] Укажите номер договора";
                }
            }
            else if (currState.SysName.ToUpper().Equals("ZIPCHECK"))
            {
                if (!user.HasAccess(AdGroup.ServiceTech)) return;

                //В настоящий момент по этому статусу происходит заказ ЗИП специалистом Тех поддержки
                if (!GetClaimCurrentState(Id).SysName.Equals("ZIPCHECKINWORK")) //На всякий случай проверяем еще раз
                {
                    goNext = true;
                    saveClaim = true;
                    CurTechSid = CurUserAdSid;
                    SpecialistSid = CurUserAdSid;
                    nextState = new ClaimState("ZIPCHECKINWORK");
                }
                else
                {
                    throw new ArgumentException("Проверка ЗИП уже в работе.");
                }
            }

            else if (currState.SysName.ToUpper().Equals("ZIPCHECKINWORK"))
            {
                if (!user.HasAccess(AdGroup.ServiceTech)) return;
                var curCl = new Claim(Id);
                if (curCl.SpecialistSid != CurUserAdSid && curCl.CurTechSid != CurUserAdSid)
                    throw new ArgumentException("Проверка ЗИП уже в работе.");
                goNext = true;
                saveClaim = true;

                //Если красная линия или Гарантийный аппарат то на утверждение
                if (curCl.Contract.ContractZipTypeSysName == "LESSZIP" ||
                    (curCl.Device.HasGuarantee.HasValue && curCl.Device.HasGuarantee.Value))
                {
                    nextState = new ClaimState("ZIPCONFIRM");

                }
                else
                {
                    ZipClaim.CreateClaimUnitWork(Id, CurUserAdSid);
                    nextState = new ClaimState("ZIPORDERED");

                    sendNote = true;
                    noteTo = new[] {ServiceRole.ZipConfirm};
                    noteText = $@"Необходимо утвердить список ЗИП в заявке №%ID% %LINK%";
                    noteSubject = $"[Заявка №%ID%] Утверждение список ЗИП";
                }
            }
            else if (currState.SysName.ToUpper().Equals("ZIPCONFIRM"))
            {
                if (!user.HasAccess(AdGroup.ServiceZipClaimConfirm)) return;
                goNext = true;
                saveClaim = true;
                if (confirm)
                {
                    ZipClaim.CreateClaimUnitWork(Id, CurUserAdSid);
                    nextState = new ClaimState("ZIPORDERED");
                }
                else
                {
                    descr = $"Отклонено\r\n{Descr}";
                    nextState = new ClaimState("ZIPCONFIRMCANCEL");
                    //Сначала сохраняем промежуточный статус
                    SaveStateStep(nextState.Id, descr);
                    saveStateInfo = false;
                    nextState = new ClaimState("ZIPCHECKINWORK");
                    sendNote = true;
                    noteTo = new[] {ServiceRole.CurTech};
                    noteText = $@"Отклонен список ЗИП в заявке №%ID% %LINK%";
                    noteSubject = $"[Заявка №%ID%] Отклонен список ЗИП";
                }
            }
            else if (currState.SysName.ToUpper().Equals("ZIPORDERED"))
            {
                goNext = true;
                saveClaim = true;
                if (!confirm)
                {
                    descr = $"Отказ в закупке ЗИП\r\n{Descr}";
                    nextState = new ClaimState("ZIPBUYCANCEL");
                    sendNote = true;
                    noteTo = new[] {ServiceRole.CurAdmin};
                    noteText = $@"Отказ в закупке ЗИП №%ID% %LINK%";
                    noteSubject = $"[Заявка №%ID%] Отказ в закупке ЗИП";
                }
                else
                {
                    nextState = new ClaimState("SERVADMSET");
                    var curCl = new Claim(Id, false);
                    SpecialistSid = curCl.CurAdminSid;
                    sendNote = true;
                    noteTo = new[] {ServiceRole.CurAdmin};
                    noteText = $@"Вам назначена заявка №%ID% %LINK%";
                    noteSubject = $"[Заявка №%ID%] Вам назначена заявка";
                }
            }
            else if (currState.SysName.ToUpper().Equals("DONE"))
            {
                //Если есть незакрытые СЛ то заявку не закрываем
                if (!GetClaimServiceSheetList(Id, false).Any())
                {
                    goNext = true;
                    saveClaim = true;
                    nextState = new ClaimState("END");
                    sendNote = true;
                    noteTo = new[] {ServiceRole.CurManager};
                    noteText = $@"Заявка №%ID% закрыта  %LINK%";
                    noteSubject = $"[Заявка №%ID%] Заявка закрыта";
                }
            }
            else if (currState.SysName.ToUpper().Equals("ZIPCL-CANCELED"))
            {
                goNext = true;
                saveClaim = true;
                descr = Descr;
                nextState = new ClaimState("ZIPBUYCANCEL");
            }
            else if (currState.SysName.ToUpper().Equals("ZIPCL-ETPREP-GET") ||
                     currState.SysName.ToUpper().Equals("ZIPCL-ETSHIP-GET"))
            {
                goNext = true;
                saveClaim = true;
                nextState = new ClaimState("SERVADMSET");
                var curCl = new Claim(Id, false);
                SpecialistSid = curCl.CurAdminSid;
                sendNote = true;
                noteTo = new[] {ServiceRole.CurAdmin};
                noteText = $@"Вам назначена заявка №%ID% %LINK%";
                noteSubject = $"[Заявка №%ID%] Вам назначена заявка";
            }
            else if (currState.SysName.ToUpper().Equals("ZIPCL-DELIV"))
            {
                goNext = true;
                saveClaim = true;
                nextState = new ClaimState("SERVADMSET");
                var curCl = new Claim(Id, false);
                SpecialistSid = curCl.CurAdminSid;
                sendNote = true;
                noteTo = new[] {ServiceRole.CurAdmin};
                noteText = $@"Вам назначена заявка №%ID% %LINK%";
                noteSubject = $"[Заявка №%ID%] Вам назначена заявка";
            }
            //else if (currState.SysName.ToUpper().Equals("CONTRACTSET"))
            //{
            //    goNext = true;
            //    saveClaim = true;
            //    nextState = new ClaimState("SERVADMSET");
            //    SpecialistSid = CurAdminSid;
            //    sendNote = true;
            //    noteTo = new[] { ServiceRole.CurAdmin };
            //    noteText = $@"Вам назначена заявка №%ID% %LINK%";
            //    noteSubject = $"[Заявка №%ID%] Вам назначена заявка";
            //}
            else
            {
                nextState = currState;
            }

            if (saveClaim) Save();
            if (goNext) SaveStateStep(nextState.Id, descr, saveStateInfo);
            //SaveStateStep(nextState.Id);
            if (sendNote)
            {
                noteText = $"{noteText} %CLAIMINFO%";
                SendNote(noteSubject, noteText, null, noteTo);

                ////Замена по маске
                //noteSubject = noteSubject.Replace("%ID%", Id.ToString());

                //noteText = noteText.Replace("%ID%", Id.ToString());
                //string link = $"{ConfigurationManager.AppSettings["ServiceUrl"]}/Claim/Index/{Id}";
                //noteText = noteText.Replace("%LINK%", $@"<p><a href=""{link}"">{link}</a></p>");

                //SendMailTo(noteText, noteSubject, noteTo);
            }
        }

        public static void ServiceSheetIsPayWraped(AdUser user, int serviceSheetId, string creatorSid)
        {
            var ss = new ServiceSheet(serviceSheetId);
            if (ss.IsPayed.HasValue)
            {
                string text;
                string subject;

                if (ss.IsPayed.Value)
                {
                    text = $"Принят сервисный лист №{serviceSheetId} по заявке №%ID% %LINK%";
                    subject = $"[Заявка №%ID%] Принят сервисный лист №{serviceSheetId}";
                    
                }
                else
                {
                    text = $"Не принят сервисный лист №{serviceSheetId} по заявке №%ID%<br />Причина: {ss.NotPayedComment} %LINK%";
                    subject = $"[Заявка №%ID%] Не принят сервисный лист №{serviceSheetId}";
                }
                var cl = new Claim(ss.IdClaim);
                cl.CurUserAdSid = creatorSid;
                cl.SendNote(subject, text, serviceSheetId, ServiceRole.ServiceSheetEngeneer);

                if (cl.State.SysName.Equals("DONE"))
                {
                    cl.Go(user); 
                }
            }
        }
        
        public void SendNote(string subject, string text, int? serviceSheetId = null, params ServiceRole[] to)
        {
            
            //Замена по маске
            subject = subject.Replace("%ID%", Id.ToString());

            text = text.Replace("%ID%", Id.ToString());
            string link = $"{ConfigurationManager.AppSettings["ServiceUrl"]}/Claim/Index/{Id}";
            text = text.Replace("%LINK%", $@"<p><a href=""{link}"">{link}</a></p>");

            if (text.Contains("%CLAIMINFO%"))
            {
                var cl = new Claim(Id);
                text = text.Replace("%CLAIMINFO%", $@"<p>Клиент: {cl.Contractor.FullName}<br />Аппарат: {cl.Device.FullName}<br />Адрес: {cl.CityName} {cl.Address}<br />Контактное лицо: {cl.ContactName}<br />Телефон: {cl.ContactPhone}<br />Комментарий: {cl.Descr}</p>");
            }

            SendMailTo(text, subject, serviceSheetId, to);
        }
        
        public enum ServiceRole
        {
            CurEngeneer,
            CurAdmin,
            CurTech,
            CurManager,
            CurSpecialist,
            AllTech,
            ZipConfirm,
            ServiceSheetEngeneer
        }

        public void SendMailTo(string message, string subject, int? serviceSheetId = null, params ServiceRole[] mailTo)
        {
            var cl = new Claim(Id, loadObject: false);

            foreach (ServiceRole mt in mailTo)
            {
                List<MailAddress> recipients = new List<MailAddress>();

                if (mt == ServiceRole.CurAdmin)
                {
                    string sid = cl.CurAdminSid;
                    string email = Employee.GetEmailBySid(sid);
                    if (!String.IsNullOrEmpty(email))
                    {
                        recipients.Add(new MailAddress(email));
                    }
                    else
                    {
                        message =
                            $"<p style='color: red; font-size: 14pt'>Сообщение для Сервисного Администратора не может быть доставлено, поэтому в рассылку включен контролер процесса.</p>{message}";
                        recipients.AddRange(AdHelper.GetRecipientsFromAdGroup(AdGroup.ServiceControler));
                    }
                }
                else if (mt == ServiceRole.CurEngeneer)
                {
                    string sid = cl.CurEngeneerSid;
                    string email = Employee.GetEmailBySid(sid);
                    if (!String.IsNullOrEmpty(email))
                    {
                        recipients.Add(new MailAddress(email));
                    }
                    else
                    {
                        message =
                            $"Сообщение для Сервисного Инженера не может быть доставлено, поэтому в рассылку включен контролер процесса.\r\n{message}";
                        recipients.AddRange(AdHelper.GetRecipientsFromAdGroup(AdGroup.ServiceControler));
                    }
                }
                else if (mt == ServiceRole.CurManager)
                {
                    string sid = cl.CurManagerSid;
                    string email = Employee.GetEmailBySid(sid);
                    if (!String.IsNullOrEmpty(email))
                    {
                        recipients.Add(new MailAddress(email));
                    }
                    else
                    {
                        message =
                               $"Сообщение для Менеджера сервисного центра не может быть доставлено, поэтому в рассылку включен контролер процесса.\r\n{message}";
                        recipients.AddRange(AdHelper.GetRecipientsFromAdGroup(AdGroup.ServiceControler));
                    }

                }
                else if (mt == ServiceRole.CurTech)
                {
                    string sid = cl.CurTechSid;
                    string email = Employee.GetEmailBySid(sid);
                    if (!String.IsNullOrEmpty(email))
                    {
                        recipients.Add(new MailAddress(email));
                    }
                    else
                    {
                        message =
                            $"Сообщение для Специалиста Технической Поддержки не может быть доставлено, поэтому в рассылку включен контролер процесса.\r\n{message}";
                        recipients.AddRange(AdHelper.GetRecipientsFromAdGroup(AdGroup.ServiceControler));
                    }
                }
                //else if (mt == ServiceRole.CurTech)
                //{
                //    string sid = cl.CurTechSid;
                //    email = new[] { Employee.GetEmailBySid(sid)};
                //}
                else if (mt == ServiceRole.CurSpecialist)
                {
                    string sid = cl.SpecialistSid;
                    string email = Employee.GetEmailBySid(sid);
                    if (!String.IsNullOrEmpty(email))
                    {
                        recipients.Add(new MailAddress(email));
                    }
                    else
                    {
                        message =
                            $"Сообщение для текущего Специалиста заявки не может быть доставлено, поэтому в рассылку включен контролер процесса.\r\n{message}";
                        recipients.AddRange(AdHelper.GetRecipientsFromAdGroup(AdGroup.ServiceControler));
                    }
                }
                else if (mt == ServiceRole.AllTech)
                {
                    //var emailList = new List<string>();
                    foreach (var item in AdHelper.GetUserListByAdGroup(AdGroup.ServiceTech))
                    {
                        string email = Employee.GetEmailBySid(item.Key);
                        if (!String.IsNullOrEmpty(email))
                        {
                            recipients.Add(new MailAddress(email));
                        }
                        else
                        {
                            message =
                            $"Сообщение для Специалиста Технической Поддержки не может быть доставлено поэтому в рассылку включен контролер процесса.\r\n{message}";
                            recipients.AddRange(AdHelper.GetRecipientsFromAdGroup(AdGroup.ServiceControler));
                        }
                    }
                    //email = emailList.ToArray();
                }
                else if (mt == ServiceRole.ZipConfirm)
                {
                    foreach (var item in AdHelper.GetUserListByAdGroup(AdGroup.ServiceZipClaimConfirm))
                    {
                        string email = Employee.GetEmailBySid(item.Key);
                        if (!String.IsNullOrEmpty(email))
                        {
                            recipients.Add(new MailAddress(email));
                        }
                        else
                        {
                            message =
                            $"Сообщение для Утврерждающего список ЗИП не может быть доставлено поэтому в рассылку включен контролер процесса.\r\n{message}";
                            recipients.AddRange(AdHelper.GetRecipientsFromAdGroup(AdGroup.ServiceControler));
                        }
                    }
                }
                else if (mt == ServiceRole.ServiceSheetEngeneer)
                {
                    string sid = null;
                    if (serviceSheetId.HasValue)
                    {
                        sid = new ServiceSheet(serviceSheetId.Value).EngeneerSid;
                    }
                    string email = Employee.GetEmailBySid(sid);
                    if (!String.IsNullOrEmpty(email))
                    {
                        recipients.Add(new MailAddress(email));
                    }
                    else
                    {
                        message =
                            $"Сообщение для Инженера сервисного листа №{serviceSheetId} не может быть доставлено, поэтому в рассылку включен контролер процесса.\r\n{message}";
                        recipients.AddRange(AdHelper.GetRecipientsFromAdGroup(AdGroup.ServiceControler));
                    }
                }
                else
                {
                    throw new ArgumentException("Указанный получатель не обрабатывается");
                }


                if (recipients.Any()) MessageHelper.SendMailSmtp(subject, message, true, recipients.ToArray());
            }
        }

        public static ListResult<Claim> GetList(AdUser user, string adminSid = null, string engeneerSid = null, DateTime? dateStart = null, DateTime? dateEnd = null, int? topRows = null, string managerSid = null, string techSid = null, string serialNum = null, int? idDevice = null, bool? activeClaimsOnly = false, int? idClaimState = null, int? clientId = null, string clientSdNum = null, int? claimId = null, string deviceName = null, int? pageNum = null, string groupStates = null, string address = null, string servManagerSid = null)
        {
            if (user.Is(AdGroup.ServiceAdmin)) { adminSid = user.Sid; }
            if (user.Is(AdGroup.ServiceEngeneer)) engeneerSid = user.Sid;
            if (user.Is(AdGroup.ServiceCenterManager)) servManagerSid = user.Sid;
            if (user.Is(AdGroup.ServiceManager)) managerSid = user.Sid;
            if (user.Is(AdGroup.ServiceTech)) techSid = user.Sid;
            

            if (!topRows.HasValue) topRows = 30;
            if (!pageNum.HasValue)pageNum = 1;

            SqlParameter pServAdminSid = new SqlParameter() { ParameterName = "admin_sid", SqlValue = adminSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pServEngeneerSid = new SqlParameter() { ParameterName = "engeneer_sid", SqlValue = engeneerSid, SqlDbType = SqlDbType.VarChar };
            //МСЦ - менеджер сервисного центра
            SqlParameter pServManagerSid = new SqlParameter() { ParameterName = "serv_manager_sid", SqlValue = servManagerSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pDateStart = new SqlParameter() { ParameterName = "date_start", SqlValue = dateStart, SqlDbType = SqlDbType.Date };
            SqlParameter pDateEnd = new SqlParameter() { ParameterName = "date_end", SqlValue = dateEnd, SqlDbType = SqlDbType.Date };
            SqlParameter pTopRows = new SqlParameter() { ParameterName = "top_rows", SqlValue = topRows, SqlDbType = SqlDbType.Int };
            SqlParameter pManagerSid = new SqlParameter() { ParameterName = "manager_sid", SqlValue = managerSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pTechSid = new SqlParameter() { ParameterName = "tech_sid", SqlValue = techSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pSerialNum = new SqlParameter() { ParameterName = "serial_num", SqlValue = serialNum, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pIdDevice = new SqlParameter() { ParameterName = "id_device", SqlValue = idDevice, SqlDbType = SqlDbType.Int };
            SqlParameter pActiveClaimsOnly = new SqlParameter() { ParameterName = "active_claims_only", SqlValue = activeClaimsOnly, SqlDbType = SqlDbType.Bit };
            SqlParameter pIdClaimState = new SqlParameter() { ParameterName = "id_claim_state", SqlValue = idClaimState, SqlDbType = SqlDbType.Int };
            SqlParameter pClientId = new SqlParameter() { ParameterName = "id_client", SqlValue = clientId, SqlDbType = SqlDbType.Int };
            SqlParameter pClientSdNum = new SqlParameter() { ParameterName = "client_sd_num", SqlValue = clientSdNum, SqlDbType = SqlDbType.Int };
            SqlParameter pclaimId = new SqlParameter() { ParameterName = "claim_id", SqlValue = claimId, SqlDbType = SqlDbType.Int };
            SqlParameter pDeviceName = new SqlParameter() { ParameterName = "device_name", SqlValue = deviceName, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pPageNum = new SqlParameter() { ParameterName = "page_num", SqlValue = pageNum, SqlDbType = SqlDbType.Int };
            SqlParameter pGroupStates = new SqlParameter() { ParameterName = "group_state_list", SqlValue = groupStates, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pAddress = new SqlParameter() { ParameterName = "address", SqlValue = address, SqlDbType = SqlDbType.NVarChar };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_claim_list", pServAdminSid, pServEngeneerSid, pDateStart, pDateEnd, pTopRows, pManagerSid, pTechSid, pSerialNum, pIdDevice, pActiveClaimsOnly, pIdClaimState, pClientId, pClientSdNum, pclaimId, pDeviceName, pPageNum, pGroupStates, pAddress, pServManagerSid);

            int cnt = 0;
            var lst = new List<Claim>();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var model = new Claim(row, false, false);
                    lst.Add(model);
                }
                cnt = Db.DbHelper.GetValueIntOrDefault(dt.Rows[0], "total_count");
            }

            // Общее количество
            // var dtCnt = Db.Service.ExecuteQueryStoredProcedure("get_claim_list_count", pServAdminSid, pServEngeneerSid, pDateStart, pDateEnd, pManagerSid, pTechSid, pSerialNum, pIdDevice, pActiveClaimsOnly, pIdClaimState, pClientId);
            //int cnt = 0;
            // if (dtCnt.Rows.Count > 0)
            // {
            //     cnt = Db.DbHelper.GetValueIntOrDefault(dtCnt.Rows[0], "cnt");
            // }

            var result = new ListResult<Claim>(lst, cnt);
            return result;
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
                case "Заказ":
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
        public static IEnumerable<Claim2ClaimState> GetStateHistory(int id, int topRows)
        {
            return Claim2ClaimState.GetList(id, topRows);
        }
        /// <summary>
        /// Получение последнего статуса
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sysName">Интересующий статус его системное имя</param>
        /// <returns></returns>
        public static Claim2ClaimState GetLastState(int id, string sysName)
        {
            return Claim2ClaimState.GetLastState(id, sysName);
        }

        public ServiceSheet GetLastServiceSheet()
        {
            return GetLastServiceSheet(Id);
        }

        public static ServiceSheet GetLastServiceSheet(int idClaim)
        {
            ServiceSheet.GetList(idClaim);

            SqlParameter pIdClaim = new SqlParameter() { ParameterName = "id_claim", SqlValue = idClaim, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_last_service_sheet_id", pIdClaim);
            ServiceSheet sheet = new ServiceSheet();
            if (dt.Rows.Count > 0)
            {
                int lastIdSerSheet = Db.DbHelper.GetValueIntOrDefault(dt.Rows[0], "id_service_sheet");
                sheet = new ServiceSheet(lastIdSerSheet);
            }
            return sheet;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idClaim"></param>
        /// <param name="payedWrap">Была ли обработана (отметка оплачено или нет)</param>
        /// <returns></returns>
        public static IEnumerable<ServiceSheet> GetClaimServiceSheetList(int idClaim, bool? payedWrap = null)
        {
            //return ServiceSheet.GetClaimServiceSheetList(idClaim);

            SqlParameter pIdClaim = new SqlParameter() { ParameterName = "id_claim", SqlValue = idClaim, SqlDbType = SqlDbType.Int };
            SqlParameter pIsPayed = new SqlParameter() { ParameterName = "payed_wrap", SqlValue = payedWrap, SqlDbType = SqlDbType.Bit };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_claim_service_sheet_list", pIdClaim, pIsPayed);

            var list = new List<ServiceSheet>();

            foreach (DataRow row in dt.Rows)
            {
                int idServiceSheet;
                int.TryParse(row["id"].ToString(), out idServiceSheet);
                list.Add(new ServiceSheet(idServiceSheet));
                //list.Add(new ServiceSheet(row, true));
            }
            return list;
            //return (from DataRow row in dt.Rows select new ServiceSheet(row)).ToList();
        }

        public static IEnumerable<ZipClaim> GetClaimZipClaimList(int idClaim)
        {
            SqlParameter pIdClaim = new SqlParameter() { ParameterName = "id_claim", SqlValue = idClaim, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_claim_zip_claim_list", pIdClaim);

            var list = new List<ZipClaim>();

            foreach (DataRow row in dt.Rows)
            {
                //int idServiceSheet;
                //int.TryParse(row["id_service_sheet"].ToString(), out idServiceSheet);
                list.Add(new ZipClaim(row));
            }
            return list;

            //return (from DataRow row in dt.Rows select new ZipClaim(row)).ToList();
        }

        public static IEnumerable<ServiceSheetZipItem> GetOrderedZipItemList(int claimId, int? serviceSheetId = null)
        {
            SqlParameter pClaimId = new SqlParameter() { ParameterName = "claim_id", SqlValue = claimId, SqlDbType = SqlDbType.Int };
            SqlParameter pServiceSheetId = new SqlParameter() { ParameterName = "id_service_sheet", SqlValue = serviceSheetId, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("claim_ordered_zip_item_list_get", pClaimId, pServiceSheetId);

            var lst = new List<ServiceSheetZipItem>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new ServiceSheetZipItem(row);
                lst.Add(model);
            }

            return lst;
        }
    }
}