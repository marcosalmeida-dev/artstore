using ClosedXML.Excel;
using Microsoft.Extensions.Localization;

namespace ArtStore.Infrastructure.Services;

public class ExcelService : IExcelService
{
    private readonly IStringLocalizer<ExcelService> _localizer;

    public ExcelService(IStringLocalizer<ExcelService> localizer)
    {
        _localizer = localizer;
    }

    /// <summary>
    /// Applies the header cell style.
    /// </summary>
    /// <param name="cell">The cell to style.</param>
    private void ApplyHeaderStyle(IXLCell cell)
    {
        var style = cell.Style;
        style.Fill.PatternType = XLFillPatternValues.Solid;
        style.Fill.BackgroundColor = XLColor.LightBlue;
        style.Border.BottomBorder = XLBorderStyleValues.Thin;
    }

    /// <summary>
    /// Saves the given workbook to a byte array.
    /// </summary>
    /// <param name="workbook">The workbook to save.</param>
    /// <returns>A byte array representing the workbook.</returns>
    private static byte[] SaveWorkbookToByteArray(XLWorkbook workbook)
    {
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Seek(0, SeekOrigin.Begin);
        return stream.ToArray();
    }

    public Task<byte[]> CreateTemplateAsync(IEnumerable<string> fields, string sheetName = "Sheet1")
    {
        using var workbook = new XLWorkbook();
        workbook.Properties.Author = string.Empty;
        var ws = workbook.Worksheets.Add(sheetName);
        var rowIndex = 1;
        var colIndex = 1;
        foreach (var header in fields)
        {
            var cell = ws.Cell(rowIndex, colIndex++);
            ApplyHeaderStyle(cell);
            cell.Value = header;
        }

        return Task.FromResult(SaveWorkbookToByteArray(workbook));
    }

    public Task<byte[]> ExportAsync<TData>(IEnumerable<TData> data, Dictionary<string, Func<TData, object?>> mappers, string sheetName = "Sheet1")
    {
        using var workbook = new XLWorkbook();
        workbook.Properties.Author = string.Empty;
        var ws = workbook.Worksheets.Add(sheetName);
        var rowIndex = 1;
        var colIndex = 1;
        var headers = mappers.Keys.ToList();

        // Write header row
        foreach (var header in headers)
        {
            var cell = ws.Cell(rowIndex, colIndex++);
            ApplyHeaderStyle(cell);
            cell.Value = header;
        }

        // Write data rows
        var dataList = data.ToList();
        foreach (var item in dataList)
        {
            colIndex = 1;
            rowIndex++;

            foreach (var header in headers)
            {
                var value = mappers[header](item);
                // If the value is null, write a Blank.Value; otherwise, use its string representation.
                ws.Cell(rowIndex, colIndex++).Value = value == null ? Blank.Value : value.ToString();
            }
        }

        return Task.FromResult(SaveWorkbookToByteArray(workbook));
    }
}

