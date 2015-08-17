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
    public class Document : DbModel
    {
        public string Sid { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Data { get; set; }


        public Document() { }

        public Document(DataRow row)
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            Sid = Db.DbHelper.GetValueString(row, "data_sid");
            Id = Db.DbHelper.GetValueIntOrDefault(row, "id");
            Name = Db.DbHelper.GetValueString(row, "name");
            Data = Db.DbHelper.GetByteArr(row, "data");
        }

        //public Document(int? idDepartment = null, int? idPosition = null, int? idEmployee = null)
        //{
        //    SqlParameter pIdDepartment = new SqlParameter() { ParameterName = "id_department", SqlValue = idDepartment, SqlDbType = SqlDbType.Int };
        //    SqlParameter pIdPosition = new SqlParameter() { ParameterName = "id_position", SqlValue = idPosition, SqlDbType = SqlDbType.Int };
        //    SqlParameter pIdEmployee = new SqlParameter() { ParameterName = "id_employee", SqlValue = idEmployee, SqlDbType = SqlDbType.Int };
        //    var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_document_list", pIdDepartment, pIdPosition, pIdEmployee);
        //    if (dt.Rows.Count > 0)
        //    {
        //        var row = dt.Rows[0];
        //        FillSelf(row);
        //    }
        //}

        public void Save()
        {
            SqlParameter pData = new SqlParameter() { ParameterName = "data", SqlValue = Data, SqlDbType = SqlDbType.VarBinary };
            SqlParameter pName = new SqlParameter() { ParameterName = "name", SqlValue = Name, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "creator_sid", SqlValue = CurUserAdSid, SqlDbType = SqlDbType.VarChar };

            var dt = Db.Stuff.ExecuteQueryStoredProcedure("save_document", pData, pName, pCreatorAdSid);
            int id = 0;
            if (dt.Rows.Count > 0)
            {
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                Id = id;
            }
        }

        public static IEnumerable<Document> GetList(int? idDepartment = null, int? idPosition = null, int? idEmployee = null)
        {
            //SqlParameter pIdDocument = new SqlParameter() { ParameterName = "id_document", SqlValue = idDocument, SqlDbType = SqlDbType.Int };
            SqlParameter pIdDepartment = new SqlParameter() { ParameterName = "id_department", SqlValue = idDepartment, SqlDbType = SqlDbType.Int };
            SqlParameter pIdPosition = new SqlParameter() { ParameterName = "id_position", SqlValue = idPosition, SqlDbType = SqlDbType.Int };
            SqlParameter pIdEmployee = new SqlParameter() { ParameterName = "id_employee", SqlValue = idEmployee, SqlDbType = SqlDbType.Int };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_document_list", pIdDepartment, pIdPosition, pIdEmployee);

            var lst = new List<Document>();

            foreach (DataRow row in dt.Rows)
            {
                var dep = new Document(row);
                lst.Add(dep);
            }

            return lst;
        }

        public static byte[] GetData(string sid)
        {
            SqlParameter pSid = new SqlParameter() { ParameterName = "sid", SqlValue = sid, SqlDbType = SqlDbType.VarChar };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_document_data", pSid);
            byte[] result = null;

            if (dt.Rows.Count > 0)
            {
                result = dt.Rows[0]["data"] != DBNull.Value ? (byte[])dt.Rows[0]["data"] : null;
            }

            return result;
        }

        public static void Close(int id, string deleterSid)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            SqlParameter pDeleterSid = new SqlParameter() { ParameterName = "deleter_sid", SqlValue = deleterSid, SqlDbType = SqlDbType.VarChar };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("close_document", pId, pDeleterSid);
        }
    }
}