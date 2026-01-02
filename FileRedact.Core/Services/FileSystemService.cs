using FolderContentExporter.Dto;
using FolderContentExporter.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace FolderContentExporter.Services
{
    public class FileSystemService : IFileSystemService
    {
        public async IAsyncEnumerable<TextFileItem> GetFilesAsync(string path, bool sub, [EnumeratorCancellation]CancellationToken token)
        {
            var options = new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = sub,
                ReturnSpecialDirectories = false
            };

            foreach (var file in Directory.EnumerateFiles(path, "*", options))
            {
                token.ThrowIfCancellationRequested();

                FileInfo info;

                try
                {
                    info = new FileInfo(file);
                }
                catch
                {
                    continue;
                }

                yield return new TextFileItem(
                    Path.GetFileNameWithoutExtension(info.Name),
                    info.DirectoryName,
                    info.Extension,
                    info.Length,
                    info.CreationTimeUtc,
                    info.LastWriteTimeUtc
                );

                await Task.Yield();
            }
        }

        public async Task<int> TotalFilesAsync(string path, bool sub)
        {
            return await Task.Run(() =>
                Directory.EnumerateFiles(
                    path,
                    "*",
                    new EnumerationOptions
                    {
                        IgnoreInaccessible = true,
                        RecurseSubdirectories = sub,
                        ReturnSpecialDirectories = false
                    }).Count()
                );
        }
    }
}
