using System;
using System.Collections.Generic;
using System.Data;
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
            ClaimCount = Db.DbHelper.GetValueIntOrDefault(row, "cnt");
        }

        public static IEnumerable<ClaimStateGroup> GetFilterList()
        {
            //SqlParameter pSome = new SqlParameter() { ParameterName = "some", SqlValue = some, SqlDbType = SqlDbType.NVarChar };
            var dt = Db.Service.ExecuteQueryStoredProcedure("claim_claim_state_group_count");

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