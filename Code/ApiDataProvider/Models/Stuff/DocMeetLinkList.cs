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
    public class DocMeetLinkList:DbModel
    {
        public int IdDocument { get; set; }
        public IEnumerable<int> IdDepartments { get; set; }
        public IEnumerable<int> IdPositions { get; set; }
        public IEnumerable<int> IdEmployees { get; set; }

        public static DocMeetLinkList GetList(int idDocument)
        {
            SqlParameter pIdDocument = new SqlParameter() { ParameterName = "id_document", SqlValue = idDocument, SqlDbType = SqlDbType.Int };
            var dt = Db.Stuff.ExecuteQueryStoredProcedure("get_doc_meet_link_list", pIdDocument);

            var deps = new List<int>();
            var poss = new List<int>();
            var emps = new List<int>();

            foreach (DataRow row in dt.Rows)
            {
                int? idDep = Db.DbHelper.GetValueIntOrNull(row["id_department"]);
                int? idPos = Db.DbHelper.GetValueIntOrNull(row["id_position"]);
                int? idEmp = Db.DbHelper.GetValueIntOrNull(row["id_employee"]);

                if (idDep.HasValue && idDep > 0)
                {
                    deps.Add(idDep.Value);
                }
                if (idPos.HasValue && idPos >0)
                {
                    poss.Add(idPos.Value);
                }
                if (idEmp.HasValue && idEmp>0)
                {
                    emps.Add(idEmp.Value);
                }
            }

            return new DocMeetLinkList() { IdDocument = idDocument, IdDepartments = deps, IdPositions = poss, IdEmployees = emps };
        }
    }
}