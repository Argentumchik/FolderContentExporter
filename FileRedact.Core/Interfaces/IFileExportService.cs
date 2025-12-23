using FileRedact.Core.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileRedact.Core.Interfaces
{
    public interface IFileExportService
    {
        void ExportToTxt(IEnumerable<TextFileItem> files, string exportPath, string exportFileName);
        void ExportToCsv(IEnumerable<TextFileItem> files, string exportPath, string exportFileName);
        void ExportToJson(IEnumerable<TextFileItem> files, string exportPath, string exportFileName);
    }
}
