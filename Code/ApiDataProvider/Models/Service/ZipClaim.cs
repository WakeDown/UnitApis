using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Objects;

namespace DataProvider.Models.Service
{
    public class ZipClaim:DbModel
    {
        public int Id { get; set; }

        public int? IdDevice { get; set; }
        public string SerialNum { get; set; }
        public string DeviceModel { get; set; }
        public string Contractor { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public int? Counter { get; set; }
        public int IdEngeneerConclusion { get; set; }
        public int? IdClaimState { get; set; }
        public string RequestNum { get; set; }
        public string Descr { get; set; }
        public int? IdManager { get; set; }
        public int? IdOperator { get; set; }
        public int? IdServiceEngeneer { get; set; }
        public int? IdServiceAdmin { get; set; }
        public int? IdContractor { get; set; }
        public int? IdCity { get; set; }
        public int IdCreator { get; set; }
        public bool DisplayPriceState { get; set; }
        public bool DisplayDoneState { get; set; }
        public bool DisplaySendState { get; set; }
        public bool DisplayZipConfirmState { get; set; }
        public bool DisplayPriceSet { get; set; }
        public bool DisplayCancelState { get; set; }
        public string ServiceDeskNum { get; set; }
        public DateTime DateCreate { get; set; }
        public int? CounterColour { get; set; }
        public string CancelComment { get; set; }
        public string ObjectName { get; set; }
        //public StateHistory[] ChangeStateHistory { get; set; }
        public string WaybillNum { get; set; }
        public string ContractNum { get; set; }
        public string ContractType { get; set; }
        public string ContractorSdNum { get; set; }

        //Данные для клиентов
        public string ServiceEngeneer { get; set; }
        public string Manager { get; set; }
        public string EngeneerConclusion { get; set; }
        public string ManagerMail { get; set; }
        public int? IdContract { get; set; }
        public bool HideTop { get; set; }
        public string ServiceIdServSheet { get; set; }
        public string ServiceIdClaim { get; set; }
        /// <summary>
        /// Вы этот объект передается список ЗИПов из сервисного листа программы Сервис-Инциденты
        /// </summary>
        public IEnumerable<ServiceSheetZipItem> ZipItemList { get; set; }

        public ZipClaim()
        {
        }

        public ZipClaim(DataRow row)
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            DateCreate = Db.DbHelper.GetValueDateTimeOrDefault(row, "date_create");
        }

        public static void CreateClaimUnitWork(int idClaim, string creatorSid)
        {
            var claim = new Claim(idClaim, true, true);
            var device = new Device(claim.IdDevice, claim.IdContract);
            var lastServiceSheet = claim.GetLastServiceSheet();
            var zipClaim = new ZipClaim();
            zipClaim.IdDevice = claim.IdDevice;
            zipClaim.SerialNum = claim.Device.SerialNum;
            zipClaim.DeviceModel = claim.Device.ModelName;
            zipClaim.Contractor = claim.ContractorName;
            zipClaim.IdCity = device.IdCity;
            zipClaim.City = device.CityName;
            zipClaim.Address = device.Address;
            zipClaim.Descr = "Заявка создана автоматически из системы Сервис";
            zipClaim.Counter = lastServiceSheet.CounterMono;
            zipClaim.CounterColour = lastServiceSheet.CounterColor;
            zipClaim.IdContractor = claim.IdContractor;
            zipClaim.IdServiceEngeneer = UserUnitProg.GetUserId(lastServiceSheet.EngeneerSid);
            zipClaim.IdEngeneerConclusion = lastServiceSheet.DeviceEnabled? 1 : 2;
            zipClaim.IdManager = claim.Contract.ManagerIdUnitProg;
            zipClaim.IdServiceAdmin = UserUnitProg.GetUserId(lastServiceSheet.AdminSid);
            zipClaim.ServiceDeskNum = claim.Id.ToString();
            zipClaim.ObjectName = device.ObjectName;
            zipClaim.ContractNum = claim.Contract.Number;
            zipClaim.ContractType = claim.Contract.TypeName;
            zipClaim.ContractorSdNum = claim.ClientSdNum;
            zipClaim.ServiceIdServSheet = lastServiceSheet.Id.ToString();
            zipClaim.ServiceIdClaim = claim.Id.ToString();

            zipClaim.CurUserAdSid = creatorSid;
            zipClaim.SaveUnitProg();

            var zipItemList = lastServiceSheet.GetOrderedZipItemList(null);

            if (zipItemList != null && zipItemList.Any())
            {
                foreach (var item in zipItemList)
                {
                    item.ClaimId = zipClaim.Id;
                    item.CurUserAdSid = creatorSid;
                    item.SaveUnitProg();
                }
            }

            zipClaim.SetSendStateUnitProg();

            lastServiceSheet.UnitProgZipClaimId = zipClaim.Id;
            lastServiceSheet.SaveUnitProgZipClaimId();

            lastServiceSheet.SetOrderedZipItemListRealyOrdered();

        }

        public void SetSendStateUnitProg()
        {
            SqlParameter pAction = new SqlParameter() { ParameterName = "action", Value = "setClaimSendState", SqlDbType = SqlDbType.NVarChar };
            SqlParameter pId = new SqlParameter() { ParameterName = "id_claim", Value = Id, DbType = DbType.Int32 };
            SqlParameter pIdCreator = new SqlParameter() { ParameterName = "id_creator", Value = UserUnitProg.GetUserId(CurUserAdSid), DbType = DbType.Int32 };

            DataTable dt = Db.UnitProg.ExecuteQueryStoredProcedure("ui_zip_claims", pAction, pId, pIdCreator);
        }

        public void SaveUnitProg()
        {
            SqlParameter pAction = new SqlParameter() { ParameterName = "action", Value = "saveClaim", SqlDbType = SqlDbType.NVarChar };
            SqlParameter pId = new SqlParameter() { ParameterName = "id_claim", Value = Id, DbType = DbType.Int32 };
            SqlParameter pIdDevice = new SqlParameter() { ParameterName = "id_device", Value = IdDevice, DbType = DbType.Int32 };
            SqlParameter pSerialNum = new SqlParameter() { ParameterName = "serial_num", Value = SerialNum, DbType = DbType.AnsiString };
            SqlParameter pDeviceModel = new SqlParameter() { ParameterName = "device_model", Value = DeviceModel, DbType = DbType.AnsiString };
            SqlParameter pContractor = new SqlParameter() { ParameterName = "contractor", Value = Contractor, DbType = DbType.AnsiString };
            SqlParameter pCity = new SqlParameter() { ParameterName = "city", Value = City, DbType = DbType.AnsiString };
            SqlParameter pAddress = new SqlParameter() { ParameterName = "address", Value = Address, DbType = DbType.AnsiString };
            SqlParameter pCounter = new SqlParameter() { ParameterName = "counter", Value = Counter, DbType = DbType.Int32 };
            SqlParameter pIdEngeneerConclusion = new SqlParameter() { ParameterName = "id_engeneer_conclusion", Value = IdEngeneerConclusion, DbType = DbType.Int32 };
            SqlParameter pIdClaimState = new SqlParameter() { ParameterName = "id_claim_state", Value = IdClaimState, DbType = DbType.Int32 };
            SqlParameter pRequestNum = new SqlParameter() { ParameterName = "request_num", Value = RequestNum, DbType = DbType.AnsiString };
            SqlParameter pDescr = new SqlParameter() { ParameterName = "descr", Value = Descr, DbType = DbType.AnsiString };
            SqlParameter pIdManager = new SqlParameter() { ParameterName = "id_manager", Value = IdManager, DbType = DbType.Int32 };
            SqlParameter pIdOperator = new SqlParameter() { ParameterName = "id_operator", Value = IdOperator, DbType = DbType.Int32 };
            SqlParameter pIdServiceEngeneer = new SqlParameter() { ParameterName = "id_engeneer", Value = IdServiceEngeneer, DbType = DbType.Int32 };
            SqlParameter pIdServiceAdmin = new SqlParameter() { ParameterName = "id_service_admin", Value = IdServiceAdmin, DbType = DbType.Int32 };
            SqlParameter pIdContractor = new SqlParameter() { ParameterName = "id_contractor", Value = IdContractor, DbType = DbType.Int32 };
            SqlParameter pIdCity = new SqlParameter() { ParameterName = "id_city", Value = IdCity, DbType = DbType.Int32 };
            SqlParameter pIdCreator = new SqlParameter() { ParameterName = "id_creator", Value = UserUnitProg.GetUserId(CurUserAdSid), DbType = DbType.Int32 };
            SqlParameter pServiceDeskNum = new SqlParameter() { ParameterName = "service_desk_num", Value = ServiceDeskNum, DbType = DbType.AnsiString };
            SqlParameter pCounterColour = new SqlParameter() { ParameterName = "counter_colour", Value = CounterColour, DbType = DbType.Int32 };
            SqlParameter pCancelComment = new SqlParameter() { ParameterName = "cancel_comment", Value = CancelComment, DbType = DbType.AnsiString };
            SqlParameter pObjectName = new SqlParameter() { ParameterName = "object_name", Value = ObjectName, DbType = DbType.AnsiString };
            SqlParameter pWaybillNum = new SqlParameter() { ParameterName = "waybill_num", Value = WaybillNum, DbType = DbType.AnsiString };
            SqlParameter pContractNum = new SqlParameter() { ParameterName = "contract_num", Value = ContractNum, DbType = DbType.AnsiString };
            SqlParameter pContractType = new SqlParameter() { ParameterName = "contract_type", Value = ContractType, DbType = DbType.AnsiString };
            SqlParameter pContractorSdNum = new SqlParameter() { ParameterName = "contractor_sd_num", Value = ContractorSdNum, DbType = DbType.AnsiString };
            SqlParameter pServiceIdServSheet = new SqlParameter() { ParameterName = "service_id_serv_sheet", Value = ServiceIdServSheet, DbType = DbType.AnsiString };
            SqlParameter pServiceIdClaim = new SqlParameter() { ParameterName = "service_id_claim", Value = ServiceIdClaim, DbType = DbType.AnsiString };

            DataTable dt = Db.UnitProg.ExecuteQueryStoredProcedure("ui_zip_claims", pAction, pId, pIdDevice, pSerialNum, pDeviceModel, pContractor, pCity, pAddress, pIdEngeneerConclusion, pCounter, pIdClaimState, pRequestNum, pDescr, pIdManager, pIdOperator, pIdServiceEngeneer, pIdServiceAdmin, pIdContractor, pIdCity, pIdCreator, pServiceDeskNum, pCounterColour, pCancelComment, pObjectName, pWaybillNum, pContractNum, pContractType, pContractorSdNum, pServiceIdServSheet, pServiceIdClaim);


            if (dt.Rows.Count > 0)
            {
                Id = (int)dt.Rows[0]["id_claim"];
            }

        }
    }
}