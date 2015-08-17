using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

        //public IEnumerable<Claim2ClaimState> StateHistory { get; set; }

        public string Descr { get; set; }

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
        }

        public int Save(SetNextState setNextState = SetNextState.None)
        {
            //if (State == null) State = ClaimState.GetFirstState();
            if (Admin == null) Admin = new EmployeeSm();
            if (Engeneer == null) Engeneer = new EmployeeSm();
            if (Contract == null) Contract = new Contract();
            if (Contractor == null) Contractor = new Contractor();
            if (Device == null) Device = new Device();

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
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };
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
                pContractorName, pContractName, pDeviceName, pIdAdmin, pIdEngeneer, pCreatorAdSid);

            int id = 0;
            if (dt.Rows.Count > 0)
            {
                Int32.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;
            }

            if (Id > 0)
            {
                var state = GetClaimCurrentState(Id);
                var c2Cs = new Claim2ClaimState();
                //c2Cs.State = state;
                c2Cs.IdClaim = Id;
                c2Cs.IdClaimState = state.Id;
                c2Cs.Descr = Descr;
                c2Cs.CurUserAdSid = CurUserAdSid;
                c2Cs.Save();

                //SqlParameter pIdClaim = new SqlParameter() { ParameterName = "id_claim", SqlValue = Id, SqlDbType = SqlDbType.Int };
                //SqlParameter pIdClaimState = new SqlParameter() { ParameterName = "id_claim_state", SqlValue = state.Id, SqlDbType = SqlDbType.Int };
                //Db.Service.ExecuteQueryStoredProcedure("set_claim_current_state", conn, tran, pIdClaim, pIdClaimState);


                if (setNextState == SetNextState.Next)
                {
                    var nextState = ClaimState.GetNext(state.Id);
                    c2Cs = new Claim2ClaimState();
                    c2Cs.IdClaim = Id;
                    c2Cs.IdClaimState = nextState.Id;
                    c2Cs.Descr = String.Empty;
                    c2Cs.CurUserAdSid = CurUserAdSid;
                    c2Cs.Save();
                }
                else if (setNextState == SetNextState.End)
                {
                    var nextState = ClaimState.GetEndState();
                    c2Cs = new Claim2ClaimState();
                    c2Cs.IdClaim = Id;
                    c2Cs.IdClaimState = nextState.Id;
                    c2Cs.Descr = String.Empty;
                    c2Cs.CurUserAdSid = CurUserAdSid;
                    c2Cs.Save();
                }
            }
            else
            {
                throw new ArgumentException("Сохранение заявки неуспешно");
            }

            //tran.Commit();




            //}
            //catch (Exception ex)
            //{
            //    tran.Rollback();
            //    throw;
            //}
            //}
            //}

            return Id;
        }

        public static IEnumerable<Claim> GetList(int? idAdmin = null, int? idEngeneer = null, DateTime? dateStart = null, DateTime? dateEnd = null, int? topRows = null)
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
            }
            else
            {
                st = ClaimState.GetFirstState();
            }

            return st;
        }

        public static ClaimState GetClaimNextState(int idClaim)
        {
            var currSt = GetClaimCurrentState(idClaim);
            var st = ClaimState.GetNext(currSt.Id);
            return st;
        }
    }
}