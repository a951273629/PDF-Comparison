using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
namespace iTextSharpReadPDF
{
    class ExcelRead
    {
        Dictionary<string, RowInfo> excelInfo = new Dictionary<string, RowInfo>();
        private string filePath;
        public ExcelRead(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            this.filePath = filePath;
        }

        public Dictionary<string, RowInfo> readExcel()
        {
            using (var p = new ExcelPackage(this.filePath))
            {
                int key = 8;
                int value = 4;
                ExcelWorksheets worksheets = p.Workbook.Worksheets;
                //只拿第一个表
                var excelCell = worksheets.First();
                for (int i = 1; i < excelCell.Dimension.Rows; i++)
                {
                    //去除空行
                    if (excelCell.Cells[i, key].Value == null || excelCell.Cells[i, key].Value.ToString() == "") continue;
                    if (excelCell.Cells[i, value].Value == null || excelCell.Cells[i, value].Value.Equals("")) continue;

                    string rowKey = excelCell.Cells[i, key].Value.ToString();
                    string rowValue = excelCell.Cells[i, value].Value.ToString();

                    excelInfo[rowKey] = new RowInfo(rowValue, i);
                }

                //Console.WriteLine(excelCell.Dimension.Columns);

            }
            //Console.WriteLine(excelInfo.Count);
            return excelInfo;
        }
    }
    class RowInfo
    {
        public RowInfo()
        {
        }

        public RowInfo(string value, int row)
        {
            this.value = value ?? throw new ArgumentNullException(nameof(value));
            this.row = row;
        }

        public string value { get; set; }
        public int row { get; set; }
    }
}
