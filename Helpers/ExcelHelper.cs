using API.Helpers.Attributes;
using ClosedXML.Excel;
using System.Data;

namespace API.Helpers;

public static class ExcelHelper
{
    public static XLWorkbook GetExcel<T>(IEnumerable<T> data , string sheetName = "Default")
    {
        var dataTable = new DataTable();

        var properties = typeof(T)
            .GetProperties()
            .Where(p => 
                p.GetCustomAttributes(typeof(IgnoreInExcelAttribute), false).Length == 0)
            .ToArray();

        foreach (var property in properties)
        {
            dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
        }

        foreach (var myObject in data)
        {
            var row = dataTable.NewRow();
            foreach (var property in properties)
            {
                row[property.Name] = property.GetValue(myObject) ?? DBNull.Value;
            }
            dataTable.Rows.Add(row);
        }

        XLWorkbook wb = new();
        
        var worksheet = wb.Worksheets.Add(sheetName);
        worksheet.Cell(1, 1).InsertTable(dataTable);

        return wb;
    }
}
