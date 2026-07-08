using System;
using System.Collections.Generic;
using System.Text;

namespace FolderContentExporter.Enums
{
    [Flags]
    public enum ExportFields
    {
        None = 0,
        Name = 2,
        Size = 4,
        Extension = 8,
        Path = 16,
        CreatedDate = 32,
        ModifiedDate = 64
    }
}
