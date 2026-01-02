using CsvHelper;
using FolderContentExporter.Dto;
using FolderContentExporter.Interfaces;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;

namespace FolderContentExporter.Services
{
    public class FileExportService : IFileExportService
    {
        private static readonly Dictionary<ExportFields, ExportColumn> Columns = new()
        {
            [ExportFields.Name] = new ExportColumn("File Name", f => f.Name),
            [ExportFields.Size] = new ExportColumn("Size", f => f.SizeDisplay),
            [ExportFields.Extension] = new ExportColumn("Extension", f => f.Extension),
            [ExportFields.Path] = new ExportColumn("Path", f => f.Path),
            [ExportFields.CreatedDate] = new ExportColumn("Created Date", f => f.Created.ToString("u")),
            [ExportFields.ModifiedDate] = new ExportColumn("Modified Date", f => f.Modified.ToString("u"))
        };

        public void ExportFiles(IEnumerable<TextFileItem> files, string exportPath, ExportOptionsDto options)
        {
            switch (options.Mode)
            {
                case ExportMode.TXT:
                    ExportToTxt(files, exportPath, options);
                    break;
                case ExportMode.CSV:
                    ExportToCsv(files, exportPath, options);
                    break;
                case ExportMode.JSON:
                    ExportToJson(files, exportPath, options);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        
        private void ExportToTxt(IEnumerable<TextFileItem> files, string exportPath, ExportOptionsDto options)
        {
            using var writer = new StreamWriter(Path.Combine(exportPath, $"{options.FileName}.txt"), false, Encoding.UTF8);

            var selectedColumns = GetSelectedColumns(options);

            foreach (var file in files)
            {
                var line = string.Join("|", selectedColumns.Select(c => c.Selector?.Invoke(file)));

                writer.WriteLine(line);
            }
        }
        private void ExportToCsv(IEnumerable<TextFileItem> files, string exportPath, ExportOptionsDto options)
        {
            using var writer = new StreamWriter(Path.Combine(exportPath, $"{options.FileName}.csv"), false, Encoding.UTF8);
            
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            var rows = new List<ExpandoObject>();
            var selectedColumns = GetSelectedColumns(options);

            foreach (var file in files)
            {
                IDictionary<string, object> row = new ExpandoObject();

                foreach (var column in selectedColumns)
                {
                    row[column.Header] = column.Selector(file);
                }

                rows.Add((ExpandoObject)row);
            }

            csv.WriteRecords<dynamic>(rows);
        }
        private void ExportToJson(IEnumerable<TextFileItem> files, string exportPath, ExportOptionsDto options)
        {
            var rows = files.Select(f => BuildRow(f, options)).ToList();

            var optionsJson = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(rows, optionsJson);

            File.WriteAllText(Path.Combine(exportPath, $"{options.FileName}.json"), json, Encoding.UTF8);
        }

        private Dictionary<string, object?> BuildRow(TextFileItem file, ExportOptionsDto options)
        {
            var row = new Dictionary<string, object?>();

            foreach (var (field, column) in Columns)
            {
                if (options.Fields.HasFlag(field))
                {
                    row[field.ToString()] = column.Selector(file);
                }
            }

            return row;
        }
        private List<ExportColumn> GetSelectedColumns(ExportOptionsDto options)
        {
            return Columns
                .Where(c => options.Fields.HasFlag(c.Key))
                .Select(c => c.Value)
                .ToList();
        }
    }
}
