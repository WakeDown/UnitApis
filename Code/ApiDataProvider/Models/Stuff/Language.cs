using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;
using DataProvider.Objects;

namespace DataProvider.Models.Stuff
{
    public class Language : DbModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Language()
        {
        }

        public Language(DataRow row)
        {
            FillSelf(row);
        }


        private void FillSelf(DataRow row)
        {
            Id = Db.DbHelper.GetValueInt(row["id"]);
            Name = row["name"].ToString();
        }

        public static IEnumerable<Language> GetList()
        {
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_language_list");
            var lst = new List<Language>();
            foreach (DataRow row in dt.Rows)
            {
                var language = new Language(row);
                lst.Add(language);
            }
            return lst;
        }

        
    }
}