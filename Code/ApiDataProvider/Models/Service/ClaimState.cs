using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Models.Stuff;
using DataProvider.Objects;
using DataProvider._TMPLTS;

namespace DataProvider.Models.Service
{
    public class ClaimState : DbModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SysName { get; set; }
        public int OrderNum { get; set; }
        public string BackgroundColor { get; set; }
        public string ForegroundColor { get; set; }
        public int ClaimCount { get; set; }
        

        public ClaimState() { }

        public ClaimState(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_claim_state", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
        }

        public ClaimState(string sysName)
        {
            SqlParameter pSysName = new SqlParameter() { ParameterName = "sys_name", SqlValue = sysName, SqlDbType = SqlDbType.NVarChar };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_claim_state", pSysName);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
        }

        public ClaimState(DataRow row)
            : this()
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id", "id_claim_state");
            Name = Db.DbHelper.GetValueString(row, "name");
            SysName = Db.DbHelper.GetValueString(row, "sys_name");
            OrderNum = Db.DbHelper.GetValueIntOrDefault(row, "order_num");
            BackgroundColor = Db.DbHelper.GetValueString(row, "background_color");
            ForegroundColor = Db.DbHelper.GetValueString(row, "foreground_color");
            ClaimCount = Db.DbHelper.GetValueIntOrDefault(row, "cnt");
        }

        public static IEnumerable<ClaimState> GetFilterList()
        {
            //SqlParameter pSome = new SqlParameter() { ParameterName = "some", SqlValue = some, SqlDbType = SqlDbType.NVarChar };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_claim_state_list_filter");

            var lst = new List<ClaimState>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new ClaimState(row);
                lst.Add(model);
            }

            return lst;
        }

        internal static ClaimState GetFirstState()
        {
            return GetNewState();
        }

        internal static ClaimState GetNewState()
        {
            return new ClaimState("NEW");
        }

        internal static ClaimState GetEndState()
        {
            return new ClaimState("END");
        }

        public static ClaimState GetNext(int idClaimState, int claimId)
        {
            var currState = new ClaimState(idClaimState);

            switch (currState.SysName.ToUpper())
            {
                case "NEW":
                    return new ClaimState("NEWADD");
                case "NEWADD":
                    return new ClaimState("SET");
                case "SET":
                    var wtId = new Claim(claimId).IdWorkType;
                    if (!wtId.HasValue) throw new ArgumentException("Невозможно определить следующий статус. Тип работ не указан.");
                    var wtSysName = new WorkType(wtId.Value).SysName;
                    switch (wtSysName)
                    {
                        case "ДНО": case "НПР": case "ТЭО":case "УТЗ":
                            return new ClaimState("TECHWORK");
                        case "РТО": case "МТС": case "УРМ": case "ЗРМ":
                        case "МДО": case "ИПТ": case "РЗРД": case "ЗНЗЧ":
                            return new ClaimState("SRVADMWORK");
                    }

                    break;
                default:
                    return currState;
            }
            

            //var st = new ClaimState();
            //SqlParameter pIdClaim = new SqlParameter() { ParameterName = "id_claim_state", SqlValue = idClaimState, SqlDbType = SqlDbType.Int };
            //var dt = Db.Service.ExecuteQueryStoredProcedure("get_next_claim_state", pIdClaim);
            //if (dt.Rows.Count > 0)
            //{
            //    st = new ClaimState(dt.Rows[0]);
            //}

            return currState;
        }

        public static ClaimState GetPrev(int idClaimState, int claimId)
        {
            //TODO: Написать функцию выбора предыдущего статуса
            var currState = new ClaimState(idClaimState);

            //switch (currState.SysName.ToUpper())
            //{
            //    case "NEW":
            //        return new ClaimState("NEWADD");
            //    case "NEWADD":
            //        return new ClaimState("SET");
            //    case "SET":
            //        var wtSysName = new WorkType(new Claim(claimId).IdWorkType).SysName;
            //        switch (wtSysName)
            //        {
            //            case "ДНО":
            //            case "НПР":
            //            case "ТЭО":
            //            case "УТЗ":
            //                return new ClaimState("TECHWORK");
            //            case "РТО":
            //            case "МТС":
            //            case "УРМ":
            //            case "ЗРМ":
            //            case "МДО":
            //            case "ИПТ":
            //            case "РЗРД":
            //            case "ЗНЗЧ":
            //                return new ClaimState("TECHWORK");
            //        }

            //        break;
            //}


            //var st = new ClaimState();
            //SqlParameter pIdClaim = new SqlParameter() { ParameterName = "id_claim_state", SqlValue = idClaimState, SqlDbType = SqlDbType.Int };
            //var dt = Db.Service.ExecuteQueryStoredProcedure("get_next_claim_state", pIdClaim);
            //if (dt.Rows.Count > 0)
            //{
            //    st = new ClaimState(dt.Rows[0]);
            //}

            return currState;
        }
    }
    
}