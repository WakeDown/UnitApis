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
    public class PlanServiceIssue:DbModel
    {
        public int Id { get; set; }
        public int IdServiceClaim { get; set; }
        public int IdContract { get; set; }
        public int IdDevice { get; set; }
        public string DeviceName { get; set; }
        public string ContractNumber { get; set; }
        public string DeviceModel { get; set; }
        public DateTime PlaningDate { get; set; }
        public string Descr { get; set; }
        public int IdCity { get; set; }
        public string CityName { get; set; }
        public string CityShortName { get; set; }
        public string Address { get; set; }
        public string ObjectName { get; set; }
        public int IdClient { get; set; }
        public string ClientName { get; set; }
        public string DeviceSerialNum { get; set; }
        public string IdWorkType { get; set; }
        public string CounterMono { get; set; }
        public string CounterColor { get; set; }
        public string SpecialistSid { get; set; }
        public DateTime DateCreate { get; set; }

        public PlanServiceIssue() { }

        public PlanServiceIssue(int id)
        {
            //SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            //var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_model", pId);
            //if (dt.Rows.Count > 0)
            //{
            //    var row = dt.Rows[0];
            //    FillSelf(row);
            //}
        }

        public PlanServiceIssue(DataRow row)
            : this()
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            IdServiceClaim = Db.DbHelper.GetValueIntOrDefault(row, "id_service_claim");
            IdContract = Db.DbHelper.GetValueIntOrDefault(row, "id_contract");
            IdDevice = Db.DbHelper.GetValueIntOrDefault(row, "id_device");
            DeviceModel = Db.DbHelper.GetValueString(row, "model");
            PlaningDate = Db.DbHelper.GetValueDateTimeOrDefault(row, "planing_date");
            Descr = Db.DbHelper.GetValueString(row, "descr");
            IdCity = Db.DbHelper.GetValueIntOrDefault(row, "id_city");
            CityName = Db.DbHelper.GetValueString(row, "city");
            CityShortName = Db.DbHelper.GetValueString(row, "city_short");
            Address = Db.DbHelper.GetValueStringOrEmpty(row, "address");
            IdClient = Db.DbHelper.GetValueIntOrDefault(row, "id_contractor");
            ClientName = Db.DbHelper.GetValueString(row, "contractor");
            ContractNumber = Db.DbHelper.GetValueString(row, "contract_number");
            DeviceName = Db.DbHelper.GetValueString(row, "device");
            ObjectName = Db.DbHelper.GetValueString(row, "object_name");
        }

        public int MobileSave()
        {
            SqlParameter pIdDevice = new SqlParameter() { ParameterName = "id_device", SqlValue = IdDevice, SqlDbType = SqlDbType.Int };
            SqlParameter pDeviceSerialNum = new SqlParameter() { ParameterName = "device_serial_num", SqlValue = DeviceSerialNum, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pDeviceModel = new SqlParameter() { ParameterName = "device_model", SqlValue = DeviceModel, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCityName = new SqlParameter() { ParameterName = "city", SqlValue = CityName, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pIAddress = new SqlParameter() { ParameterName = "address", SqlValue = Address, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pClientName = new SqlParameter() { ParameterName = "client_name", SqlValue = ClientName, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pIdWorkType = new SqlParameter() { ParameterName = "id_work_type", SqlValue = IdWorkType, SqlDbType = SqlDbType.Int };
            SqlParameter pCounterMono = new SqlParameter() { ParameterName = "counter_mono", SqlValue = CounterMono, SqlDbType = SqlDbType.BigInt };
            SqlParameter pCounterColor = new SqlParameter() { ParameterName = "counter_color", SqlValue = CounterColor, SqlDbType = SqlDbType.BigInt };
            SqlParameter pDescr = new SqlParameter() { ParameterName = "descr", SqlValue = Descr, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pSpecialistSid = new SqlParameter() { ParameterName = "specialist_sid", SqlValue = SpecialistSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pDateCreate = new SqlParameter() { ParameterName = "date_create", SqlValue = DateCreate, SqlDbType = SqlDbType.DateTime };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };

            var dt = Db.Service.ExecuteQueryStoredProcedure("save_mobile_came", pIdDevice, pDeviceSerialNum, pDeviceModel, pCityName, pIAddress, pClientName, pIdWorkType, pCounterMono, pCounterColor, pDescr, pSpecialistSid, pDateCreate, pCreatorAdSid);
            int id = 0;
            if (dt.Rows.Count > 0)
            {
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
            }
            return id;
        }

        public static IEnumerable<PlanServiceIssue> GetClaimList(DateTime month, int? idCity = null, string address=null, int? idClient = null)
        {
            SqlParameter pMonth = new SqlParameter() { ParameterName = "date_month", SqlValue = month, SqlDbType = SqlDbType.Date };
            SqlParameter pIdCity = new SqlParameter() { ParameterName = "id_city", SqlValue = idCity, SqlDbType = SqlDbType.Int };
            SqlParameter pAddress = new SqlParameter() { ParameterName = "address", SqlValue = address, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pIdClient = new SqlParameter() { ParameterName = "id_contractor", SqlValue = idClient, SqlDbType = SqlDbType.Int };
            var dt = Db.UnitProg.ExecuteQueryStoredProcedure("get_service_claim_list", pMonth, pIdCity, pAddress, pIdClient);

            var lst = new List<PlanServiceIssue>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new PlanServiceIssue(row);
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