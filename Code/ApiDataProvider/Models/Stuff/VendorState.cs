﻿using System;
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
using System.Text;
using System.Configuration;
using System.IO;
using System.Net.Mime;

namespace DataProvider.Models.Stuff
{
    public class VendorState : DbModel
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

        public VendorState() { }

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
            VendorName = Db.DbHelper.GetValueString(row, "vendor_name");
            StateDescription = Db.DbHelper.GetValueString(row, "descr");
            Picture = Db.DbHelper.GetByteArr(row, "pic_data");
            EndDate = Db.DbHelper.GetValueDateTimeOrDefault(row, "date_end");
            UnitOrganizationId = Db.DbHelper.GetValueIntOrDefault(row, "id_organization");
            UnitOrganizationName = Db.DbHelper.GetValueString(row, "organization_name");
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
            SqlParameter pId = new SqlParameter()
            {
                ParameterName = "id",
                SqlValue = id,
                SqlDbType = SqlDbType.Int
            };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_vendor_state_history", pId);
            var list = new List<VendorState>();
            foreach (DataRow row in dt.Rows)
            {
                var vnd = new VendorState(row);
                list.Add(vnd);
            }
            return (list);
        }
        public static void SetDeliverySent(int id, byte expires, byte newbie, byte updated)
        {
            SqlParameter pExp = new SqlParameter()
            {
                ParameterName = "expired",
                SqlValue = expires,
                SqlDbType = SqlDbType.Bit
            };
            SqlParameter pNew = new SqlParameter()
            {
                ParameterName = "new",
                SqlValue = newbie,
                SqlDbType = SqlDbType.Bit
            };
            SqlParameter pUpd = new SqlParameter()
            {
                ParameterName = "updated",
                SqlValue = updated,
                SqlDbType = SqlDbType.Bit
            };
            SqlParameter pId = new SqlParameter()
            {
                ParameterName = "id",
                SqlValue = id,
                SqlDbType = SqlDbType.Int
            };
            Db.Stuff.ExecuteStoredProcedure("set_vendor_state_delivery_sent", pId, pExp, pNew, pUpd);
        }

        public static VendorState GetPrevValue(int id)
        {
            if (id != 0)
            {
                SqlParameter pId = new SqlParameter()
                {
                    ParameterName = "id",
                    SqlValue = id,
                    SqlDbType = SqlDbType.Int
                };
                var prev = new VendorState(Db.Stuff.ExecuteQueryStoredProcedure("get_vendor_state_prev_version", pId).Rows[0]);
                return prev;
            }
            return null;
        }
        public static IEnumerable<VendorState> DeliverList(byte expires, byte newbie, byte updated)
        {
            SqlParameter pExp =new SqlParameter()
            {
                ParameterName = "expires",
                SqlValue = expires,
                SqlDbType = SqlDbType.Bit
            };
            SqlParameter pNew = new SqlParameter()
            {
                ParameterName = "newbie",
                SqlValue = newbie,
                SqlDbType = SqlDbType.Bit
            };
            SqlParameter pUpd = new SqlParameter()
            {
                ParameterName = "updated",
                SqlValue = updated,
                SqlDbType = SqlDbType.Bit
            };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_expires_vendor_state_list", pExp, pNew, pUpd);
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
            bool isNew = Id == 0;

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

            var dt = Db.Stuff.ExecuteQueryStoredProcedure("save_vendor_state", pId, pName, pIdVendor, pDescription,
                pDateEnd, pIdOrganization, pIdLanguage, pCreatorSid, pPicData);
            var body = new StringBuilder("Добрый день.<br/>");
            UnitOrganizationName = new Organization(UnitOrganizationId).Name;
            VendorName = new Vendor(VendorId).Name;
            var mailTo = Employee.GetFullRecipientList(null);
            var stuffUrl = ConfigurationManager.AppSettings["StuffUrl"];

            if (dt.Rows.Count > 0)
            {
                int id;
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;
            }

            //if (isNew)
            //{

            //    var subject = string.Format("Новый статус {0} от {1}.", StateName, VendorName);
            //    body.AppendFormat("У организации {0} появился новый статус {1} от {2}.<br/>", UnitOrganizationName, StateName,
            //        VendorName);
            //    body.AppendFormat("Срок действия до {0}.<br/>", EndDate.ToShortDateString());
            //    body.AppendFormat("{0}<br/>", StateDescription);
            //    body.AppendFormat("<a href='{0}/VendorState/Index/#vs-{1}'>{0}/VendorState/Index/#vs-{1}</a><br/>", stuffUrl, Id);
            //    MemoryStream stream = new MemoryStream(new VendorState(Id).Picture.ToArray());;
            //    var file = new AttachmentFile() { Data = stream.ToArray(), FileName = "state.jpeg", DataMimeType = MediaTypeNames.Image.Jpeg };
            //    MessageHelper.SendMailSmtp(subject, body.ToString(), true, mailTo, null, null, file);
            //}
            //else
            //{
            //    var vnd = new VendorState((Db.Stuff.ExecuteQueryStoredProcedure("check_vendor_state_is_changed", pId)).Rows[0]);
            //    vnd.VendorName = new Vendor(vnd.VendorId).Name;
            //    vnd.UnitOrganizationName = new Organization(vnd.UnitOrganizationId).Name;
            //    var changed = vnd.Id != 0;
            //    if (changed)
            //    {
            //        string subject = string.Format("Обновление статуса {0} от {1}", vnd.StateName, vnd.VendorName);
            //        body.AppendFormat("Обновился статус {0} от {1} для организации {2}.<br/>", vnd.StateName, vnd.VendorName, vnd.UnitOrganizationName);
            //        body.Append("<br/>Новая версия статуса:<br/>");
            //        body.AppendFormat("Организация {0}<br/>", UnitOrganizationName);
            //        body.AppendFormat("Вендор {0}<br/>", VendorName);
            //        body.AppendFormat("Статус {0}<br/>", StateName);
            //        body.AppendFormat("Срок действия до {0}<br/>", EndDate.ToShortDateString());
            //        body.AppendFormat("{0}<br/><a href='{1}/VendorState/Index/#vs-{2}'>{1}/VendorState/Index/#vs-{2}</a><br/>", StateDescription, stuffUrl, Id);
            //        MemoryStream stream = new MemoryStream(new VendorState(Id).Picture.ToArray()); ;
            //        var file = new AttachmentFile() { Data = stream.ToArray(), FileName = "state.jpeg", DataMimeType = MediaTypeNames.Image.Jpeg };
            //        MessageHelper.SendMailSmtp(subject, body.ToString(), true, mailTo, null, null, file);
            //    }
            //}
        }

        public static IEnumerable<string> GetMailAddressVendorStateExpiresDeliveryList()
        {
            var sidList = AdHelper.GetUserListByAdGroup(AdGroup.VendorStateExpiresDelivery);
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
    }
}
