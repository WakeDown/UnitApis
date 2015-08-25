using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Objects;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DataProvider.Models.Service
{
    public class ClientAccess : DbModel
    {
        public int Id { get; set; }
        public int? IdClientEtalon { get; set; }
        public string AdSid { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public bool ZipAccess { get; set; }
        public bool CounterAccess { get; set; }


        public ClientAccess() { }

        public ClientAccess(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_client_access", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
        }

        public ClientAccess(DataRow row)
            : this()
        {
            FillSelf(row);
        }

        private const string passSalt = "UN1T12345";

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            IdClientEtalon = Db.DbHelper.GetValueIntOrDefault(row, "id_client_etalon");
            Name = Db.DbHelper.GetValueString(row, "name");
            FullName = Db.DbHelper.GetValueString(row, "full_name");
            Login = Db.DbHelper.GetValueString(row, "login");
            string passEncoded = Db.DbHelper.GetValueString(row, "password");
            if (!String.IsNullOrEmpty(passEncoded) && !passEncoded.EndsWith("-Z"))
            {
                Password = MathHelper.Decrypt(passEncoded, passSalt);
            }
            else
            {
                Password = passEncoded;
            }
            ZipAccess = Db.DbHelper.GetValueBool(row, "zip_access");
            CounterAccess = Db.DbHelper.GetValueBool(row, "counter_access");
            AdSid = Db.DbHelper.GetValueString(row, "ad_sid");
        }

        public bool CheckClientAccessIsExists(int IdClientEtalon)
        {
            bool result = false;
            SqlParameter pIdClientEtalon = new SqlParameter() { ParameterName = "id_client_etalon", SqlValue = IdClientEtalon, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("check_client_access_is_exists", pIdClientEtalon);
            if (dt.Rows.Count > 0)
            {
                result = Db.DbHelper.GetValueBool(dt.Rows[0],"result");
            }
            return result;
        }

        public void Save(bool isUpdate = false)
        {
            if (!IdClientEtalon.HasValue) throw new ArgumentException("Не указан ID клиента.");
            Login = IdClientEtalon.ToString();

            if (!isUpdate && CheckClientAccessIsExists(IdClientEtalon.Value)) throw new ArgumentException("Доступ для клиента уже существует.");

            Password = MathHelper.GenerateSimplePassword();
            string truePassword = Password;
            Password = MathHelper.Encrypt(Password, passSalt);

            SqlParameter pIdContractor = new SqlParameter() { ParameterName = "id_contractor", SqlValue = IdClientEtalon, SqlDbType = SqlDbType.Int };
            var dtCtrtr = Db.Service.ExecuteQueryStoredProcedure("get_contractor", pIdContractor);
            if (dtCtrtr.Rows.Count > 0)
            {
                Name = Db.DbHelper.GetValueString(dtCtrtr.Rows[0], "name");
                FullName = Db.DbHelper.GetValueString(dtCtrtr.Rows[0], "full_name");
            }

            string adPath = "OU=Zip-client,OU=Users External,DC=UN1T,DC=GROUP";
            string adSid = AdHelper.CreateSimpleAdUser(IdClientEtalon.ToString(), truePassword, Name, description:"клиент", adPath:adPath);
            AdSid = adSid;

            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            SqlParameter pIdClientEtalon = new SqlParameter() { ParameterName = "id_client_etalon", SqlValue = IdClientEtalon, SqlDbType = SqlDbType.Int };

            SqlParameter pAdSid = new SqlParameter() { ParameterName = "ad_sid", SqlValue = AdSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pName = new SqlParameter() { ParameterName = "name", SqlValue = Name, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pFullName = new SqlParameter() { ParameterName = "full_name", SqlValue = FullName, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pLosgin = new SqlParameter() { ParameterName = "login", SqlValue = Login, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pPassword = new SqlParameter() { ParameterName = "password", SqlValue = Password, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pZipAccess = new SqlParameter() { ParameterName = "zip_access", SqlValue = ZipAccess, SqlDbType = SqlDbType.Bit };
            SqlParameter pCounterAccess = new SqlParameter() { ParameterName = "counter_access", SqlValue = CounterAccess, SqlDbType = SqlDbType.Bit };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };

            //Получаем имя контрагента

            

            //TODO: Завернуть в транзакцию

            var dt = Db.Service.ExecuteQueryStoredProcedure("save_client_access", pId, pIdClientEtalon, pAdSid, pFullName, pName, pLosgin, pPassword, pZipAccess, pCounterAccess, pCreatorAdSid);
            int id = 0;
            if (dt.Rows.Count > 0)
            {
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;
            }

            //Назначаем доступ клиенту
            var clAcc = new ClientAccess(Id);
            if (clAcc.ZipAccess || clAcc.CounterAccess)
            {
                AdHelper.IncludeUser2AdGroup(clAcc.AdSid, AdGroup.ZipClaimClient);
            }
            else
            {
                AdHelper.ExcludeUserFromAdGroup(clAcc.AdSid, AdGroup.ZipClaimClient);
            }

            if (ZipAccess)
            {
                AdHelper.IncludeUser2AdGroup(clAcc.AdSid, AdGroup.ZipClaimClientZipView);
            }
            else
            {
                AdHelper.ExcludeUserFromAdGroup(clAcc.AdSid, AdGroup.ZipClaimClientZipView);
            }

            if (CounterAccess)
            {
                AdHelper.IncludeUser2AdGroup(clAcc.AdSid, AdGroup.ZipClaimClientCounterView);
            }
            else
            {
                AdHelper.ExcludeUserFromAdGroup(clAcc.AdSid, AdGroup.ZipClaimClientCounterView);
            }
        }

        public static IEnumerable<ClientAccess> GetList()
        {
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_client_access");

            var lst = new List<ClientAccess>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new ClientAccess(row);
                lst.Add(model);
            }

            return lst;
        }

        public static void Close(int id, string deleterSid)
        {
            var clAcc = new ClientAccess(id);
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            SqlParameter pDeleterSid = new SqlParameter() { ParameterName = "deleter_sid", SqlValue = deleterSid, SqlDbType = SqlDbType.VarChar };
            var dt = Db.Service.ExecuteQueryStoredProcedure("close_client_access", pId, pDeleterSid);

            //Убираем доступы клиента
            AdHelper.ExcludeUserFromAdGroup(clAcc.AdSid, AdGroup.ZipClaimClient);
            AdHelper.ExcludeUserFromAdGroup(clAcc.AdSid, AdGroup.ZipClaimClientZipView);
            AdHelper.ExcludeUserFromAdGroup(clAcc.AdSid, AdGroup.ZipClaimClientCounterView);

        }
    }
}