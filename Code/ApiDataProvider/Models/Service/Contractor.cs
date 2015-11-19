using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider._TMPLTS;

namespace DataProvider.Models.Service
{
    public class Contractor
    {
        public string Sid { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Inn { get; set; }
        public int ClaimCount { get; set; }

        public Contractor()
        {
        }

        public Contractor(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.UnitProg.ExecuteQueryStoredProcedure("get_contractor_name", pId);
            if (dt.Rows.Count > 0)
            {
                FillSelf(dt.Rows[0]);
            }
        }

        public Contractor(DataRow row)
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Sid = Db.DbHelper.GetValueString(row, "sid");
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            Name = Db.DbHelper.GetValueString(row, "name");
            FullName = Db.DbHelper.GetValueString(row, "full_name");
            Inn = Db.DbHelper.GetValueString(row, "inn");
            ClaimCount = Db.DbHelper.GetValueIntOrDefault(row, "cnt");

            if (!String.IsNullOrEmpty(Inn)) Name = String.Format("{0} (ИНН {1})", Name, Inn);
        }

        public static IEnumerable<Contractor> GetServicePlanList(int? idContractor = null, string contractorName = null, int? idContract = null, string contractNumber = null, int? idDevice=null, string deviceName = null)
        {
            //if (idContractor.HasValue) contractorName = null;
            //if (idContract.HasValue) contractNumber = null;
            if (idDevice.HasValue)
            {
                idContractor = null;
                contractorName = null;
                idContract = null;
                contractNumber = null;
            }

            SqlParameter pId = new SqlParameter() { ParameterName = "id_contractor", SqlValue = idContractor, SqlDbType = SqlDbType.Int };
            SqlParameter pName = new SqlParameter() { ParameterName = "contractor_name", SqlValue = contractorName, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pIdContract = new SqlParameter() { ParameterName = "id_contract", SqlValue = idContract, SqlDbType = SqlDbType.Int };
            SqlParameter pContractNumber = new SqlParameter() { ParameterName = "contract_number", SqlValue = contractNumber, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pIdDevice = new SqlParameter() { ParameterName = "id_device", SqlValue = idDevice, SqlDbType = SqlDbType.Int };
            SqlParameter pDeviceName = new SqlParameter() { ParameterName = "device_name", SqlValue = deviceName, SqlDbType = SqlDbType.NVarChar };
            var dt = Db.UnitProg.ExecuteQueryStoredProcedure("get_contractor_list", pId, pName, pIdContract, pContractNumber, pIdDevice, pDeviceName);

            var lst = new List<Contractor>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new Contractor(row);
                lst.Add(model);
            }

            return lst;
        }

        public static IEnumerable<Contractor> GetServiceClaimFilterList()
        {
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_claim_client_list");

            var lst = new List<Contractor>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new Contractor(row);
                lst.Add(model);
            }

            return lst;
        }

        public static string GetCurrentClientManagerSid(int clientId)
        {
            SqlParameter pClientId = new SqlParameter() { ParameterName = "client_id", SqlValue = clientId, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("client_get_client_manager", pClientId);
            string sid = String.Empty;
            if (dt.Rows.Count > 0)
            {
                sid = Db.DbHelper.GetValueString(dt.Rows[0], "sid");
            }
            return sid;
        }
    }
}