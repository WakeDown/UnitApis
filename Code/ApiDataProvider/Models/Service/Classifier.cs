using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClosedXML.Excel;

namespace DataProvider.Models.Service
{
    public class Classifier
    {
        public int Id { get; set; }
        public ClassifierCaterory Category { get; set; }
        //public WorkType{get;set;}

        public static void SaveFromExcel(XLWorkbook wb, string creatorSid)
        {
            var ws = wb.Worksheet(1);

            int r = 0;
            foreach (var row in ws.Rows())
            {
                r++;
                if (r == 1) continue;
                int c = 1;
                if (String.IsNullOrEmpty(row.Cell(c).Value.ToString())) break;
                string catName = row.Cell(c).Value.ToString();
                c++;
                string catNumber = row.Cell(c).Value.ToString();
                c++;
                int catComplexity;
                int.TryParse(row.Cell(c).Value.ToString(), out catComplexity);

                var cat =new ClassifierCaterory() {Name = catName, Number = catNumber, Complexity = catComplexity, CurUserAdSid = creatorSid };
                cat.Save();
            }
        }
    }
}