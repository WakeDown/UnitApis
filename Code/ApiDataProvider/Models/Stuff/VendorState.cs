using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Models.SpeCalc;
using DataProvider.Objects;
using Microsoft.Ajax.Utilities;
using Microsoft.OData.Core.UriParser.Semantic;

namespace DataProvider.Models.Stuff
{
    public class VendorState:DbModel
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string StateDiscription { get; set; }
        public byte[] Picture { get; set; }
        public DateTime EndDate { get; set; }
        public int UnitOrganizationId { get; set; }
        public string UnitOrganizationName { get; set; }
        public int LanguageId { get; set; }
        public string LanguageName { get; set; }
        public string Author { get; set; }
        public DateTime CreationDate { get; set; }

        public VendorState() {}

        public VendorState(int id)
        {
            SqlParameter pId = new SqlParameter()
            {
                ParameterName = "id",
                SqlValue = id,
                SqlDbType = SqlDbType.Int
            };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_vendor_state", pId);
            FillSelf(dt.Rows[0]);
        }
        public VendorState(DataRow row)
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            VendorId = Db.DbHelper.GetValueIntOrDefault(row, "id_vendor");
            VendorName = Db.DbHelper.GetValueString(row,"vendor_name");
            StateDiscription = Db.DbHelper.GetValueString(row, "descr");
            Picture = Db.DbHelper.GetByteArr(row, "pic_data"); 
            EndDate = Db.DbHelper.GetValueDateTimeOrDefault(row, "date_end");
            UnitOrganizationId = Db.DbHelper.GetValueIntOrDefault(row, "id_organization");
            UnitOrganizationName =Db.DbHelper.GetValueString(row, "organization_name");
       //     UnitOrganizationId = Db.DbHelper.GetValueIntOrDefault(row, )
            LanguageId = Db.DbHelper.GetValueIntOrDefault(row, "id_language");
            LanguageName = Db.DbHelper.GetValueString(row, "language");
            Author = Db.DbHelper.GetValueString(row, "creator");
            CreationDate = Db.DbHelper.GetValueDateTimeOrDefault(row, "date_create");
        }

        public static IEnumerable<VendorState> GetList()
        {
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_vendor_state_list");
            var list = new List<VendorState>();
            foreach (DataRow row in dt.Rows)
            {
                var vnd = new VendorState(row);
                list.Add(vnd);
            }
            return (list);
        }
        public static IEnumerable<VendorState> GetHistoryList(int id)
        {
            SqlParameter pId =new SqlParameter()
            {
                ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int
            };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_vendor_state_history",pId);
            var list = new List<VendorState>();
            foreach (DataRow row in dt.Rows)
            {
                var vnd = new VendorState(row);
                list.Add(vnd);
            }
            return (list);
        }
        internal void Save()
        {
            SqlParameter pId = new SqlParameter() {ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int};
            SqlParameter pIdVendor = new SqlParameter()
            {
                ParameterName = "id_vendor",
                SqlValue = VendorId,
                SqlDbType = SqlDbType.Int
            };
            SqlParameter pDescription = new SqlParameter()
            {
                ParameterName = "descr",
                SqlValue = StateDiscription,
                SqlDbType = SqlDbType.NVarChar
            };
            SqlParameter pDateEnd = new SqlParameter()
            {
                ParameterName = "date_end",
                SqlValue = EndDate,
                SqlDbType = SqlDbType.DateTime
            };
            SqlParameter pIdOrganization = new SqlParameter()
            {
                ParameterName = "id_organization",
                SqlValue = UnitOrganizationId,
                SqlDbType = SqlDbType.Int
            };
            SqlParameter pIdLanguage = new SqlParameter()
            {
                ParameterName = "id_language",
                SqlValue = LanguageId,
                SqlDbType = SqlDbType.Int
            };
            SqlParameter pCreatorSid = new SqlParameter()
            {
                ParameterName = "creator_sid",
                SqlValue = CurUserAdSid,
                SqlDbType = SqlDbType.VarChar
            };
            SqlParameter pPicData = new SqlParameter()
            {
                ParameterName = "pic_data",
                SqlValue = Picture,
                SqlDbType = SqlDbType.VarBinary
            };
            try
            {
                var dt = Db.Stuff.ExecuteQueryStoredProcedure("save_vendor_state", pId, pIdVendor, pDescription,
                    pDateEnd, pIdOrganization, pIdLanguage, pCreatorSid, pPicData);

                if (dt.Rows.Count > 0)
                {
                    int id;
                    int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                    Id = id;
                }
            }
            catch (Exception ex)
            {
                
            }
        }
        public static void Close(int id, string deleter)
        {
            try
            {
                SqlParameter pDeleter = new SqlParameter()
                {
                    ParameterName = "deleter_sid",
                    SqlValue = deleter,
                    SqlDbType = SqlDbType.VarChar
                };
                SqlParameter pId = new SqlParameter()
                {
                    ParameterName = "id",
                    SqlValue = id,
                    SqlDbType = SqlDbType.Int
                };
                Db.Stuff.ExecuteStoredProcedure("close_vendor_state", pId, pDeleter);
            }
            catch (Exception ex)
            {
            }
            


        }
    }
}
