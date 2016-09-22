using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DataProvider.Helpers;

namespace DataProvider.Models.Eprice
{
    public class CatalogProduct
    {
        public int Sid { get; set; }
        public string Id { get; set; }
        public string PartNumber { get; set; }
        public string Name { get; set; }
        public string Vendor { get; set; }
        public Currency Currency { get; set; }
        public int CategorySid { get; set; }
        public decimal Price { get; set; }

        public void Save()
        {
            if (Currency == null) Currency = new Currency();
            if (Currency.ProviderName.ToUpper().Equals("RUB"))
            {
                Currency.Id = 1;}
            else if (Currency.ProviderName.ToUpper().Equals("USD"))
            {
                Currency.Id = 2;}
            else if (Currency.ProviderName.ToUpper().Equals("EUR"))
            {
                Currency.Id = 3;
            }
            else
            {
                Currency.Id = 0;
            }
            Name = Name.Replace("\"", "");
            
            SqlParameter pSid = new SqlParameter() { ParameterName = "sid", SqlValue = Sid, SqlDbType = SqlDbType.BigInt };
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCategorySid = new SqlParameter() { ParameterName = "sid_cat", SqlValue = CategorySid, SqlDbType = SqlDbType.BigInt };
            SqlParameter pPrice = new SqlParameter() { ParameterName = "price", SqlValue = Price, SqlDbType = SqlDbType.Decimal };
            SqlParameter pCurrencyId = new SqlParameter() { ParameterName = "id_currency", SqlValue = Currency.Id, SqlDbType = SqlDbType.Int };
            SqlParameter pPartNumber = new SqlParameter() { ParameterName = "part_number", SqlValue = PartNumber, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pName = new SqlParameter() { ParameterName = "name", SqlValue = Name, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pVendor = new SqlParameter() { ParameterName = "vendor", SqlValue = Vendor, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCurrencyProviderName = new SqlParameter() { ParameterName = "currency_str", SqlValue = Currency.ProviderName, SqlDbType = SqlDbType.NVarChar };
            
            var dt = Db.Eprice.ExecuteQueryStoredProcedure("save_catalog_product", pId, pPartNumber, pName, pCategorySid, pVendor, pCurrencyId, pCurrencyProviderName, pPrice, pSid);
            
            if (dt.Rows.Count > 0)
            {
                int sid =Db.DbHelper.GetValueInt(dt.Rows[0]["sid"]);
                Sid = sid;
            }
        }

        public static PriceResult GetMinPrice(string partNum)
        {
            if (String.IsNullOrEmpty(partNum)) throw new ArgumentException("Партномер не указан");
            SqlParameter pPartNum = new SqlParameter() { ParameterName = "part_num", SqlValue = partNum, SqlDbType = SqlDbType.NVarChar };

            var dt = Db.Eprice.ExecuteQueryStoredProcedure("get_catalog_product", pPartNum);

            if (dt.Rows.Count > 0)
            {
                return new PriceResult(dt.Rows[0]);    
            }
            return new PriceResult();
        }

        public static PriceResult GetProductActualInfo(string partNum)
        {
            if (String.IsNullOrEmpty(partNum)) throw new ArgumentException("Партномер не указан");
            SqlParameter pPartNum = new SqlParameter() { ParameterName = "part_num", SqlValue = partNum, SqlDbType = SqlDbType.NVarChar };

            var dt = Db.Eprice.ExecuteQueryStoredProcedure("get_catalog_product", pPartNum);

            if (dt.Rows.Count > 0)
            {
                return new PriceResult(dt.Rows[0]);
            }
            return new PriceResult();
        }
    }
}