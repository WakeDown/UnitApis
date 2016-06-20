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
    

    public class DocMeetLink:DbModel
    {
        public int Id { get; set; }
        public int IdDocument { get; set; }
        public int IdDepartment { get; set; }
        public int IdPosition { get; set; }
        public int IdEmployee { get; set; }



        public DocMeetLink() { }

        //public DocMeetLink(DataRow row)
        //{
        //    FillSelf(row);
        //}

        //private void FillSelf(DataRow row)
        //{
        //    Id = Db.DbHelper.GetValueInt(row["id"]);
        //    IdDocument = Db.DbHelper.GetValueInt(row["id_document"]);
        //    //IdPosition = Db.DbHelper.GetValueInt(row["id_position"]);
        //}

        public DocMeetLink(int id)
        {
            //SqlParameter pIdDepartment = new SqlParameter() { ParameterName = "id_department", SqlValue = idDepartment, SqlDbType = SqlDbType.Int };
            //var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_document_list", pIdDepartment, pIdPosition, pIdEmployee);
            //if (dt.Rows.Count > 0)
            //{
            //    var row = dt.Rows[0];
            //    FillSelf(row);
            //}
        }

        public void Save()
        {
            using (var conn = Db.Stuff.connection)
            {
                conn.Open();
                    SqlParameter pIdDocument = new SqlParameter(){ParameterName = "id_document",SqlValue = IdDocument,SqlDbType = SqlDbType.Int};
                    SqlParameter pIdDepartment = new SqlParameter() { ParameterName = "id_department", SqlValue = IdDepartment, SqlDbType = SqlDbType.Int };
                    SqlParameter pIdPosition = new SqlParameter() { ParameterName = "id_position", SqlValue = IdPosition, SqlDbType = SqlDbType.Int };
                    SqlParameter pIdEmployee = new SqlParameter() { ParameterName = "id_employee", SqlValue = IdEmployee, SqlDbType = SqlDbType.Int };
                    SqlParameter pCreatorAdSid = new SqlParameter(){ParameterName = "creator_sid",SqlValue = CurUserAdSid,SqlDbType = SqlDbType.VarChar};

                    var dt = Db.Stuff.ExecuteQueryStoredProcedure("save_doc_meet_link", pIdDocument, pIdDepartment, pIdPosition,pIdEmployee, pCreatorAdSid);
            }
        }

        public static void Close(int idDocument, string deleterSid, int? idDepartment, int? idPosition, int? idEmployee)
        {
            SqlParameter pIdDocument = new SqlParameter() { ParameterName = "id_document", SqlValue = idDocument, SqlDbType = SqlDbType.Int };
            SqlParameter pIdDepartment = new SqlParameter() { ParameterName = "id_department", SqlValue = idDepartment, SqlDbType = SqlDbType.Int };
            SqlParameter pIdPosition = new SqlParameter() { ParameterName = "id_position", SqlValue = idPosition, SqlDbType = SqlDbType.Int };
            SqlParameter pIdEmployee = new SqlParameter() { ParameterName = "id_employee", SqlValue = idEmployee, SqlDbType = SqlDbType.Int };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "deleter_sid", SqlValue = deleterSid, SqlDbType = SqlDbType.VarChar };
            Db.Stuff.ExecuteStoredProcedure("close_doc_meet_link", pIdDocument, pIdDepartment, pIdPosition, pIdEmployee, pCreatorAdSid);
        }

        public static void Close(int id, string deleterSid)
        {
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = id, SqlDbType = SqlDbType.Int };
            SqlParameter pCreatorAdSid = new SqlParameter() { ParameterName = "deleter_sid", SqlValue = deleterSid, SqlDbType = SqlDbType.VarChar };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("close_doc_meet_link", pId, pCreatorAdSid);
        }
    }

   
}