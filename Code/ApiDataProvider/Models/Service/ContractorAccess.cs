using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Models.Stuff;
using DataProvider.Objects;

namespace DataProvider.Models.Service
{
    public class ContractorAccess : DbModel
    {
        public int Id { get; set; }
        public string AdSid { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string OrgName { get; set; }
        public string City { get; set; }
        public string OrgSid { get; set; }
        public string Email { get; set; }


        public ContractorAccess() { }

        public ContractorAccess(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_contractor_access", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
        }

        public ContractorAccess(DataRow row)
            : this()
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            AdSid = Db.DbHelper.GetValueString(row, "ad_sid");
            Name = Db.DbHelper.GetValueString(row, "name");
            Login = Db.DbHelper.GetValueString(row, "login");

            string passEncoded = Db.DbHelper.GetValueString(row, "password");
            if (!String.IsNullOrEmpty(passEncoded) && !passEncoded.EndsWith("-Z") && passEncoded.Length > 10)
            {
                Password = MathHelper.Decrypt(passEncoded, passSalt);
            }
            else
            {
                Password = passEncoded;
            }

            OrgName = Db.DbHelper.GetValueString(row, "org_name");
            City = Db.DbHelper.GetValueString(row, "city");
            OrgSid = Db.DbHelper.GetValueString(row, "org_sid");
            Email = Db.DbHelper.GetValueString(row, "email");
        }

        private const string passSalt = "UN1T32154";

        public void Save()
        {
            if (String.IsNullOrEmpty(Name)) throw new ArgumentException("Не указано имя подрядчика");

            Password = MathHelper.GenerateSimplePassword();
            string truePassword = Password;
            Password = MathHelper.Encrypt(Password, passSalt);

            string adPath = AdOrganization.GetAdPathByAdOrg(AdOrg.ZipClient);
            var nameArr = Name.Split(' ');
            string surname = nameArr[0];
            string name = nameArr.Count() > 1 ? nameArr[1] : nameArr[0];
            Login = AdHelper.GenerateLoginByName(surname, name);
            string adSid = AdHelper.CreateSimpleAdUser(Login, truePassword, Name, description: "подрядчик", adPath: adPath);
            AdSid = adSid;

            if (String.IsNullOrEmpty(OrgSid) || OrgSid.ToUpper().Equals("NEW"))
            {
                string adPathEngGroups = AdOrganization.GetAdPathByAdOrg(AdOrg.EngeneerGroups);
                //Создаем группу в AD
                string orgSid = AdHelper.CreateAdGroup($"{OrgName} --- {City}", adPathEngGroups);
                OrgSid = orgSid;
            }
            else
            {
                string displName = AdHelper.GetADGroupNameBySid(OrgSid);
                if (displName.Contains("---"))
                {
                    OrgName = displName.Remove(displName.IndexOf("---", StringComparison.Ordinal)-1);
                    City = displName.Remove(0, displName.IndexOf("---", StringComparison.Ordinal)+4);
                }
            }

            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            SqlParameter pAdSid = new SqlParameter() { ParameterName = "ad_sid", SqlValue = AdSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pName = new SqlParameter() { ParameterName = "name", SqlValue = Name, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pLogin = new SqlParameter() { ParameterName = "login", SqlValue = Login, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pPassword = new SqlParameter() { ParameterName = "password", SqlValue = Password, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pOrgName = new SqlParameter() { ParameterName = "org_name", SqlValue = OrgName, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCity = new SqlParameter() { ParameterName = "city", SqlValue = City, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pOrgSid = new SqlParameter() { ParameterName = "org_sid", SqlValue = OrgSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pEmail = new SqlParameter() { ParameterName = "email", SqlValue = Email, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };

            var dt = Db.Service.ExecuteQueryStoredProcedure("save_contractor_access", pId, pAdSid, pName, pLogin, pPassword, pOrgName, pCity, pOrgSid, pEmail, pCreatorAdSid);
            int id = 0;
            if (dt.Rows.Count > 0)
            {
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;
            }

            AdHelper.IncludeUser2AdGroup(AdSid, OrgSid);
            AdHelper.IncludeUser2AdGroup(AdSid, AdGroup.ZipClaimAddressChange, AdGroup.ServiceEngeneer);
        }

        public static IEnumerable<ContractorAccess> GetList()
        {
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_contractor_access");

            var lst = new List<ContractorAccess>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new ContractorAccess(row);
                lst.Add(model);
            }

            return lst;
        }

        public static void Close(int id, string deleterSid)
        {
            var ctrAcc = new ContractorAccess(id);
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            SqlParameter pDeleterSid = new SqlParameter() { ParameterName = "deleter_sid", SqlValue = deleterSid, SqlDbType = SqlDbType.VarChar };
            var dt = Db.Service.ExecuteQueryStoredProcedure("close_contractor_access", pId, pDeleterSid);

            AdHelper.ExcludeUserFromAdGroup(ctrAcc.AdSid, ctrAcc.OrgSid);
            AdHelper.ExcludeUserFromAdGroup(ctrAcc.AdSid, AdGroup.ZipClaimAddressChange, AdGroup.ServiceEngeneer);
        }
    }
}