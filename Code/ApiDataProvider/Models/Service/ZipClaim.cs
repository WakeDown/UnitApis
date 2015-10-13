using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Objects;

namespace DataProvider.Models.Service
{
    public class ZipClaim:DbModel
    {
        public int Id { get; set; }
        public DateTime DateCreate { get; set; }

        public ZipClaim(DataRow row)
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            DateCreate = Db.DbHelper.GetValueDateTimeOrDefault(row, "date_create");
        }
    }
}