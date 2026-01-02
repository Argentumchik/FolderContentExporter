using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FolderContentExporter.Dto
{
    public enum ExportMode
    {
        [Description("Plain Text (.txt)")]
        TXT,
        [Description("Comma-Separated Values (.csv)")]
        CSV,
        [Description("JavaScript Object Notation (.json)")]
        JSON
    }
}
