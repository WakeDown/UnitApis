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
    public class ClaimStateGroup:DbModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SysName { get; set; }
        public int OrderNum { get; set; }
        public string BackgroundColor { get; set; }
        public string ForegroundColor { get; set; }
        public string BorderColor { get; set; }
        public int ClaimCount { get; set; }

        public ClaimStateGroup(DataRow row)
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            Name = Db.DbHelper.GetValueString(row, "name");
            SysName = Db.DbHelper.GetValueString(row, "sys_name");
            OrderNum = Db.DbHelper.GetValueIntOrDefault(row, "order_num");
            BackgroundColor = Db.DbHelper.GetValueString(row, "background_color");
            ForegroundColor = Db.DbHelper.GetValueString(row, "foreground_color");
            BorderColor = Db.DbHelper.GetValueString(row, "border_color");
            ClaimCount = Db.DbHelper.GetValueIntOrDefault(row, "cnt");
        }

        public static IEnumerable<ClaimStateGroup> GetFilterList(AdUser user, string adminSid = null, string engeneerSid = null, DateTime? dateStart = null, DateTime? dateEnd = null, int? topRows = null, string managerSid = null, string techSid = null, string serialNum = null, int? idDevice = null, bool? activeClaimsOnly = false, int? idClaimState = null, int? clientId = null, string clientSdNum = null, int? claimId = null, string deviceName = null, int? pageNum = null, string groupStates = null, string address = null, string servManagerSid = null)
        {
            if (user.Is(AdGroup.ServiceAdmin)) { adminSid = user.Sid; }
            if (user.Is(AdGroup.ServiceEngeneer)) engeneerSid = user.Sid;
            if (user.Is(AdGroup.ServiceCenterManager)) servManagerSid = user.Sid;
            if (user.Is(AdGroup.ServiceManager)) managerSid = user.Sid;
            if (user.Is(AdGroup.ServiceTech)) techSid = user.Sid;


            if (!topRows.HasValue) topRows = 30;
            if (!pageNum.HasValue) pageNum = 1;

            SqlParameter pServAdminSid = new SqlParameter() { ParameterName = "admin_sid", SqlValue = adminSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pServEngeneerSid = new SqlParameter() { ParameterName = "engeneer_sid", SqlValue = engeneerSid, SqlDbType = SqlDbType.VarChar };
            //МСЦ - менеджер сервисного центра
            SqlParameter pServManagerSid = new SqlParameter() { ParameterName = "serv_manager_sid", SqlValue = servManagerSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pDateStart = new SqlParameter() { ParameterName = "date_start", SqlValue = dateStart, SqlDbType = SqlDbType.Date };
            SqlParameter pDateEnd = new SqlParameter() { ParameterName = "date_end", SqlValue = dateEnd, SqlDbType = SqlDbType.Date };
            SqlParameter pTopRows = new SqlParameter() { ParameterName = "top_rows", SqlValue = topRows, SqlDbType = SqlDbType.Int };
            SqlParameter pManagerSid = new SqlParameter() { ParameterName = "manager_sid", SqlValue = managerSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pTechSid = new SqlParameter() { ParameterName = "tech_sid", SqlValue = techSid, SqlDbType = SqlDbType.VarChar };
            SqlParameter pSerialNum = new SqlParameter() { ParameterName = "serial_num", SqlValue = serialNum, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pIdDevice = new SqlParameter() { ParameterName = "id_device", SqlValue = idDevice, SqlDbType = SqlDbType.Int };
            SqlParameter pActiveClaimsOnly = new SqlParameter() { ParameterName = "active_claims_only", SqlValue = activeClaimsOnly, SqlDbType = SqlDbType.Bit };
            SqlParameter pIdClaimState = new SqlParameter() { ParameterName = "id_claim_state", SqlValue = idClaimState, SqlDbType = SqlDbType.Int };
            SqlParameter pClientId = new SqlParameter() { ParameterName = "id_client", SqlValue = clientId, SqlDbType = SqlDbType.Int };
            SqlParameter pClientSdNum = new SqlParameter() { ParameterName = "client_sd_num", SqlValue = clientSdNum, SqlDbType = SqlDbType.Int };
            SqlParameter pclaimId = new SqlParameter() { ParameterName = "claim_id", SqlValue = claimId, SqlDbType = SqlDbType.Int };
            SqlParameter pDeviceName = new SqlParameter() { ParameterName = "device_name", SqlValue = deviceName, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pPageNum = new SqlParameter() { ParameterName = "page_num", SqlValue = pageNum, SqlDbType = SqlDbType.Int };
            SqlParameter pGroupStates = new SqlParameter() { ParameterName = "group_state_list", SqlValue = groupStates, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pAddress = new SqlParameter() { ParameterName = "address", SqlValue = address, SqlDbType = SqlDbType.NVarChar };

            //SqlParameter pSome = new SqlParameter() { ParameterName = "some", SqlValue = some, SqlDbType = SqlDbType.NVarChar };
            var dt = Db.Service.ExecuteQueryStoredProcedure("claim_claim_state_group_count", pServAdminSid, pServEngeneerSid, pDateStart, pDateEnd, pTopRows, pManagerSid, pTechSid, pSerialNum, pIdDevice, pActiveClaimsOnly, pIdClaimState, pClientId, pClientSdNum, pclaimId, pDeviceName, pPageNum, pGroupStates, pAddress, pServManagerSid);

            var lst = new List<ClaimStateGroup>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new ClaimStateGroup(row);
                lst.Add(model);
            }

            return lst;
        }
    }
}