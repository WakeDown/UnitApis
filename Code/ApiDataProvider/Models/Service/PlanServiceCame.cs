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
    public class PlanServiceCame:DbModel
    {
        public int Id { get; set; }
        public int IdServiceClaim { get; set; }
        public DateTime DateCame { get; set; }
        public string Descr { get; set; }
        public int Counter { get; set; }
        public int? CounterColor { get; set; }
        public string ServiceEngeneerSid { get; set; }
        public int IdServiceActionType { get; set; }
        public string ServiceActionTypeName { get; set; }
        public string CreatorSid { get; set; }
        public bool NoPay { get; set; }
        public bool? ProcessEnabled { get; set; }
        public bool? DeviceEnabled { get; set; }
        public bool? NeedZip { get; set; }
        public bool? NoCounter { get; set; }
        public bool? CounterAvailable { get; set; }
        public int IdDevice { get; set; }
        public int IdContract { get; set; }
        public int IdContractor { get; set; }
        public string ZipDescr { get; set; }
        public string DateWorkStart { get; set; }
        public string DateWorkEnd { get; set; }

        public PlanServiceCame() { }

        public PlanServiceCame(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id_service_came", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.UnitProg.ExecuteQueryStoredProcedure("get_service_came", pId);

            if (dt.Rows.Count > 0)
            {
                FillSelf(dt.Rows[0]);
            }
        }

        public PlanServiceCame(DataRow row)
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id_service_came");
            IdServiceClaim = Db.DbHelper.GetValueIntOrDefault(row, "id_service_claim");
            DateCame = Db.DbHelper.GetValueDateTimeOrDefault(row, "date_came");
            Descr = Db.DbHelper.GetValueString(row, "descr");
            Counter = Db.DbHelper.GetValueIntOrDefault(row, "counter");
            CounterColor = Db.DbHelper.GetValueIntOrDefault(row, "counter_colour");
            ServiceEngeneerSid = Db.DbHelper.GetValueString(row, "service_engeneer_sid");
            IdServiceActionType = Db.DbHelper.GetValueIntOrDefault(row, "id_service_action_type");
            ServiceActionTypeName = Db.DbHelper.GetValueString(row, "service_action_type_name");
            CreatorSid = Db.DbHelper.GetValueString(row, "creator_sid");
            NoPay = Db.DbHelper.GetValueBool(row, "no_pay");
            ProcessEnabled = Db.DbHelper.GetValueBoolOrNull(row, "process_enabled");
            DeviceEnabled = Db.DbHelper.GetValueBoolOrNull(row, "device_enabled");
            NeedZip = Db.DbHelper.GetValueBoolOrNull(row, "need_zip");
            NoCounter = Db.DbHelper.GetValueBoolOrNull(row, "no_counter");
            CounterAvailable = Db.DbHelper.GetValueBoolOrNull(row, "counter_unavailable");
            IdDevice= Db.DbHelper.GetValueIntOrDefault(row, "id_device");
            IdContract = Db.DbHelper.GetValueIntOrDefault(row, "id_contract");
            IdContractor = Db.DbHelper.GetValueIntOrDefault(row, "id_contractor");
            ZipDescr = Db.DbHelper.GetValueString(row, "zip_descr");
            DateWorkStart = Db.DbHelper.GetValueString(row, "date_work_start");
            DateWorkEnd = Db.DbHelper.GetValueString(row, "date_work_end");
        }
    }
}