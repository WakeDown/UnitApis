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
        DateTime? DateInstalled { get; set; }
        /// <summary>
        /// Предоставлено клиентом
        /// </summary>
        public bool ClientGiven { get; set; }

        public int ClaimId {get;set;}
        public string NomenclatureNum { get; set; }
        public string PriceIn { get; set; }
        public string PriceOut { get; set; }
        public string DeliveryTime { get; set; }
        public string NomenclatureClaimNum { get; set; }
        public bool NoNomenclatureNum { get; set; }
        public int? IdSupplyMan { get; set; }


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
            InstalledServiceSheetId = Db.DbHelper.GetValueIntOrDefault(row, "installed_id_service_sheet");
            DateInstalled = Db.DbHelper.GetValueDateTimeOrNull(row, "date_install");
            ClientGiven = Db.DbHelper.GetValueBool(row, "client_given");
        }

        public void SaveUnitProg(bool fromTop =false)
        {
            SqlParameter pAction = new SqlParameter() { ParameterName = "action", Value = "saveClaimUnit", SqlDbType = SqlDbType.NVarChar };
            SqlParameter pId = new SqlParameter() { ParameterName = "id_claim_unit", Value = Id, DbType = DbType.Int32 };
            SqlParameter pIdClaim = new SqlParameter() { ParameterName = "id_claim", Value = ClaimId, DbType = DbType.Int32 };
            SqlParameter pCatalogNum = new SqlParameter() { ParameterName = "catalog_num", Value = PartNum, DbType = DbType.AnsiString };
            SqlParameter pName = new SqlParameter() { ParameterName = "name", Value = Name, DbType = DbType.AnsiString };
            SqlParameter pCount = new SqlParameter() { ParameterName = "count", Value = Count, DbType = DbType.Int32 };
            SqlParameter pNomenclatureNum = new SqlParameter() { ParameterName = "nomenclature_num", Value = NomenclatureNum, DbType = DbType.AnsiString };
            SqlParameter pPriceIn = new SqlParameter() { ParameterName = "price_in", Value = PriceIn, DbType = DbType.AnsiString };
            SqlParameter pPriceOut = new SqlParameter() { ParameterName = "price_out", Value = PriceOut, DbType = DbType.AnsiString };
            SqlParameter pIdCreator = new SqlParameter() { ParameterName = "id_creator", Value = UserUnitProg.GetUserId(CurUserAdSid), DbType = DbType.Int32 };
            SqlParameter pDeliveryTime = new SqlParameter() { ParameterName = "delivery_time", Value = DeliveryTime, DbType = DbType.AnsiString };
            SqlParameter pFromTop = new SqlParameter() { ParameterName = "from_top", Value = fromTop, DbType = DbType.Boolean };
            SqlParameter pNomenclatureClaimNum = new SqlParameter() { ParameterName = "nomenclature_claim_num", Value = NomenclatureClaimNum, DbType = DbType.AnsiString };
            SqlParameter pIdSupplyMan = new SqlParameter() { ParameterName = "id_supply_man", Value = IdSupplyMan, DbType = DbType.Int32 };
            SqlParameter pNoNomenclatureNum = new SqlParameter() { ParameterName = "no_nomenclature_num", Value = NoNomenclatureNum, DbType = DbType.Boolean };

            DataTable dt = Db.UnitProg.ExecuteQueryStoredProcedure("ui_zip_claims", pAction, pId, pIdClaim, pCatalogNum, pName, pCount, pNomenclatureNum, pPriceIn, pPriceOut, pIdCreator, pDeliveryTime, pFromTop, pNomenclatureClaimNum, pIdSupplyMan, pNoNomenclatureNum);


            if (dt.Rows.Count > 0)
            {
                ZipClaimUnitId = (int)dt.Rows[0]["id_claim_unit"];
            }
        }

        public void IssuedSave()
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            SqlParameter pServiceSheetId = new SqlParameter() { ParameterName = "id_service_sheet", SqlValue = ServiceSheetId, SqlDbType = SqlDbType.Int };
            SqlParameter pName = new SqlParameter() { ParameterName = "name", SqlValue = Name, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pPartNum = new SqlParameter() { ParameterName = "part_num", SqlValue = PartNum, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCount = new SqlParameter() { ParameterName = "count", SqlValue = Count, SqlDbType = SqlDbType.Int };
            //SqlParameter pZipClaimUnitId = new SqlParameter() { ParameterName = "id_zip_claim_unit", SqlValue = ZipClaimUnitId, SqlDbType = SqlDbType.Int };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pCLientGiven = new SqlParameter() { ParameterName = "@client_given", SqlValue = ClientGiven, SqlDbType = SqlDbType.Bit };

            var dt = Db.Service.ExecuteQueryStoredProcedure("service_sheet_issued_zip_item_save", pId, pServiceSheetId,pName, pPartNum, pCount, pCreatorAdSid, pCLientGiven);
            int id = 0;
            if (dt.Rows.Count > 0)
            {
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;
            }
        }

        public static void NotInstalledSaveList(int[] idOrderedZipItem, int serviceSheetId, string creatorSid)
        {
            
                SqlParameter pId = new SqlParameter() { ParameterName = "id_ordered_zip_item_list", SqlValue = String.Join(",", idOrderedZipItem), SqlDbType = SqlDbType.Int };
                SqlParameter pServiceSheetId = new SqlParameter() { ParameterName = "id_service_sheet", SqlValue = serviceSheetId, SqlDbType = SqlDbType.Int };
                SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = creatorSid, SqlDbType = SqlDbType.VarChar };

                var dt = Db.Service.ExecuteQueryStoredProcedure("service_sheet_not_installed_zip_item_save_list", pId, pServiceSheetId, pCreatorAdSid);
        }

        public static IEnumerable<ServiceSheetZipItem> GetIssuedList(int serviceSheetId)
        {
            SqlParameter pServiceSheetId = new SqlParameter() { ParameterName = "id_service_sheet", SqlValue = serviceSheetId, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("service_sheet_issued_zip_item_get_list", pServiceSheetId);

            var lst = new List<ServiceSheetZipItem>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new ServiceSheetZipItem(row);
                lst.Add(model);
            }

            return lst;
        }

        /// <summary>
        /// Возвращает заказанный для сервисного листа ЗИП
        /// </summary>
        /// <param name="serviceSheetId"></param>
        /// <param name="realyOrdered">Был ли оформлен заказ ЗИП СТП или еще в промежуточной стадии</param>
        /// <returns></returns>
        public static IEnumerable<ServiceSheetZipItem> GetOrderedList(int serviceSheetId, bool? realyOrdered = null)
        {
            SqlParameter pServiceSheetId = new SqlParameter() { ParameterName = "id_service_sheet", SqlValue = serviceSheetId, SqlDbType = SqlDbType.Int };
            SqlParameter pRealyOrdered = new SqlParameter() { ParameterName = "ordered", SqlValue = realyOrdered, SqlDbType = SqlDbType.Bit };
            var dt = Db.Service.ExecuteQueryStoredProcedure("service_sheet_ordered_zip_item_get_list", pServiceSheetId, pRealyOrdered);

            var lst = new List<ServiceSheetZipItem>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new ServiceSheetZipItem(row);
                lst.Add(model);
            }

            return lst;
        }
        /// <summary>
        /// Устанавливает что ЗИП был заказан
        /// </summary>
        /// <param name="serviceSheetId"></param>
        public static void SetOrderedListRealyOrdered(int serviceSheetId)
        {
            SqlParameter pServiceSheetId = new SqlParameter() { ParameterName = "id_service_sheet", SqlValue = serviceSheetId, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("service_sheet_ordered_zip_item_set_ordered", pServiceSheetId);
        }

        public static IEnumerable<ServiceSheetZipItem> GetNotInstalledList(int serviceSheetId)
        {
            SqlParameter pServiceSheetId = new SqlParameter() { ParameterName = "id_service_sheet", SqlValue = serviceSheetId, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("service_sheet_not_installed_zip_item_get_list", pServiceSheetId);

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
            var dt = Db.Service.ExecuteQueryStoredProcedure("service_sheet_installed_zip_item_get_list", pServiceSheetId);

            var lst = new List<ServiceSheetZipItem>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new ServiceSheetZipItem(row);
                lst.Add(model);
            }

            return lst;
        }
        

        public static void IssuedClose(int id, string deleterSid)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            SqlParameter pDeleterSid = new SqlParameter() { ParameterName = "deleter_sid", SqlValue = deleterSid, SqlDbType = SqlDbType.VarChar };
            var dt = Db.Service.ExecuteQueryStoredProcedure("service_sheet_issued_zip_item_close", pId, pDeleterSid);
        }

        public void OrderedSave()
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.Int };
            SqlParameter pServiceSheetId = new SqlParameter() { ParameterName = "id_service_sheet", SqlValue = ServiceSheetId, SqlDbType = SqlDbType.Int };
            SqlParameter pName = new SqlParameter() { ParameterName = "name", SqlValue = Name, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pPartNum = new SqlParameter() { ParameterName = "part_num", SqlValue = PartNum, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCount = new SqlParameter() { ParameterName = "count", SqlValue = Count, SqlDbType = SqlDbType.Int };
            //SqlParameter pZipClaimUnitId = new SqlParameter() { ParameterName = "id_zip_claim_unit", SqlValue = ZipClaimUnitId, SqlDbType = SqlDbType.Int };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };

            var dt = Db.Service.ExecuteQueryStoredProcedure("service_sheet_ordered_zip_item_save", pId, pServiceSheetId, pName, pPartNum, pCount, pCreatorAdSid);
            int id = 0;
            if (dt.Rows.Count > 0)
            {
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;
            }
        }

        public static void OrderedClose(int id, string deleterSid)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            SqlParameter pDeleterSid = new SqlParameter() { ParameterName = "deleter_sid", SqlValue = deleterSid, SqlDbType = SqlDbType.VarChar };
            var dt = Db.Service.ExecuteQueryStoredProcedure("service_sheet_ordered_zip_item_close", pId, pDeleterSid);
        }

        public static void SetInstalled(int idOrderedZipItem, int idServiceSheet, string creatorSid, bool? installed=true)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id_ordered_zip_item", SqlValue = idOrderedZipItem, SqlDbType = SqlDbType.Int };
            SqlParameter pIdServiceSheet = new SqlParameter() { ParameterName = "id_service_sheet", SqlValue = idServiceSheet, SqlDbType = SqlDbType.Int };
            SqlParameter pCreatorSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = creatorSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pInstalled = new SqlParameter() { ParameterName = "installed", SqlValue = installed, SqlDbType = SqlDbType.Bit };
            var dt = Db.Service.ExecuteQueryStoredProcedure("service_sheet_installed_zip_item_save", pId, pIdServiceSheet, pInstalled, pCreatorSid);
        }

        public static void SaveOrderedZipItemsCopyFromIssued(int idServiceSheet, string creatorSid)
        {
            SqlParameter pIdServiceSheet = new SqlParameter() { ParameterName = "id_service_sheet", SqlValue = idServiceSheet, SqlDbType = SqlDbType.Int };
            SqlParameter pCreatorSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = creatorSid, SqlDbType = SqlDbType.VarChar };
            var dt = Db.Service.ExecuteQueryStoredProcedure("service_sheet_ordered_zip_item_copy_from_issued", pIdServiceSheet,  pCreatorSid);
        }
    }
}