using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DataProvider.Helpers
{
    public partial class Db
    {
        public class Stuff
        {
            public static SqlConnection connection { get { return new SqlConnection(ConfigurationManager.ConnectionStrings["StuffConnectionString"].ConnectionString); } }

            public static void ExecuteStoredProcedure(string spName, params SqlParameter[] sqlParams)
            {
                DbHelper.ExecuteStoredProcedure(connection, spName, sqlParams);
            }

            public static DataTable ExecuteQueryStoredProcedure(string spName, params SqlParameter[] sqlParams)
            {
                DataTable dt = DbHelper.ExecuteQueryStoredProcedure(connection, spName, sqlParams);
                return dt;
            }

            public static DataTable ExecuteQueryStoredProcedure(string spName, SqlConnection conn, SqlTransaction tran, params SqlParameter[] sqlParams)
            {
                DataTable dt = DbHelper.ExecuteQueryStoredProcedure(conn, tran, spName, sqlParams);
                return dt;
            }

            public static void ExecuteStoredProcedure(string spName, SqlConnection conn, SqlTransaction tran, params SqlParameter[] sqlParams)
            {
                DbHelper.ExecuteStoredProcedure(conn, tran, spName, sqlParams);
            }

            public static object ExecuteScalar(string spName, params SqlParameter[] sqlParams)
            {
                object result = DbHelper.ExecuteScalar(connection, spName, sqlParams);
                return result;
            }
        }
    }
}