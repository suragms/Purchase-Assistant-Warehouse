using System.IO.Compression;
using System.Xml;
using System.Globalization;
using SkiaSharp;
namespace PurchaseAssistant.Infrastructure.Services;
public static class ExportFileBuilder
{
    public record CsvNumber(decimal Value, string Format = "0.####");
    public static byte[] Csv(string[] columns, IEnumerable<object?[]> rows, string? comment = null)
    {
        static string Cell(object? value)
        {
            var text = value switch { CsvNumber n => n.Value.ToString(n.Format, CultureInfo.InvariantCulture),
                decimal n => n.ToString("0.####", CultureInfo.InvariantCulture), _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? "" };
            if (value is string && text.TrimStart(' ', '\t', '\r', '\n') is var clean && clean.Length > 0 && "=+@-".Contains(clean[0])) text = "'" + text;
            return text.IndexOfAny([',', '"', '\r', '\n']) >= 0 ? "\"" + text.Replace("\"", "\"\"") + "\"" : text;
        }
        var csv = new System.Text.StringBuilder();
        if (comment != null) csv.Append(comment.Replace('\r', ' ').Replace('\n', ' ')).Append('\n');
        csv.AppendJoin(',', columns).Append('\n');
        foreach (var row in rows) { if (row.Length != columns.Length) throw new ArgumentException("CSV column count mismatch."); csv.AppendJoin(',', row.Select(Cell)).Append('\n'); }
        return new System.Text.UTF8Encoding(false).GetBytes(csv.ToString());
    }
    public static byte[] Spreadsheet(string[] columns, IEnumerable<object?[]> rows)
    {
        using var output = new MemoryStream();
        using (var zip = new ZipArchive(output, ZipArchiveMode.Create, true))
        {
            void Text(string name, string value) { using var w = new StreamWriter(zip.CreateEntry(name).Open()); w.Write(value); }
            Text("[Content_Types].xml", "<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\"><Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/><Default Extension=\"xml\" ContentType=\"application/xml\"/><Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/><Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/></Types>");
            Text("_rels/.rels", "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/></Relationships>");
            Text("xl/workbook.xml", "<workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\"><sheets><sheet name=\"Stock\" sheetId=\"1\" r:id=\"rId1\"/></sheets></workbook>");
            Text("xl/_rels/workbook.xml.rels", "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/></Relationships>");
            using var stream = zip.CreateEntry("xl/worksheets/sheet1.xml").Open();
            using var xml = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = new System.Text.UTF8Encoding(false) });
            const string ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
            xml.WriteStartElement("worksheet", ns); xml.WriteStartElement("sheetData", ns);
            foreach (var row in new[] { columns.Cast<object?>().ToArray() }.Concat(rows))
            {
                xml.WriteStartElement("row", ns);
                foreach (var cell in row)
                {
                    xml.WriteStartElement("c", ns);
                    if (cell is decimal or int or long) xml.WriteElementString("v", ns, Convert.ToString(cell, CultureInfo.InvariantCulture));
                    else { xml.WriteAttributeString("t", "inlineStr"); xml.WriteStartElement("is", ns); xml.WriteElementString("t", ns, new string((Convert.ToString(cell) ?? "").Where(XmlConvert.IsXmlChar).ToArray())); xml.WriteEndElement(); }
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();
            }
            xml.WriteEndElement(); xml.WriteEndElement();
        }
        return output.ToArray();
    }
    public static byte[] Pdf(string title, IEnumerable<string> rows)
    {
        using var output = new MemoryStream(); using var document = SKDocument.CreatePdf(output);
        using var paint = new SKPaint { Color = SKColors.Black, IsAntialias = true };
        using var font = new SKFont(SKTypeface.Default, 10); SKCanvas? canvas = null; float y = 0;
        foreach (var row in new[] { title, "" }.Concat(rows))
        {
            // Bound line width while retaining the entire record over successive lines/pages.
            var remaining = row.Replace('\r', ' ').Replace('\n', ' ');
            do {
                if (canvas == null || y > 800) { if (canvas != null) document.EndPage(); canvas = document.BeginPage(595, 842); y = 40; }
                int length = Math.Min(85, remaining.Length);
                canvas.DrawText(remaining[..length], 32, y, SKTextAlign.Left, font, paint); y += 15;
                remaining = remaining[length..];
            } while (remaining.Length > 0);
        }
        if (canvas != null) document.EndPage(); document.Close(); return output.ToArray();
    }
}
