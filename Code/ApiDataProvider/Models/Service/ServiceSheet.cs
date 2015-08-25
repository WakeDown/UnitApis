using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Objects;
using DataProvider._TMPLTS;

namespace DataProvider.Models.Service
{
    public class ServiceSheet:DbModel
    {
        public int Id { get; set; }
        public int IdClaim { get; set; }
        public int IdClaim2ClaimState { get; set; }
        public bool ProcessEnabled { get; set; }
        public bool DeviceEnabled { get; set; }
        public bool ZipClaim { get; set; }
        public string ZipClaimNumber { get; set; }
        public int CounterMono { get; set; }
        public int CounterColor { get; set; }
        public bool NoTechWork { get; set; }


        public ServiceSheet() { }

        public ServiceSheet(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_service_sheet", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
        }

        public ServiceSheet(DataRow row)
            : this()
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            IdClaim = Db.DbHelper.GetValueIntOrDefault(row, "id_claim");
            IdClaim2ClaimState = Db.DbHelper.GetValueIntOrDefault(row, "id_claim2claim_state");
            ProcessEnabled = Db.DbHelper.GetValueBool(row, "process_enabled");
            DeviceEnabled = Db.DbHelper.GetValueBool(row, "device_enabled");
            ZipClaim = Db.DbHelper.GetValueBool(row, "zip_claim");
            ZipClaimNumber = Db.DbHelper.GetValueString(row, "zip_claim_number");
            CounterMono = Db.DbHelper.GetValueIntOrDefault(row, "counter_mono");
            CounterColor = Db.DbHelper.GetValueIntOrDefault(row, "counter_color");
        }

        public void Save()
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            SqlParameter pIdClaim2ClaimState = new SqlParameter() { ParameterName = "id_claim2claim_state", SqlValue = IdClaim2ClaimState, SqlDbType = SqlDbType.Int };
            SqlParameter pProcessEnabled = new SqlParameter() { ParameterName = "process_enabled", SqlValue = ProcessEnabled, SqlDbType = SqlDbType.Bit };
            SqlParameter pDeviceEnabled = new SqlParameter() { ParameterName = "device_enabled", SqlValue = DeviceEnabled, SqlDbType = SqlDbType.Bit };
            SqlParameter pZipClaim = new SqlParameter() { ParameterName = "zip_claim", SqlValue = ZipClaim, SqlDbType = SqlDbType.Int };
            SqlParameter pZipClaimNumber = new SqlParameter() { ParameterName = "zip_claim_number", SqlValue = ZipClaimNumber, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCounterMono = new SqlParameter() { ParameterName = "counter_mono", SqlValue = CounterMono, SqlDbType = SqlDbType.Int };
            SqlParameter pCounterColor = new SqlParameter() { ParameterName = "counter_color", SqlValue = CounterColor, SqlDbType = SqlDbType.Int };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };

            var dt = Db.Service.ExecuteQueryStoredProcedure("save_service_sheet", pId, pIdClaim2ClaimState, pProcessEnabled, pDeviceEnabled, pZipClaim, pZipClaimNumber, pCounterMono, pCounterColor, pCreatorAdSid);
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

        //public static void Close(int id, string deleterSid)
        //{
        //    SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
        //    SqlParameter pDeleterSid = new SqlParameter() { ParameterName = "deleter_sid", SqlValue = deleterSid, SqlDbType = SqlDbType.VarChar };
        //    var dt = Db.Stuff.ExecuteQueryStoredProcedure("close_model", pId, pDeleterSid);
        //}
    }
}