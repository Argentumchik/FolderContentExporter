using FileRedact.Core.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileRedact.Core.Interfaces
{
    public interface IFileSystemService
    {
        IEnumerable<TextFileItem> GetFiles(string path, bool sub);
    }
}
