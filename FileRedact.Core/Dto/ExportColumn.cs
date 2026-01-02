using System;
using System.Collections.Generic;
using System.Text;

namespace FolderContentExporter.Dto
{
    public class ExportColumn
    {
        public string Header { get; set; } = string.Empty;
        public Func<TextFileItem, string> Selector { get; }

        public ExportColumn(string header, Func<TextFileItem, string> selector)
        {
            Header = header;
            Selector = selector;
        }
    }
}
