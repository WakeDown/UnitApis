using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Models.Stuff;
using DataProvider.Objects;

namespace DataProvider.Models.SpeCalc
{
        public class Tender:DbModel
        {
            public Employee Manager { get; set; }
            public TenderState State { get; set; }
            public int PositionCount { get; set; }
            public int CalcCount { get; set; }

            public Tender() { }

            public Tender(DataRow row)
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Manager = new Employee() { AdSid = Db.DbHelper.GetValueString(row, "Manager") };
            State = new TenderState() { Id = Db.DbHelper.GetValueIntOrDefault(row, "id_state"), Name = Db.DbHelper.GetValueString(row, "state_name") };
            PositionCount = Db.DbHelper.GetValueIntOrDefault(row, "position_count"); //Db.DbHelper.GetValueInt(row["position_count"]);
            CalcCount = Db.DbHelper.GetValueIntOrDefault(row, "calc_count");
        }

        public static IEnumerable<Tender> GetManagerReport(DateTime dateStart, DateTime dateEnd)
            {
                SqlParameter pDateStart = new SqlParameter() { ParameterName = "date_start", SqlValue = dateStart, SqlDbType = SqlDbType.Date };
                SqlParameter pDateEnd = new SqlParameter() { ParameterName = "sid", SqlValue = dateEnd, SqlDbType = SqlDbType.Date };
                var dt = Db.SpeCalc.ExecuteQueryStoredProcedure("get_manager_position_report", pDateStart, pDateEnd);

                var lst = new List<Tender>();

                foreach (DataRow row in dt.Rows)
                {
                    var model = new Tender(row);
                    lst.Add(model);
                }

                return lst;
            }
        }
}