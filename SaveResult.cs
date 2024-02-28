using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace iTextSharpReadPDF
{
    class SaveResult
    {
        private string filePath;
        private List<Result> results;
        public SaveResult(string filePath)
        {
            this.filePath = filePath;
            this.results = new List<Result>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }
        public void saveResultAsExcel()
        {
            // 创建一个新的 Excel 文件
            FileInfo newFile = new FileInfo(this.filePath);

            // 如果文件已存在，删除文件
            if (newFile.Exists)
            {
                newFile.Delete();
                newFile = new FileInfo(this.filePath);
            }
            // 创建 ExcelPackage 对象
            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                // 添加工作表
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // 在工作表中填充数据（这里是一个简单的示例）
                worksheet.Cells["A1"].Value = "orginContent";
                worksheet.Cells["B1"].Value = "matchContent";
                worksheet.Cells["C1"].Value = "matchContentPercent";
                worksheet.Cells["D1"].Value = "matchPage";
                worksheet.Cells["E1"].Value = "matchRow";
                worksheet.Cells["F1"].Value = "matchTitle";
                worksheet.Cells["G1"].Value = "titleIsExist";
                foreach (var item in worksheet.Columns)
                {
                    item.Width = 26;
                }
                using (var range = worksheet.Cells["A1:G1"])
                {
                    range.Style.Font.Bold = true; // 设置加粗
                    range.Style.Font.Size = 16; // 设置字体大小
                    // 设置背景颜色
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.GreenYellow);
                }
                Comparison<Result> comparison = (x, y) =>
                {
                    return (int)(y.matchContentPercent - x.matchContentPercent);
                };
                results.Sort(comparison);
                for (int i = 0; i < results.Count; i++)
                {
                    Result result = results[i];
                    worksheet.Cells[i + 2, 1].Value = result.orginContent;
                    worksheet.Cells[i + 2, 2].Value = result.matchContent;
                    worksheet.Cells[i + 2, 3].Value = $"{result.matchContentPercent}%";
                    worksheet.Cells[i + 2, 4].Value = result.matchPage;
                    worksheet.Cells[i + 2, 5].Value = result.matchRow;
                    worksheet.Cells[i + 2, 6].Value = result.matchTitle;
                    worksheet.Cells[i + 2, 7].Value = result.titleExist;
                }
                // 保存 Excel 文件
                package.Save();
            }

            Console.WriteLine("Excel 文件已创建并保存成功！");
        }
        public void addResult(Result r)
        {
            this.results.Add(r);
            //results.
        }
    }
    class Result : IComparer<Result>
    {
        public string orginContent;
        public string matchContent;
        public double matchContentPercent;
        public int matchPage;
        public int matchRow;
        public string matchTitle;
        public bool titleExist;

        public Result()
        {
        }

        public Result(string orginContent, string matchContent, double matchContentPercent, int matchPage, int matchRow, string matchTitle, bool titleExist)
        {
            this.orginContent = orginContent;
            this.matchContent = matchContent;
            this.matchContentPercent = matchContentPercent;
            this.matchPage = matchPage;
            this.matchRow = matchRow;
            this.matchTitle = matchTitle;
            this.titleExist = titleExist;
        }

        public int Compare(Result x, Result y)
        {
            return y.matchContentPercent.CompareTo(x.matchContentPercent);
        }

        public override string ToString()
        {
            return $"orginContent:{orginContent}--matchContent:{matchContent}--" +
                $"matchContentPercent:{matchContentPercent}--matchPage:{matchPage}--" +
                $"matchRow:{matchRow}--matchTitle:{matchTitle}--titleExist:{titleExist}";
        }
    }
}
