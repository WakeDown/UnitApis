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
    public class Address:DbModel
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
        public string AddressName { get; set; }

        public Address()
        {
        }

        public Address(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.UnitProg.ExecuteQueryStoredProcedure("get_contract", pId);
            if (dt.Rows.Count > 0)
            {
                FillSelf(dt.Rows[0]);
            }
        }

        public Address(DataRow row)
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            CityId = Db.DbHelper.GetValueIntOrDefault(row, "id_city");
            CityName = Db.DbHelper.GetValueString(row, "city");
            AddressName = Db.DbHelper.GetValueString(row, "address");
        }

        public static IEnumerable<Address> GetList(int? idContractor = null, int? idContract = null, int? idDevice = null, string addrName = null)
        {

            SqlParameter pIdContractor = new SqlParameter() { ParameterName = "id_contractor", SqlValue = idContractor, SqlDbType = SqlDbType.Int };
            SqlParameter pIdContract = new SqlParameter() { ParameterName = "id_contract", SqlValue = idContract, SqlDbType = SqlDbType.Int };
            SqlParameter pIdDevice = new SqlParameter() { ParameterName = "id_device", SqlValue = idDevice, SqlDbType = SqlDbType.Int };
            SqlParameter pAddrName = new SqlParameter() { ParameterName = "address", SqlValue = addrName, SqlDbType = SqlDbType.NVarChar };
            var dt = Db.UnitProg.ExecuteQueryStoredProcedure("get_address_list", pIdContractor, pIdContract, pIdDevice, pAddrName);

            var lst = new List<Address>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new Address(row);
                lst.Add(model);
            }

            return lst;
        }

        public static IEnumerable<KeyValuePair<string, string>> GetSelectionList(int? idContractor = null, int? idContract = null, int? idDevice = null, string addrName = null)
        {
            SqlParameter pIdContractor = new SqlParameter() { ParameterName = "id_contractor", SqlValue = idContractor, SqlDbType = SqlDbType.Int };
            SqlParameter pIdContract = new SqlParameter() { ParameterName = "id_contract", SqlValue = idContract, SqlDbType = SqlDbType.Int };
            SqlParameter pIdDevice = new SqlParameter() { ParameterName = "id_device", SqlValue = idDevice, SqlDbType = SqlDbType.Int };
            SqlParameter pAddrName = new SqlParameter() { ParameterName = "address", SqlValue = addrName, SqlDbType = SqlDbType.NVarChar };
            var dt = Db.UnitProg.ExecuteQueryStoredProcedure("get_address_list", pIdContractor, pIdContract, pIdDevice, pAddrName);

            var lst = new List<KeyValuePair<string, string>>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new KeyValuePair<string, string>(Db.DbHelper.GetValueString(row, "name"), Db.DbHelper.GetValueString(row, "name"));
                lst.Add(model);
            }

            return lst;
        }
    }
}