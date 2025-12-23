using CsvHelper;
using FileRedact.Core.Dto;
using FileRedact.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;

namespace FileRedact.Core.Services
{
    public class FileExportService : IFileExportService
    {
        public void ExportToTxt(IEnumerable<TextFileItem> files, string exportPath, string exportFileName)
        {
            using var writer = new StreamWriter(Path.Combine(exportPath, $"{exportFileName}.txt"), false, Encoding.UTF8);

            foreach (var file in files)
            {
                writer.WriteLine(file.Name);
            }
        }
        public void ExportToCsv(IEnumerable<TextFileItem> files, string exportPath, string exportFileName)
        {
            using var writer = new StreamWriter(Path.Combine(exportPath, $"{exportFileName}.csv"), false, Encoding.UTF8);
            
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            var data = files.Select(f => new FileExportDto { FileName = f.Name });

            csv.WriteRecords(data);
        }
        public void ExportToJson(IEnumerable<TextFileItem> files, string exportPath, string exportFileName)
        {
            var data = files.Select(f => new FileExportDto { FileName = f.Name });

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(data, options);

            File.WriteAllText(Path.Combine(exportPath, $"{exportFileName}.json"), json, Encoding.UTF8);
        }
    }
}
