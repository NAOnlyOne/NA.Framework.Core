using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace NA.Framework.Core.Tools
{
    public partial class NPOIHelper
    {
        public static Stream GetExcelStream(List<DataTable> dts, List<string> sheetNames, EExcelType type)
        {
            if (dts == null || dts.Count <= 0)
                return null;
            sheetNames = sheetNames ?? new List<string>();

            IWorkbook workbook = CreateWorkBook(dts, sheetNames, type);
            Stream outputStream = new MemoryStream();
            workbook.Write(outputStream);
            return outputStream;
        }
    }

    public partial class NPOIHelper
    {
        private static readonly Type[] _strTypes = new Type[] { typeof(string), typeof(char), typeof(DateTime), typeof(DateTime?) };
        private static readonly Type[] _numTypes = new Type[] {typeof(int),typeof(long),typeof(short),
                typeof(double),typeof(float),typeof(decimal),
                typeof(uint),typeof(ulong),typeof(ushort),
                typeof(int?),typeof(long?),typeof(short?),
                typeof(double?),typeof(float?),typeof(decimal?),
                typeof(uint?),typeof(ulong?),typeof(ushort?)};
        private static readonly Type[] _boolTypes = new Type[] { typeof(bool), typeof(bool?) };

        private static CellType TypeConverter(Type type)
        {
            if (_strTypes.Contains(type))
                return CellType.String;
            else if (_numTypes.Contains(type))
                return CellType.Numeric;
            else if (_boolTypes.Contains(type))
                return CellType.Boolean;
            else
                return CellType.Unknown;
        }

        private static IWorkbook CreateWorkBook(List<DataTable> dts, List<string> sheetNames, EExcelType type)
        {
            int deviation = dts.Count - sheetNames.Count;
            if (deviation > 0)
            {
                for (int i = 0; i < deviation; i++)
                {
                    sheetNames.Add($"未命名页{i + 1}");
                }
            }

            IWorkbook workbook = null;
            switch (type)
            {
                case EExcelType.xls:
                    workbook = new HSSFWorkbook();
                    break;
                case EExcelType.xlsx:
                    workbook = new XSSFWorkbook();
                    break;
                default:
                    break;
            }

            try
            {
                for (int i = 0; i < dts.Count; i++)
                {
                    //生成页，并设置页名
                    ISheet sheet = workbook.CreateSheet(sheetNames[i]);

                    //设置列名
                    IRow titleRow = sheet.CreateRow(0);
                    for (int j = 0; j < dts[i].Columns.Count; j++)
                    {
                        titleRow.CreateCell(j, CellType.String).SetCellValue(dts[i].Columns[j]?.ColumnName ?? "未命名");
                        sheet.AutoSizeColumn(j);
                    }

                    //录入数据
                    for (int j = 0; j < dts[i].Rows.Count; j++)
                    {
                        IRow row = sheet.CreateRow(j + 1);
                        for (int k = 0; k < dts[i].Columns.Count; k++)
                        {
                            row.CreateCell(k, TypeConverter(dts[i].Columns[k].DataType)).SetCellValue(dts[i].Rows[j][k]?.ToString());
                        }
                    }
                }
                return workbook;
            }
            catch (Exception ep)
            {
                throw new Exception("Excel生成失败，详见内部异常", ep);
            }
        }
    }
}
