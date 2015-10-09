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
    public class Device:DbModel
    {
        public string Sid { get; set; }
        public int Id { get; set; }
        public string SerialNum { get; set; }
        public string ModelName { get; set; }
        //public string FullName { get; set; }
        public string Vendor { get; set; }
        public string Address { get; set; }
        public string ObjectName { get; set; }
        public string ContactName { get; set; }
        public string Descr { get; set; }
        public string FullName { get; set; }
        public string ExtendedName { get; set; }
        public int ClassifierCategoryId { get; set; }


        public Device()
        {
        }

        public Device(int id, int? idContract = null)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            SqlParameter pIdContract = new SqlParameter() { ParameterName = "id_contract", SqlValue = idContract, SqlDbType = SqlDbType.Int };
            var dt = Db.UnitProg.ExecuteQueryStoredProcedure("get_device", pId, pIdContract);
            if (dt.Rows.Count > 0)
            {
                FillSelf(dt.Rows[0]);
            }
        }

        public static DeviceInfoResult GetInfo(string serialNum)
        {
            SqlParameter pSerialNum = new SqlParameter() { ParameterName = "serial_num", SqlValue = serialNum, SqlDbType = SqlDbType.NVarChar };
            var dt = Db.UnitProg.ExecuteQueryStoredProcedure("get_device_info", pSerialNum);
            var result= new DeviceInfoResult();
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                result.DeviceId = Db.DbHelper.GetValueIntOrNull(row, "id_device");
                result.DeviceSerialNum = Db.DbHelper.GetValueString(row, "serial_num");
                result.ContractorStr = Db.DbHelper.GetValueString(row, "contractor_name");
                result.ContractStr = Db.DbHelper.GetValueString(row, "contract_number");
                result.AddressStr = Db.DbHelper.GetValueString(row, "device_address");
                result.DeviceStr = Db.DbHelper.GetValueString(row, "device_name");
                result.DescrStr = Db.DbHelper.GetValueString(row, "descr");
            }
            return result;
        }

        public static IEnumerable<DeviceInfoResult> GetInfoList(DateTime? lastModifyDate = null)
        {
            SqlParameter pLastModifyDate = new SqlParameter() { ParameterName = "last_modify_date", SqlValue = lastModifyDate, SqlDbType = SqlDbType.Date };
            var dt = Db.UnitProg.ExecuteQueryStoredProcedure("get_device_info", pLastModifyDate);
            var list = new List<DeviceInfoResult>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var info = new DeviceInfoResult();
                    info.DeviceId = Db.DbHelper.GetValueIntOrNull(row, "id_device");
                    info.DeviceSerialNum = Db.DbHelper.GetValueString(row, "serial_num");
                    info.ContractorStr = Db.DbHelper.GetValueString(row, "contractor_name");
                    info.ContractStr = Db.DbHelper.GetValueString(row, "contract_number");
                    info.AddressStr = Db.DbHelper.GetValueString(row, "device_address");
                    info.DeviceStr = Db.DbHelper.GetValueString(row, "device_name");
                    info.DescrStr = Db.DbHelper.GetValueString(row, "descr");

                    list.Add(info);
                }
                
            }
            return list;
        }
       
        public Device(DataRow row)
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Sid = Db.DbHelper.GetValueString(row, "sid");
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            ModelName = Db.DbHelper.GetValueString(row, "model_name");
            SerialNum = Db.DbHelper.GetValueString(row, "serial_num");
            Vendor = Db.DbHelper.GetValueString(row, "vendor");
            Address = Db.DbHelper.GetValueString(row, "address");
            ObjectName = Db.DbHelper.GetValueString(row, "object_name");
            ContactName = Db.DbHelper.GetValueString(row, "contact_name");
            Descr = Db.DbHelper.GetValueString(row, "comment");
            FullName = $"{Vendor} {ModelName} №{SerialNum}";
            ExtendedName = $"{FullName} {Address} {ObjectName}";
            ClassifierCategoryId = Db.DbHelper.GetValueIntOrDefault(row, "id_classifier_category");

        }

        public static string GetCurServiceAdminSid(int idDevice, int idContract)
        {
            string sid = String.Empty;
            SqlParameter pIdDevice = new SqlParameter() { ParameterName = "id_device", SqlValue = idDevice, SqlDbType = SqlDbType.Int };
            SqlParameter pIdContract = new SqlParameter() { ParameterName = "id_contract", SqlValue = idContract, SqlDbType = SqlDbType.Int };
            var dt = Db.UnitProg.ExecuteQueryStoredProcedure("device_get_current_service_admin", pIdDevice, pIdContract);
            if (dt.Rows.Count > 0)
            {
                sid = dt.Rows[0]["service_admin_sid"].ToString();
            }
            return sid;
        }

        public static IEnumerable<Device> GetList(int? idContractor = null, string contractorName = null, int? idContract = null, string contractNumber = null, int? idDevice = null, string deviceName = null, string serialNum = null)
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
            SqlParameter pSerialNum = new SqlParameter() { ParameterName = "serial_num", SqlValue = serialNum, SqlDbType = SqlDbType.NVarChar };
            var dt = Db.UnitProg.ExecuteQueryStoredProcedure("get_device_list", pId, pName, pIdContract, pContractNumber, pIdDevice, pDeviceName, pSerialNum);

            var lst = new List<Device>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new Device(row);
                lst.Add(model);
            }

            return lst;
        }
    }
}