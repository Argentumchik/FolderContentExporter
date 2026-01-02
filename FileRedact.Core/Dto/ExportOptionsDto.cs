using System;
using System.Collections.Generic;
using System.Text;

namespace FolderContentExporter.Dto
{
    public class ExportOptionsDto
    {
        public ExportFields Fields { get; set; }
        public ExportMode Mode { get; set; }
        public string FileName { get; set; } = string.Empty;
    }
}
