using FileRedact.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileRedact.Core.Dto
{
    public class TextFileItem(string name, string path, string extention, long size, DateTime created, DateTime modified)
    {
        public string Name { get; set; } = name;
        public string Path { get; set; } = path;
        public string Extention { get; set; } = extention;
        public string Size { get; set; } = Formatter.FormatSize(size);
        public DateTime Created { get; set; } = created;
        public DateTime Modified { get; set; } = modified;


    }
}
