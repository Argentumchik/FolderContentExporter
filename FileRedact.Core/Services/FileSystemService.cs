using FileRedact.Core.Dto;
using FileRedact.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileRedact.Core.Services
{
    public class FileSystemService : IFileSystemService
    {
        public IEnumerable<TextFileItem> GetFiles(string path, bool sub)
        {
            var options = new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = sub,
                ReturnSpecialDirectories = false
            };
            foreach (var file in Directory.EnumerateFiles(path, "*", options))
            {
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
            }
        }
    }
}
