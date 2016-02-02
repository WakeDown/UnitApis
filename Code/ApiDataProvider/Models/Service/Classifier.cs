using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using ClosedXML.Excel;
using DataProvider.Helpers;
using DataProvider.Objects;

namespace DataProvider.Models.Service
{
    public class Classifier : DbModel
    {
        public static IEnumerable<ClassifierItem> GetList()
        {
            var dt = Db.Service.ExecuteQueryStoredProcedure("get_classifier_list");

            var list = new List<ClassifierItem>();

            foreach (DataRow row in dt.Rows)
            {
                var model = new ClassifierItem(row);
                list.Add(model);
            }

            return list;
        }
        
        public static byte[] GetExcel()
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Classifier");

            var catList = ClassifierCaterory.GetLowerList();
            var clsfr = Classifier.GetList();
            var wtList = WorkType.GetList();

            int c = 1;
            int r = 2;//Оставляем 1 строку для надзаголовков
            int firstCol = c;
            int firstRow = r - 1;

            //Header
            var rng = ws.Range(ws.Cell(r - 1, c), ws.Cell(r, c));
            rng.Merge();
            rng.SetValue("Подтип");
            rng.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            c++;
            rng = ws.Range(ws.Cell(r - 1, c), ws.Cell(r, c));
            rng.Merge();
            rng.SetValue("№ подтипа");
            rng.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            c++;
            rng = ws.Range(ws.Cell(r - 1, c), ws.Cell(r, c));
            rng.Merge();
            rng.SetValue("Сложность");
            rng.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.SheetView.Freeze(r, c);

            c++;
            rng = ws.Range(ws.Cell(r - 1, c), ws.Cell(r - 1, c + wtList.Count() - 1));
            rng.Merge();
            rng.SetValue("Временной норматив");
            rng.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            foreach (WorkType wt in wtList)
            {
                ws.Cell(r, c).SetValue(wt.SysName).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                c++;
            }

            rng = ws.Range(ws.Cell(r - 1, c), ws.Cell(r - 1, c + wtList.Count() - 1));
            rng.Merge();
            rng.SetValue("Прайс лист");
            rng.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            foreach (WorkType wt in wtList)
            {
                ws.Cell(r, c).SetValue(wt.SysName).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                c++;
            }

            

            var headerRange = ws.Range(ws.Cell(r - 1, firstCol), ws.Cell(r, c));
            headerRange.Style.Font.SetBold();
            //headerRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //headerRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            //>Header
            foreach (var cat in catList)
            {
                c = firstCol;
                r++;
                ws.Cell(r, c).SetValue($"{cat.Number} {cat.Name}");
                c++;
                var cell = ws.Cell(r, c);
                cell.SetValue(cat.Number);
                cell.DataType = XLCellValues.Text;
                c++;
                ws.Cell(r, c).SetValue(cat.Complexity);
                foreach (WorkType wt in wtList)
                {
                    c++;
                    cell = ws.Cell(r, c);
                        cell.SetValue(clsfr.Any(x => x.IdCategory == cat.Id && x.IdWorkType == wt.Id) ? clsfr.First(x => x.IdCategory == cat.Id && x.IdWorkType == wt.Id).Time.Value.ToString() : null);
                    cell.DataType=XLCellValues.Number;
                    cell.Style.NumberFormat.Format = "# ##0";
                }
                foreach (WorkType wt in wtList)
                {
                    c++;
                    cell = ws.Cell(r, c);
                        cell.SetValue(clsfr.Any(x => x.IdCategory == cat.Id && x.IdWorkType == wt.Id) ? clsfr.First(x => x.IdCategory == cat.Id && x.IdWorkType == wt.Id).Price.Value.ToString() : null);
                    cell.DataType = XLCellValues.Number;
                    cell.Style.NumberFormat.Format = "# ##0.00";
                }
            }
            //ws.Rows().AdjustToContents();
            ws.Column(firstCol).AdjustToContents();
            ws.Column(firstCol+1).Width = 11;
            ws.Column(firstCol + 2).Width = 11;

            rng = ws.Range(ws.Cell(firstRow, firstCol), ws.Cell(r, c));
            rng.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            rng.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            rng.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            rng.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            byte[] data = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                wb.SaveAs(stream);
                data = stream.ToArray();
            }
            return data;
        }

        public static void SaveFromExcel(XLWorkbook wb, string creatorSid)
        {
            var attrs = ClassifierAttributes.Get();
            int wtCnt = WorkType.GetList().Count();
            var ws = wb.Worksheet(1);

            var workTypes = WorkType.GetList();

            
            int r = 0;
            int headerRow = r+2;
            foreach (var row in ws.Rows())
            {
                r++;
                if (r <= headerRow) continue;
                int c = 1;
                if (String.IsNullOrEmpty(row.Cell(c).Value.ToString())) break;
                string catName = row.Cell(c).Value.ToString();
                c++;
                string catNumber = row.Cell(c).Value.ToString();
                c++;
                int catComplexity;
                int.TryParse(row.Cell(c).Value.ToString(), out catComplexity);

                var cat = new ClassifierCaterory() { Name = catName, Number = catNumber, Complexity = catComplexity, CurUserAdSid = creatorSid };
                cat.Save();

                int colOffset = c;
                c++;
                while (!String.IsNullOrEmpty(ws.Row(headerRow).Cell(c).Value.ToString()))
                {
                    int idCategory = cat.Id;
                    var headerVal = ws.Row(headerRow).Cell(c).Value;
                    string wtValue = headerVal.ToString().Trim();
                    int pointIndex = wtValue.IndexOf(".", StringComparison.Ordinal);
                    string currWorkTypeSysName = wtValue;
                    string wtType = String.Empty;

                    if (pointIndex > -1)
                    {
                        currWorkTypeSysName = wtValue.Remove(pointIndex);
                        wtType = wtValue.Substring(pointIndex);
                    }

                    int idWorkType = workTypes.Any(x => x.SysName == currWorkTypeSysName)
                        ? workTypes.First(x => x.SysName == currWorkTypeSysName).Id
                        : 0;

                    var clItem = new ClassifierItem() { IdCategory = idCategory, IdWorkType = idWorkType, CurUserAdSid = creatorSid };

                    //string cellAddress = row.Cell(c).Address.ToStringRelative();
                    //var cVal = ws.Cell(cellAddress).Value;

                    if (currWorkTypeSysName.Equals("НПР"))
                    {
                        string s = "";
                    }

                    if (String.IsNullOrEmpty(wtType))
                    {
                        int flag = (c - colOffset - 1) / wtCnt;

                        switch (flag)
                        {
                            //первая шеренга типов работ - это время
                            case 0:
                                wtType = ".t";
                                break;
                            //ыторая шеренга типов работ - это прайс
                            case 1:
                                wtType = ".wi";
                                break;
                        }
                    }

                    if (!String.IsNullOrEmpty(wtType) && (wtType.Equals(".t") || wtType.Equals(".wi")))
                    {
                        string cellValue = row.Cell(c).Value.ToString();
                        switch (wtType)
                        {
                            case ".t":
                                int t;
                                int.TryParse(cellValue, out t);
                                clItem.Time = t;
                                break;
                            case ".wi":
                                decimal wi;
                                decimal.TryParse(cellValue, out wi);
                                clItem.Price = wi;
                                clItem.CostPeople = clItem.Price + attrs.Wage;
                                clItem.CostCompany = clItem.Price + attrs.Overhead;
                                break;
                                //case ".f":
                                //    decimal f;
                                //    decimal.TryParse(cellValue, out f);
                                //    clItem.Price = f;
                                //    break;
                                //case ".wc":
                                //    decimal wc;
                                //    decimal.TryParse(cellValue, out wc);
                                //    clItem.Price = wc;
                                //    break;
                        }
                        clItem.Save();
                    }
                    c++;
                }

            }
        }
    }
}