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
    public class Contract:DbModel
    {
        public string Sid { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public bool? ClientSdNumRequired { get; set; }
        public string ContractZipTypeSysName { get; set; }
        public string ManagerSid { get; set; }
        public int? ManagerIdUnitProg { get; set; }
        public string TypeName { get; set; }

        public Contract()
        {
        }

        public Contract(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.UnitProg.ExecuteQueryStoredProcedure("get_contract", pId);
            if (dt.Rows.Count > 0)
            {
                FillSelf(dt.Rows[0]);
            }
        }

        public Contract(DataRow row)
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Sid = Db.DbHelper.GetValueString(row, "sid");
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            Name = Db.DbHelper.GetValueString(row, "name");
            Number = Db.DbHelper.GetValueString(row, "number");
            ClientSdNumRequired = Db.DbHelper.GetValueBoolOrNull(row, "client_sd_num_required");
            ContractZipTypeSysName = Db.DbHelper.GetValueString(row, "zip_state_sys_name");
            ManagerSid = Db.DbHelper.GetValueString(row, "manager_sid");
            ManagerIdUnitProg = Db.DbHelper.GetValueIntOrNull(row, "id_manager");
            TypeName = Db.DbHelper.GetValueString(row, "contract_type_name"); 
        }

        public static IEnumerable<Contract> GetList(int? idContractor = null, string contractorName = null, int? idContract = null, string contractNumber = null, int? idDevice = null, string deviceName = null)
        {
            //if (idContractor.HasValue) contractorName = null;
            //if (idContract.HasValue) contractNumber = null;
            //if (idDevice.HasValue) deviceName = null;

            SqlParameter pId = new SqlParameter() { ParameterName = "id_contractor", SqlValue = idContractor, SqlDbType = SqlDbType.Int };
            SqlParameter pName = new SqlParameter() { ParameterName = "contractor_name", SqlValue = contractorName, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pIdContract = new SqlParameter() { ParameterName = "id_contract", SqlValue = idContract, SqlDbType = SqlDbType.Int };
            SqlParameter pContractNumber = new SqlParameter() { ParameterName = "contract_number", SqlValue = contractNumber, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pIdDevice = new SqlParameter() { ParameterName = "id_device", SqlValue = idDevice, SqlDbType = SqlDbType.Int };
            SqlParameter pDeviceName = new SqlParameter() { ParameterName = "device_name", SqlValue = deviceName, SqlDbType = SqlDbType.NVarChar };
            var dt = Db.UnitProg.ExecuteQueryStoredProcedure("get_contract_list", pId, pName, pIdContract, pContractNumber, pIdDevice, pDeviceName);

            var lst = new List<Contract>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new Contract(row);
                lst.Add(model);
            }

            return lst;
        }
    }
}