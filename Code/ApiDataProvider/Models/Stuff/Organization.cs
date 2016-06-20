using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Objects;

namespace DataProvider.Models.Stuff
{
    public class Organization : DbModel
    {
        public string Code { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int EmpCount { get; set; }
        //public Employee Creator { get; set; }
        public string AddressUr { get; set; }
        public string AddressFact { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Inn { get; set; }
        public string Kpp { get; set; }
        public string Ogrn { get; set; }
        public string Rs { get; set; }
        public string Bank { get; set; }
        public string Ks { get; set; }
        public string Bik { get; set; }
        public string Okpo { get; set; }
        public string Okved { get; set; }
        public string ManagerShortName { get; set; }
        public string ManagerName { get; set; }
        public string ManagerNameDat { get; set; }
        public string ManagerPosition { get; set; }
        public string ManagerPositionDat { get; set; }
        public string Site { get; set; }
        public Employee Director { get; set; }

        public IEnumerable<OrgStateImage> StateImages { get; set; }

        public Organization()
        {
        }

        public Organization(DataRow row)
        {
            FillSelf(row);
        }

        public Organization(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_organization", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
                StateImages = OrgStateImage.GetList(Id);
            }
        }

        public Organization(string sysName)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "sys_name", SqlValue = sysName, SqlDbType = SqlDbType.NVarChar };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_organization", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
                StateImages = OrgStateImage.GetList(Id);
            }
        }

        private void FillSelf(DataRow row)
        {
            Code = row["code"].ToString();
            Id = Db.DbHelper.GetValueInt(row["id"]);
            Name = row["name"].ToString();
            EmpCount = Db.DbHelper.GetValueInt(row["emp_count"]);
            AddressUr = row["address_ur"].ToString();
            AddressFact = row["address_fact"].ToString();
            Phone = row["phone"].ToString();
            Email = row["email"].ToString();
            Inn = row["inn"].ToString();
            Kpp = row["kpp"].ToString();
            Ogrn = row["ogrn"].ToString();
            Rs = row["rs"].ToString();
            Bank = row["bank"].ToString();
            Ks = row["ks"].ToString();
            Bik = row["bik"].ToString();
            Okpo = row["okpo"].ToString();
            Okved = row["okved"].ToString();
            //ManagerName = row["manager_name"].ToString();
            //ManagerNameDat = row["manager_name_dat"].ToString();
            //ManagerPosition = row["manager_position"].ToString();
            //ManagerPositionDat = row["manager_position_dat"].ToString();
            Director = new Employee() { Id=Db.DbHelper.GetValueIntOrDefault(row["id_director"]), AdSid = row["director_sid"].ToString() }; 
            Site = row["site"].ToString();
            //string[] nameArr = ManagerName.Split(' ');
            //for (int i = 0;i<nameArr.Count(); i++)
            //{
            //    string name = nameArr[i];
                
            //    if (i > 0) name = name[0] + ".";
            //    if (i == 1) name = " " + name;
            //    ManagerShortName += name;
            //}
            
        }

        public static IEnumerable<Organization> GetList()
        {
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_organization");
            var lst = new List<Organization>();
            foreach (DataRow row in dt.Rows)
            {
                var org = new Organization(row);
                lst.Add(org);
            }
            return lst;
        }

        public void Save()
        {
            //if (Creator == null) Creator = new Employee();
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            SqlParameter pName = new SqlParameter() { ParameterName = "name", SqlValue = Name, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pCode = new SqlParameter() { ParameterName = "code", SqlValue = Code, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pAddressUr = new SqlParameter() { ParameterName = "address_ur", SqlValue = AddressUr, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pAddressFact = new SqlParameter() { ParameterName = "address_fact", SqlValue = AddressFact, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pPhone = new SqlParameter() { ParameterName = "phone", SqlValue = Phone, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pEmail = new SqlParameter() { ParameterName = "email", SqlValue = Email, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pInn = new SqlParameter() { ParameterName = "inn", SqlValue = Inn, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pKpp = new SqlParameter() { ParameterName = "kpp", SqlValue = Kpp, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pOgrn = new SqlParameter() { ParameterName = "ogrn", SqlValue = Ogrn, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pRs = new SqlParameter() { ParameterName = "rs", SqlValue = Rs, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pBank = new SqlParameter() { ParameterName = "bank", SqlValue = Bank, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pKs = new SqlParameter() { ParameterName = "ks", SqlValue = Ks, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pBik = new SqlParameter() { ParameterName = "bik", SqlValue = Bik, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pOkpo = new SqlParameter() { ParameterName = "okpo", SqlValue = Okpo, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pOkved = new SqlParameter() { ParameterName = "okved", SqlValue = Okved, SqlDbType = SqlDbType.NVarChar };
            //SqlParameter pManagerName = new SqlParameter() { ParameterName = "manager_name", SqlValue = ManagerName, SqlDbType = SqlDbType.NVarChar };
            //SqlParameter pManagerNameDat = new SqlParameter() { ParameterName = "manager_name_dat", SqlValue = ManagerNameDat, SqlDbType = SqlDbType.NVarChar };
            //SqlParameter pManagerPosition = new SqlParameter() { ParameterName = "manager_position", SqlValue = ManagerPosition, SqlDbType = SqlDbType.NVarChar };
            //SqlParameter pManagerPositionDat = new SqlParameter() { ParameterName = "manager_position_dat", SqlValue = ManagerPositionDat, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pSite = new SqlParameter() { ParameterName = "site", SqlValue = Site, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pDirectorSid = new SqlParameter() { ParameterName = "director_sid", SqlValue = Director.AdSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pIdDirector = new SqlParameter() { ParameterName = "id_director", SqlValue = Director.Id, SqlDbType = SqlDbType.Int };

            var dt = Db.Stuff.ExecuteQueryStoredProcedure("save_organization", pId, pName, pCreatorAdSid, pAddressUr, pAddressFact, pPhone, pEmail, pInn, pKpp, pOgrn, pRs, pBank, pKs, pBik, pOkpo, pOkved, pDirectorSid, pSite, pIdDirector, pCode);
            if (dt.Rows.Count > 0)
            {
                int id;
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;

                foreach (OrgStateImage image in StateImages)
                {
                    if (image.Id > 0) continue;
                    image.IdOrganization = id;
                    image.Save();
                }
            }
        }

        public static void Close(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };

            int count = (int)Db.Stuff.ExecuteScalar("get_organization_link_count", pId);

            if (count == 0)
            {
                SqlParameter pIdOrg = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
                Db.Stuff.ExecuteStoredProcedure("close_organization", pIdOrg);
            }
            else
            {
                throw new Exception("Невозможно удалить юр. лицо так как есть привязка к сотрудникам!");
            }
        }
    }
}