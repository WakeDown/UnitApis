using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using DataProvider.Helpers;

namespace DataProvider.Models.Eprice
{
    public class PriceResult
    {
        public decimal Price { get; set; }
        public Currency Currency { get; set; }
        //public CatalogCategory Category { get; set; }
        public ProductProvider Provider { get; set; }
        public string NomenclatureName { get; set; }

        public PriceResult()
        {
            Currency=new Currency();
            Provider = new ProductProvider();
        }

        public PriceResult(DataRow dr):this()
        {
            if (dr.Table.Columns.Contains("price")) Price = Db.DbHelper.GetValueDecimal(dr["price"]);
            if (dr.Table.Columns.Contains("currency_str")) Currency.Name = dr["currency_str"].ToString();
            if (dr.Table.Columns.Contains("name")) Provider.Name = dr["name"].ToString();
            if (dr.Table.Columns.Contains("nomenclature_name")) NomenclatureName = dr["nomenclature_name"].ToString();
        }

        public string GetStr()
        {
            string result = String.Empty;
            if (Price > 0)
            {
                result = String.Format("{0} - {1} {2}", Provider.Name, Price, Currency.Name);
            }
            else
            {
            }
            return result;
        }
    }
}