using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Objects;
using DataProvider._TMPLTS;

namespace DataProvider.Models.Service
{
    public class ServiceSheetZipItem:DbModel
    {
        public int Id { get; set; }
        public int ServiceSheetId { get; set; }
        public string PartNum { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public int? ZipClaimUnitId { get; set; }
        public DateTime DateCreate { get; set; }
        public string CreatorSid { get; set; }
        public bool Installed { get; set; }
        public string InstalledSid { get; set; }
        public string InstalledCancelSid { get; set; }
        public int InstalledServiceSheetId { get; set; }

        public ServiceSheetZipItem() { }

        public ServiceSheetZipItem(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("service_sheet_zip_item_get", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
        }

        public ServiceSheetZipItem(DataRow row)
            : this()
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            ServiceSheetId = Db.DbHelper.GetValueIntOrDefault(row, "id_service_sheet");
            Name = Db.DbHelper.GetValueString(row, "name");
            PartNum = Db.DbHelper.GetValueString(row, "part_num");
            Count = Db.DbHelper.GetValueIntOrDefault(row, "count");
            ZipClaimUnitId = Db.DbHelper.GetValueIntOrNull(row, "id_zip_claim_unit");
            DateCreate = Db.DbHelper.GetValueDateTimeOrDefault(row, "date_create");
            CreatorSid =Db.DbHelper.GetValueString(row, "creator_sid");
            Installed = Db.DbHelper.GetValueBool(row, "installed");
            InstalledSid = Db.DbHelper.GetValueString(row, "installed_sid");
            InstalledCancelSid = Db.DbHelper.GetValueString(row, "installed_cancel_sid");
            InstalledServiceSheetId = Db.DbHelper.GetValueIntOrDefault(row, "installed_id_service_sheet");
        }

        public void Save()
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            SqlParameter pServiceSheetId = new SqlParameter() { ParameterName = "id_service_sheet", SqlValue = ServiceSheetId, SqlDbType = SqlDbType.Int };
            SqlParameter pName = new SqlParameter() { ParameterName = "name", SqlValue = Name, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pPartNum = new SqlParameter() { ParameterName = "part_num", SqlValue = PartNum, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCount = new SqlParameter() { ParameterName = "count", SqlValue = Count, SqlDbType = SqlDbType.Int };
            //SqlParameter pZipClaimUnitId = new SqlParameter() { ParameterName = "id_zip_claim_unit", SqlValue = ZipClaimUnitId, SqlDbType = SqlDbType.Int };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };

            var dt = Db.Service.ExecuteQueryStoredProcedure("service_sheet_zip_item_save", pId, pServiceSheetId,pName, pPartNum, pCount, pCreatorAdSid);
            int id = 0;
            if (dt.Rows.Count > 0)
            {
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;
            }
        }

        public static IEnumerable<ServiceSheetZipItem> GetList(int serviceSheetId)
        {
            SqlParameter pServiceSheetId = new SqlParameter() { ParameterName = "id_service_sheet", SqlValue = serviceSheetId, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("service_sheet_zip_item_get_list", pServiceSheetId);

            var lst = new List<ServiceSheetZipItem>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new ServiceSheetZipItem(row);
                lst.Add(model);
            }

            return lst;
        }
        public static IEnumerable<ServiceSheetZipItem> GetInstalledList(int serviceSheetId)
        {
            SqlParameter pServiceSheetId = new SqlParameter() { ParameterName = "id_service_sheet", SqlValue = serviceSheetId, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("service_sheet_zip_item_get_installed_list", pServiceSheetId);

            var lst = new List<ServiceSheetZipItem>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new ServiceSheetZipItem(row);
                lst.Add(model);
            }

            return lst;
        }
        

        public static void Close(int id, string deleterSid)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            SqlParameter pDeleterSid = new SqlParameter() { ParameterName = "deleter_sid", SqlValue = deleterSid, SqlDbType = SqlDbType.VarChar };
            var dt = Db.Service.ExecuteQueryStoredProcedure("service_sheet_zip_item_close", pId, pDeleterSid);
        }

        public static void SetInstalled(int id, int idServiceSheet, string creatorSid, bool? installed=true)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            SqlParameter pIdServiceSheet = new SqlParameter() { ParameterName = "id_service_sheet", SqlValue = idServiceSheet, SqlDbType = SqlDbType.Int };
            SqlParameter pCreatorSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = creatorSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pInstalled = new SqlParameter() { ParameterName = "installed", SqlValue = installed, SqlDbType = SqlDbType.Bit };
            var dt = Db.Service.ExecuteQueryStoredProcedure("service_sheet_zip_item_installed", pId, pIdServiceSheet, pInstalled, pCreatorSid);
        }
    }
}