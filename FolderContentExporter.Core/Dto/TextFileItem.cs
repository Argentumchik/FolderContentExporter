using FolderContentExporter.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace FolderContentExporter.Dto
{
    public class TextFileItem(string name, string path, string extention, long size, DateTime created, DateTime modified)
    {
        public string Name { get; set; } = name;
        public string SizeDisplay { get; set; } = Formatter.FormatSize(size);
        public string Extension { get; set; } = extention;
        public long Size { get; set; } = size;
        public string Path { get; set; } = path;
        public DateTime Created { get; set; } = created;
        public DateTime Modified { get; set; } = modified;
    }
}
