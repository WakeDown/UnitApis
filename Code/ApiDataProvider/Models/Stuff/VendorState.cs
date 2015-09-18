using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
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
        public string StateName { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string StateDescription { get; set; }
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
            StateName = Db.DbHelper.GetValueString(row, "name");
            VendorId = Db.DbHelper.GetValueIntOrDefault(row, "id_vendor");
            VendorName = Db.DbHelper.GetValueString(row,"vendor_name");
            StateDescription = Db.DbHelper.GetValueString(row, "descr");
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
        public static void SetDeliverySent(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            Db.Stuff.ExecuteStoredProcedure("set_vendor_state_delivery_sent", pId);
        }
        public static IEnumerable<VendorState> ExpiredList()
        {
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_expires_vendor_state_list");
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
            SqlParameter pId = new SqlParameter()
            {
                ParameterName = "id",
                SqlValue = Id,
                SqlDbType = SqlDbType.Int
            };
            SqlParameter pName = new SqlParameter()
            {
                ParameterName = "name",
                SqlValue = StateName,
                SqlDbType = SqlDbType.NChar
            };
            SqlParameter pIdVendor = new SqlParameter()
            {
                ParameterName = "id_vendor",
                SqlValue = VendorId,
                SqlDbType = SqlDbType.Int
            };
            SqlParameter pDescription = new SqlParameter()
            {
                ParameterName = "descr",
                SqlValue = StateDescription,
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

                var dt = Db.Stuff.ExecuteQueryStoredProcedure("save_vendor_state", pId,pName, pIdVendor, pDescription,
                    pDateEnd, pIdOrganization, pIdLanguage, pCreatorSid, pPicData);
                var changed = (Id == null || Id == 0)
                    ? true
                    :Db.Stuff.ExecuteQueryStoredProcedure("check_vendor_state_is_changed", pId).Rows[0].ItemArray.Length > 1;
                if (changed)
                {
                    string subject = (Id == null || Id == 0) ? "New" : "Edit";
                    var mailTo = GetMailAddressList();
                    VendorName = new Vendor(VendorId).Name;
                    MessageHelper.SendMailSmtp(subject, VendorName, false, mailTo, null, null, true);
                }
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

        public static IEnumerable<string> GetMailAddressList()
        {
            var sidList = AdHelper.GetUserListByAdGroup(AdGroup.VendorStateDelivery);
            var mailList = new List<string>();
            foreach (var sid in sidList)
            {
                var mail = new Employee(sid.Key).Email;
                mailList.Add(mail);
            }
            return (mailList);
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
