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
    public class OrgStateImage : DbModel
    {
        public int Id { get; set; }
        public int IdOrganization { get; set; }
        public byte[] Image { get; set; }

        public OrgStateImage()
        {
        }

        public OrgStateImage(DataRow row)
        {
            FillSelf(row);
        }

        //public OrgStateImage(int id)
        //{
        //    SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
        //    var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_org_state_image", pId);
        //    if (dt.Rows.Count > 0)
        //    {
        //        var row = dt.Rows[0];
        //        FillSelf(row);
        //    }
        //}

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueInt(row["id"]);
            IdOrganization = Db.DbHelper.GetValueInt(row["id_organization"]);
            Image = (byte[]) row["data"];
        }

        public static IEnumerable<OrgStateImage> GetList(int idOrganization)
        {
            SqlParameter pIdOrganization = new SqlParameter() { ParameterName = "id_organization", SqlValue = idOrganization, SqlDbType = SqlDbType.Int };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_org_state_image", pIdOrganization);
            var lst = new List<OrgStateImage>();
            foreach (DataRow row in dt.Rows)
            {
                var org = new OrgStateImage(row);
                lst.Add(org);
            }
            return lst;
        }

        public void Save()
        {
            //if (Creator == null) Creator = new Employee();
            SqlParameter pIdOrganization = new SqlParameter() { ParameterName = "id_organization", SqlValue = IdOrganization, SqlDbType = SqlDbType.Int };
            SqlParameter pImage = new SqlParameter() { ParameterName = "data", SqlValue = Image, SqlDbType = SqlDbType.VarBinary };
            //SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };


            var dt = Db.Stuff.ExecuteQueryStoredProcedure("save_org_state_image", pIdOrganization, pImage);
            if (dt.Rows.Count > 0)
            {
                int id;
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;


            }
        }

        public static void Close(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            Db.Stuff.ExecuteStoredProcedure("close_orgstate_image", pId);
        }
    }
}