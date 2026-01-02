using System;
using System.Collections.Generic;
using System.Text;

namespace FolderContentExporter.Services
{
    public static class Formatter
    {
        public static string FormatSize(long size)
        {
            const long KB = 1024;
            const long MB = KB * 1024;
            const long GB = MB * 1024;

            if (size >= KB && size <= MB)
                return $"{size / (double)KB:0.##} KB";
            if (size >= MB && size <= GB)
                return $"{size / (double)MB:0.##} MB";
            if (size >= GB)
                return $"{size / (double)GB:0.##} GB";

            return $"{size} B";
        }
    }
}
