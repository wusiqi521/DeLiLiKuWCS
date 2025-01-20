using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BMHRI.WCS.Server.Tools
{
    public static class ExcelHelper
    {
        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="row"></param>
        /// <param name="evaluator"></param>
        /// <param name="dr"></param>
        private static void FillDataRowByRow(IRow row, IFormulaEvaluator evaluator, ref DataRow dr)
        {
            if (row != null)
            {
                for (int j = 0; j < dr.Table.Columns.Count; j++)
                {
                    ICell cell = row.GetCell(j);

                    if (cell != null)
                    {
                        switch (cell.CellType)
                        {
                            case CellType.Blank:
                                {
                                    dr[j] = DBNull.Value;
                                    break;
                                }
                            case CellType.Boolean:
                                {
                                    dr[j] = cell.BooleanCellValue;
                                    break;
                                }
                            case CellType.Numeric:
                                {
                                    if (DateUtil.IsCellDateFormatted(cell))
                                    {
                                        dr[j] = cell.DateCellValue;
                                    }
                                    else
                                    {
                                        dr[j] = cell.NumericCellValue;
                                    }
                                    break;
                                }
                            case CellType.String:
                                {
                                    dr[j] = cell.StringCellValue;
                                    break;
                                }
                            case CellType.Error:
                                {
                                    dr[j] = cell.ErrorCellValue;
                                    break;
                                }
                            case CellType.Formula:
                                {
                                    cell = evaluator.EvaluateInCell(cell) as HSSFCell;
                                    dr[j] = cell.ToString();
                                    break;
                                }
                            default:
                                throw new NotSupportedException(string.Format("Unsupported format type:{0}", cell.CellType));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取单元格
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private static int GetCellCount(ISheet sheet)
        {
            int firstRowNum = sheet.FirstRowNum;

            int cellCount = 0;

            for (int i = sheet.FirstRowNum; i <= sheet.LastRowNum; ++i)
            {
                IRow row = sheet.GetRow(i);

                if (row != null && row.LastCellNum > cellCount)
                {
                    cellCount = row.LastCellNum;
                }
            }
            return cellCount;
        }

        /// <summary>
        /// 数据导出
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sheetName"></param>
        public static void ExportToExcel(this DataTable data, string filePathName, string sheetName)
        {
            IWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(sheetName);
            IRow rowHead = sheet.CreateRow(0);


            //填写表头
            for (int i = 0; i < data.Columns.Count; i++)
            {
                rowHead.CreateCell(i, CellType.String).SetCellValue(data.Columns[i].ColumnName.ToString());
            }
            //填写内容
            for (int i = 0; i < data.Rows.Count; i++)
            {
                IRow row = sheet.CreateRow(i + 1);
                for (int j = 0; j < data.Columns.Count; j++)
                {
                    row.CreateCell(j, NPOI.SS.UserModel.CellType.String).SetCellValue(data.Rows[i][j].ToString());
                }
            }

            for (int i = 0; i < data.Columns.Count; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            using (FileStream stream = File.OpenWrite(filePathName))
            {
                workbook.Write(stream);
                stream.Close();
            }
            //MessageBox.Show("导出数据成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            GC.Collect();
        }
    }
}
