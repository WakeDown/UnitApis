using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Xml.Linq;
using DataProvider.Helpers;
using DataProvider.Models.SpeCalc;
using DataProvider.Objects;

namespace DataProvider.Models.Eprice
{
    public class CatalogCategory:DbModel
    {
        public int Sid { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string IdParent { get; set; }
        public ProductProvider Provider { get; set; }

        public List<CatalogProduct> Products { get; set; }

        public CatalogCategory()
        {
            Products = new List<CatalogProduct>();
        }

        //public CatalogCategory(DataRow row)
        //{
        //    FillSelf(row);
        //}

        //private void FillSelf(DataRow row)
        //{
        //    //Id = Db.DbHelper.GetValueInt(row["id"]);
        //    //Manager = new Employee(row["manager_sid"].ToString());
        //    //DateLimit = Db.DbHelper.GetValueDateTime(row["date_limit"]);
        //    //Descr = row["descr"].ToString();
        //    //DateCreate = Db.DbHelper.GetValueDateTime(row["dattim1"]);
        //    //State = new QueState() { Id = Db.DbHelper.GetValueInt(row["id_que_state"]), Name = row["que_state"].ToString() };
        //}

        public void Save()
        {
            Name = Name.Replace("\"", "");

            SqlParameter pSid = new SqlParameter() { ParameterName = "sid", SqlValue = Sid, SqlDbType = SqlDbType.BigInt };
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pIdParent = new SqlParameter() { ParameterName = "id_parent", SqlValue = IdParent, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pName = new SqlParameter() { ParameterName = "name", SqlValue = Name, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pProviderId = new SqlParameter() { ParameterName = "id_provider", SqlValue = Provider.Id, SqlDbType = SqlDbType.Int };

            var dt = Db.Eprice.ExecuteQueryStoredProcedure("save_catalog_category", pId, pIdParent, pName, pProviderId, pSid);
            if (dt.Rows.Count > 0)
            {
                int sid = Db.DbHelper.GetValueInt(dt.Rows[0]["sid"]);
                Sid = sid;
            }

            if (Products != null && Products.Any())
            {
                foreach (CatalogProduct prod in Products)
                {
                    prod.CategorySid = Sid;
                    try
                    {
                        prod.Save();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }
    }
}