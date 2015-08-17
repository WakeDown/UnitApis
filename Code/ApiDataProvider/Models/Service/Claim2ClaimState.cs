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
    public class Claim2ClaimState : DbModel
    {
        public int Id { get; set; }
        public int IdClaim { get; set; }
        public int IdClaimState { get; set; }
        public string Descr { get; set; }
        public EmployeeSm Creator { get; set; }
        public DateTime DateCreate { get; set; }
        public ClaimState State { get; set; }


        public Claim2ClaimState() { }

        public Claim2ClaimState(int id)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_model", pId);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                FillSelf(row);
            }
        }

        public Claim2ClaimState(DataRow row)
            : this()
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            IdClaim = Db.DbHelper.GetValueIntOrDefault(row, "id_claim");
            Descr = Db.DbHelper.GetValueString(row, "descr");
            Creator = new EmployeeSm(Db.DbHelper.GetValueString(row, "creator_sid"));
            DateCreate = Db.DbHelper.GetValueDateTimeOrDefault(row, "dattim1");
            IdClaimState = Db.DbHelper.GetValueIntOrDefault(row, "id_claim_state");
            State = new ClaimState(row);
        }

        public void Save()
        {
            SqlParameter pIdClaim = new SqlParameter() { ParameterName = "id_claim", SqlValue = IdClaim, SqlDbType = SqlDbType.Int };
            SqlParameter pIdClaimState = new SqlParameter() { ParameterName = "id_claim_state", SqlValue = IdClaimState, SqlDbType = SqlDbType.Int };
            SqlParameter pDescr = new SqlParameter() { ParameterName = "descr", SqlValue = Descr, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };

            using (var conn = Db.Service.connection)
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        var dt = Db.Service.ExecuteQueryStoredProcedure("save_claim2claim_state", conn, tran, pIdClaim, pIdClaimState,
                pDescr, pCreatorAdSid);
                        int id = 0;
                        if (dt.Rows.Count > 0)
                        {
                            int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                            Id = id;
                            SqlParameter pIdClaim2 = new SqlParameter() { ParameterName = "id_claim", SqlValue = IdClaim, SqlDbType = SqlDbType.Int };
                            SqlParameter pIdClaimState2 = new SqlParameter() { ParameterName = "id_claim_state", SqlValue = IdClaimState, SqlDbType = SqlDbType.Int };
                            Db.Stuff.ExecuteQueryStoredProcedure("set_claim_current_state", conn, tran, pIdClaim2, pIdClaimState2);
                        }
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public static IEnumerable<Claim2ClaimState> GetList(int idClaim)
        {
            SqlParameter pIdClaim = new SqlParameter() { ParameterName = "id_claim", SqlValue = idClaim, SqlDbType = SqlDbType.Int };
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_claim2claim_state_list", pIdClaim);

            var lst = new List<Claim2ClaimState>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new Claim2ClaimState(row);
                lst.Add(model);
            }

            return lst;
        }

        public static void Close(int id, string deleterSid)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            SqlParameter pDeleterSid = new SqlParameter() { ParameterName = "deleter_sid", SqlValue = deleterSid, SqlDbType = SqlDbType.VarChar };
            var dt = Db.Service.ExecuteQueryStoredProcedure("close_model", pId, pDeleterSid);
        }
    }
}