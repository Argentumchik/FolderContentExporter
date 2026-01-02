using FolderContentExporter.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace FolderContentExporter.Interfaces
{
    public interface IFileSystemService
    {
        IAsyncEnumerable<TextFileItem> GetFilesAsync(string path, bool sub, CancellationToken token);
        Task<int> TotalFilesAsync(string path, bool sub);
    }
}
