using FolderContentExporter.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace FolderContentExporter.Interfaces
{
    public interface IFileExportService
    {
        void ExportFiles(IEnumerable<TextFileItem> files, string exportPath, ExportOptionsDto options);
    }
}
