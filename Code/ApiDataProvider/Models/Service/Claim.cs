using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Channels;
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
        public string ContractorName { get; set; }
        public string ContractName { get; set; }
        public string DeviceName { get; set; }
        //public EmployeeSm Admin { get; set; }
        //public EmployeeSm Engeneer { get; set; }
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
        public int? CurServiceIssueId { get; set; }
        public int? IdServiceCame { get; set; }

        public Claim() { }

        public Claim(int id, bool getNames = false, bool loadObject = true)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_claim", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row, loadObject);
                if (getNames) GetNames();
            }

            //if (!UserCanViewClaimNow(user))
            //{
            //    throw new AccessDenyException($"В настоящий момент у вас нет доступа к заявке №{id}.");
            //}
        }

        public Claim(int id, AdUser user, bool getNames = false, bool loadObject = true) : this(id, getNames, loadObject)
        {

            bool access = false;
            if (user.Is(AdGroup.ServiceControler))
            {
                access = true;
            }
            else if (user.Is(AdGroup.SuperAdmin) || user.Is(AdGroup.ServiceManager))
            {
                access = true;
            }
            else if (user.Is(AdGroup.ServiceEngeneer) && (CurEngeneerSid == user.Sid || SpecialistSid == user.Sid))
            {
                access = true;
            }
            else if (user.Is(AdGroup.ServiceManager) && (CurManagerSid == user.Sid || SpecialistSid == user.Sid))
            {
                access = true;
            }
            else if (user.Is(AdGroup.ServiceAdmin) && (CurAdminSid == user.Sid || SpecialistSid == user.Sid))
            {
                access = true;
            }
            else if (user.Is(AdGroup.ServiceTech) && (CurTechSid == user.Sid || SpecialistSid == user.Sid))
            {
                access = true;
            }
            

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

        private void FillSelf(DataRow row, bool loadObj = true)
        {
            Sid = Db.DbHelper.GetValueString(row, "sid");
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            IdContractor = Db.DbHelper.GetValueIntOrDefault(row, "id_contractor");
            IdContract = Db.DbHelper.GetValueIntOrDefault(row, "id_contract");
            IdDevice = Db.DbHelper.GetValueIntOrDefault(row, "id_device");
            ContractorName = Db.DbHelper.GetValueString(row, "contractor_name");
            ContractName = Db.DbHelper.GetValueString(row, "contract_name");
            DeviceName = Db.DbHelper.GetValueString(row, "device_name");
            //Admin = new EmployeeSm(Db.DbHelper.GetValueIntOrDefault(row, "id_admin"));
            //Engeneer = new EmployeeSm(Db.DbHelper.GetValueIntOrDefault(row, "id_engeneer"));

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

            if (loadObj)
            {
                if (IdWorkType.HasValue && IdWorkType.Value > 0) WorkType = new WorkType(IdWorkType.Value);
                Specialist = new EmployeeSm(SpecialistSid);
                State = new ClaimState(Db.DbHelper.GetValueIntOrDefault(row, "id_claim_state"));
                Contractor = new Contractor() { Id = Db.DbHelper.GetValueIntOrDefault(row, "id_contractor") };
                Contract = new Contract() { Id = Db.DbHelper.GetValueIntOrDefault(row, "id_contract") };
                Device = new Device() { Id = Db.DbHelper.GetValueIntOrDefault(row, "id_device") };
            }
        }

        /// <summary>
        /// Для удаленного создания заявки на ЗИП из системы планирования (ДСУ)
        /// </summary>
        /// <param name="idServiceCame"></param>
        /// <param name="creatorSid"></param>
        /// <returns></returns>
        public static int SaveFromServicePlan4ZipClaim(int idServiceCame)
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
            claim.Descr = came.Descr;

            var sheet = new ServiceSheet();
            sheet.Descr= came.Descr;
            sheet.AdminSid= came.CreatorSid;
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
            claim.Go();
            return id;
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
            if (Contract == null) { Contract = new Contract();}
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
            if (Device == null) { Device = new Device();}else if (Device.Id > 0)
            {
                IdDevice = Device.Id;
            }
            if (isNew && IdContractor>0 && IsNullOrEmpty(ContractorName))//Загрузка названия контрагента из Эталон
            {
                Contractor = new Contractor(IdContractor);
                ContractorName = Contractor.Name;
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
            SqlParameter pManagerSid = new SqlParameter() { ParameterName = "cur_manager_sid", SqlValue = CurManagerSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pSerialNum = new SqlParameter() { ParameterName = "serial_num", SqlValue = Device.SerialNum, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCurServiceIssueId = new SqlParameter() { ParameterName = "cur_service_issue_id", SqlValue = CurServiceIssueId, SqlDbType = SqlDbType.Int };
            SqlParameter pIdServiceCame = new SqlParameter() { ParameterName = "id_service_came", SqlValue = IdServiceCame, SqlDbType = SqlDbType.Int };
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
                pContractorName, pContractName, pDeviceName, /*pIdAdmin, pIdEngeneer,*/ pCreatorAdSid, pIdWorkType, pSpecialistSid, pClientSdNum, pEngeneerSid, pAdminSid, pTechSid, pManagerSid, pSerialNum, pCurServiceIssueId, pIdServiceCame);

            int id = 0;
            if (dt.Rows.Count > 0)
            {
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
            }

            if (isNew)
            {
                Id = id;
                var st = firstState??ClaimState.GetFirstState();
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

        public static void RemoteStateChange(int idClaim, string stateSysName, string creatorSid, string descr = null, int? idZipClaim = null)
        {
            var claim = new Claim(idClaim);
            claim.CurUserAdSid = creatorSid;
            if (claim.Id > 0)
            {
                var state = new ClaimState(stateSysName);
                claim.SaveStateStep(state.Id, descr);
                //if (stateSysName.ToUpper().Equals("ZIPCL-FAIL"))
                //{
                //    var nextState = new ClaimState("ZIPBUYCANCEL");
                //    claim.SaveStateStep(nextState.Id);
                //}
            }
        }

        public void SaveStateStep(int stateId, string descr = null, bool saveStateInfo = true, int? idZipClaim = null)
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
            }else if (role == ServiceRole.CurEngeneer)
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
        public void Go(bool confirm = true)
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

            switch (currState.SysName.ToUpper())//Текущий статус
            {
                //case "NEW":
                //    nextState = new ClaimState("NEWADD");
                //    break;
                case "NEW":
                    goNext = true;
                    saveClaim = true;
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
                    sendNote = true;
                    noteTo = new[] { ServiceRole.CurSpecialist };
                    noteText = $@"Вам назначена заявка №%ID% %LINK%";
                    noteSubject = $"Назначена заявка №%ID%";
                    
                    break;
                case "TECHSET":
                    goNext = true;
                    saveClaim = true;
                    if (confirm)
                    {
                        nextState = new ClaimState("TECHWORK");
                    }
                    else
                    {
                        descr = $"Отклонено\r\n{Descr}";
                        nextState = new ClaimState("NEW");
                        //Очищаем выбранного специалиста так как статус заявки поменялся
                        Clear(specialist: true, workType: true, tech: true);
                        sendNote = true;
                        noteTo = new[] { ServiceRole.CurAdmin };
                        noteText = $@"Отклонено назначение заявки №%ID% %LINK%";
                        noteSubject = $"Отклонено назначение заявки №%ID%";
                    }
                    break;
                case "TECHWORK":
                    goNext = true;
                    saveClaim = true;
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
                        ServiceSheet4Save.EngeneerSid = CurUserAdSid;
                        ServiceSheet4Save.IdServiceIssue = -999;
                        ServiceSheet4Save.Save("TECHWORK");
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
                            nextState = new ClaimState("ZIPORDER");

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
                    goNext = true;
                    saveClaim = true;
                    nextState = new ClaimState("SERVADMSET");
                    sendNote = true;
                    noteTo = new[] { ServiceRole.CurSpecialist };
                    noteText = $@"Вам назначена заявка №%ID% %LINK%";
                    noteSubject = $"Назначена заявка №%ID%";
                    break;
                case "SERVADMSET":
                    goNext = true;
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
                        Clear(specialist: true, admin: true);
                        sendNote = true;
                        noteTo = new[] { ServiceRole.CurManager };
                        noteText = $@"Отклонено назначение заявки №%ID% %LINK%";
                        noteSubject = $"Отклонено назначение заявки №%ID%";
                    }
                    break;
                case "SRVADMWORK":
                    goNext = true;
                    saveClaim = true;
                    ServiceIssue4Save.IdClaim = Id;
                    ServiceIssue4Save.Descr = Descr;
                    ServiceIssue4Save.SpecialistSid = SpecialistSid;
                    ServiceIssue4Save.CurUserAdSid = CurUserAdSid;
                    int serviceIssueId = ServiceIssue4Save.Save();
                    CurServiceIssueId = serviceIssueId;//Устанавливает текущий заявку на выезд
                    descr = $"Назначен специалист {AdHelper.GetUserBySid(SpecialistSid).FullName}\r\nДата выезда {ServiceIssue4Save.DatePlan:dd.MM.yyyy}\r\n{Descr}";
                    nextState = new ClaimState("SRVENGSET");
                    CurEngeneerSid = SpecialistSid;
                    sendNote = true;
                    noteTo = new[] { ServiceRole.CurSpecialist };
                    noteText = $@"Вам назначена заявка №%ID% %LINK%";
                    noteSubject = $"Назначена заявка №%ID%";
                    break;
                case "SERVENGSETWAIT":
                    goNext = true;
                    saveClaim = true;
                    nextState = new ClaimState("SRVENGSET");
                    break;
                case "SRVENGSET":
                    goNext = true;
                    saveClaim = true;
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
                        Clear(specialist: true, engeneer: true);
                        sendNote = true;
                        noteTo = new[] { ServiceRole.CurAdmin };
                        noteText = $@"Отклонено назначение заявки №%ID% %LINK%";
                        noteSubject = $"Отклонено назначение заявки №%ID%";
                    }
                    break;
                case "SERVENGOUTWAIT":
                    goNext = true;
                    saveClaim = true;
                    nextState = new ClaimState("SRVENGWENT");
                    break;
                case "SRVENGWENT":
                    goNext = true;
                    saveClaim = true;
                    nextState = new ClaimState("SRVENGWORK");
                    break;
                case "SRVENGWORK":
                    goNext = true;
                    saveClaim = true;
                    ServiceSheet4Save.IdClaim = Id;
                    if (ServiceSheet4Save == null || ServiceSheet4Save.IdClaim == 0)
                    { throw new ArgumentException("Сервисный лист отсутствует. Операция не завершена!"); }

                    var cl = new Claim(Id);

                    if (IsNullOrEmpty(ServiceSheet4Save.CurUserAdSid)) ServiceSheet4Save.CurUserAdSid = CurUserAdSid;
                    if (IsNullOrEmpty(ServiceSheet4Save.EngeneerSid))ServiceSheet4Save.EngeneerSid = CurUserAdSid;
                    ServiceSheet4Save.IdServiceIssue = cl.CurServiceIssueId ?? -1;
                    ServiceSheet4Save.Save("SRVENGWORK");

                    if (ServiceSheet4Save.ProcessEnabled && ServiceSheet4Save.DeviceEnabled)
                    {
                        nextState = new ClaimState("DONE");

                    }
                    else if ((!ServiceSheet4Save.ProcessEnabled || !ServiceSheet4Save.DeviceEnabled) && ServiceSheet4Save.ZipClaim.HasValue &&
                             ServiceSheet4Save.ZipClaim.Value)
                    {
                        nextState = new ClaimState("ZIPORDER");
                    }
                    else if ((!ServiceSheet4Save.ProcessEnabled || !ServiceSheet4Save.DeviceEnabled) && (!ServiceSheet4Save.ZipClaim.HasValue || !ServiceSheet4Save.ZipClaim.Value))
                    {
                        nextState = new ClaimState("ZIPORDER");
                    }

                    if (nextState.SysName.Equals("ZIPORDER"))
                    {
                        sendNote = true;
                        noteTo = new[] { ServiceRole.AllTech };
                        noteText = $@"Необходимо заказать ЗИП по заявке №%ID% %LINK%";
                        noteSubject = $"Необходимо заказать ЗИП по заявке №%ID%";
                    }

                    break;
                case "ZIPORDER"://В настоящий момент по этому статусу происходит заказ ЗИП специалистом Тех поддержки
                    if (!GetClaimCurrentState(Id).SysName.Equals("ZIPCLINWORK"))//На всякий случай проверяем еще раз
                    {
                        goNext = true;
                        saveClaim = true;
                        CurTechSid = CurUserAdSid;
                        SpecialistSid = CurUserAdSid;
                        nextState = new ClaimState("ZIPCLINWORK");
                    }
                    else
                    {
                        throw new ArgumentException("Заказ уже в работе.");
                    }
                    break;
                case "ZIPCLINWORK":
                    var curCl = new Claim(Id);
                    if (curCl.SpecialistSid != CurUserAdSid && curCl.CurTechSid != CurUserAdSid) throw new ArgumentException("Заказ уже в работе.");
                    break;
                case "ZIPCHECK":
                    goNext = true;
                    saveClaim = true;
                    nextState = new ClaimState("ZIPCHECKED");
                    break;
                case "ZIPCHECKED":
                    goNext = true;
                    saveClaim = true;
                    nextState = new ClaimState("ZIPCONFIRMED");
                    break;
                case "ZIPCONFIRMED":
                    goNext = true;
                    saveClaim = true;
                    nextState = new ClaimState("ZIPORDERED");
                    break;
                case "ZIPORDERED":
                    goNext = true;
                    saveClaim = true;
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
                    goNext = true;
                    saveClaim = true;
                    nextState = new ClaimState("END");
                    sendNote = true;
                    noteTo = new[] { ServiceRole.CurManager };
                    noteText = $@"Заявка №%ID% закрыта  %LINK%";
                    noteSubject = $"Заявка №%ID% закрыта";
                    break;
                case "ZIPCL-FAIL":
                    goNext = true;
                    saveClaim = true;
                    descr = Descr;
                    nextState = new ClaimState("ZIPBUYCANCEL");
                    break;
                case "ZIPCL-ETPREP-GET":
                    goNext = true;
                    saveClaim = true;
                    nextState = new ClaimState("SERVADMSET");
                    SpecialistSid = CurAdminSid;
                    sendNote = true;
                    noteTo = new[] { ServiceRole.CurAdmin };
                    noteText = $@"Вам назначена заявка №%ID% %LINK%";
                    noteSubject = $"Назначена заявка №%ID%";
                    break;
                case "ZIPCL-DELIV":
                    goNext = true;
                    saveClaim = true;
                    nextState = new ClaimState("SERVADMSET");
                    SpecialistSid = CurAdminSid;
                    sendNote = true;
                    noteTo = new[] { ServiceRole.CurAdmin };
                    noteText = $@"Вам назначена заявка №%ID% %LINK%";
                    noteSubject = $"Назначена заявка №%ID%";
                    break;
                default:
                    nextState = currState;
                    break;
            }
            if (saveClaim) Save();
            if (goNext)SaveStateStep(nextState.Id, descr, saveStateInfo);
            //SaveStateStep(nextState.Id);
            if (sendNote)
            {
                SendNote(noteSubject, noteText, noteTo);

                ////Замена по маске
                //noteSubject = noteSubject.Replace("%ID%", Id.ToString());

                //noteText = noteText.Replace("%ID%", Id.ToString());
                //string link = $"{ConfigurationManager.AppSettings["ServiceUrl"]}/Claim/Index/{Id}";
                //noteText = noteText.Replace("%LINK%", $@"<p><a href=""{link}"">{link}</a></p>");

                //SendMailTo(noteText, noteSubject, noteTo);
            }
        }

        public void SendNote(string subject, string text, ServiceRole[] to)
        {
            //Замена по маске
            subject = subject.Replace("%ID%", Id.ToString());

            text = text.Replace("%ID%", Id.ToString());
            string link = $"{ConfigurationManager.AppSettings["ServiceUrl"]}/Claim/Index/{Id}";
            text = text.Replace("%LINK%", $@"<p><a href=""{link}"">{link}</a></p>");

            SendMailTo(text, subject, to);
        }

        public enum ServiceRole
        {
            CurEngeneer,
            CurAdmin,
            CurTech,
            CurManager,
            CurSpecialist,
            AllTech
        }

        public void SendMailTo(string message, string subject, params ServiceRole[] mailTo)
        {
            var cl = new Claim(Id, loadObject: false);

            foreach (ServiceRole mt in mailTo)
            {
                string[] email = null;

                if (mt == ServiceRole.CurAdmin)
                {
                    string sid = cl.CurAdminSid;
                    email = new[] {Employee.GetEmailBySid(sid)};
                }
                else if (mt == ServiceRole.CurEngeneer)
                {
                    string sid = cl.CurEngeneerSid;
                    email = new[] { Employee.GetEmailBySid(sid)};
                }
                else if (mt == ServiceRole.CurManager)
                {
                    string sid = cl.CurManagerSid;
                    email = new[] { Employee.GetEmailBySid(sid)};

                }
                else if (mt == ServiceRole.CurTech)
                {
                    string sid = cl.CurTechSid;
                    email = new[] { Employee.GetEmailBySid(sid)};
                }
                //else if (mt == ServiceRole.CurTech)
                //{
                //    string sid = cl.CurTechSid;
                //    email = new[] { Employee.GetEmailBySid(sid)};
                //}
                else if (mt == ServiceRole.CurSpecialist)
                {
                    string sid = cl.SpecialistSid;
                    email = new[] { Employee.GetEmailBySid(sid)};
                }
                else if (mt == ServiceRole.AllTech)
                {
                    var emailList = new List<string>();
                    foreach (var item in AdHelper.GetUserListByAdGroup(AdGroup.ServiceTech))
                    {
                        emailList.Add(Employee.GetEmailBySid(item.Key));
                    }
                    email = emailList.ToArray();
                }
                else
                {
                    throw new ArgumentException("Указанный получатель не обрабатывается");
                }
                if (email.Any()) MessageHelper.SendMailSmtp(subject, message, true, email);
            }
        }

        public static IEnumerable<Claim> GetList(AdUser user, out int cnt, string adminSid = null, string engeneerSid = null, DateTime? dateStart = null, DateTime? dateEnd = null, int? topRows = null, string managerSid = null, string techSid = null, string serialNum=null, int? idDevice = null, bool? activeClaimsOnly = false, int? idClaimState = null, int? clientId = null)
        {
            if (user.Is(AdGroup.ServiceAdmin)) adminSid = user.Sid;
            if (user.Is(AdGroup.ServiceEngeneer)) engeneerSid = user.Sid;
            if (user.Is(AdGroup.ServiceManager)) managerSid = user.Sid;
            if (user.Is(AdGroup.ServiceTech)) techSid = user.Sid;
            
            //!!!!ЕСЛИ МЕНЯЕШЬ ЭТУ ФУНКЦИЮ ПОМИ ЧТО НАДО ПОПРАВИТЬ ФУНКЦИЮ ЧУТЬ НИЖЕ 
            if (!topRows.HasValue) topRows = 30;

            SqlParameter pServAdminSid = new SqlParameter() { ParameterName = "admin_sid", SqlValue = adminSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pServEngeneerSid = new SqlParameter() { ParameterName = "engeneer_sid", SqlValue = engeneerSid, SqlDbType = SqlDbType.VarChar };
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
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_claim_list", pServAdminSid, pServEngeneerSid, pDateStart, pDateEnd, pTopRows, pManagerSid, pTechSid, pSerialNum, pIdDevice, pActiveClaimsOnly, pIdClaimState, pClientId);

            var lst = new List<Claim>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new Claim(row, true);
                lst.Add(model);
            }

            //Общее количество
            var dtCnt = Db.Service.ExecuteQueryStoredProcedure("get_claim_list_count", pServAdminSid, pServEngeneerSid, pDateStart, pDateEnd, pManagerSid, pTechSid, pSerialNum, pIdDevice, pActiveClaimsOnly, pIdClaimState, pClientId);
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
        public static IEnumerable<Claim2ClaimState> GetStateHistory(int id)
        {
            return Claim2ClaimState.GetList(id);
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

        public static ServiceSheet GetLastServiceSheet(int idClaim)
        {
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
    }
}