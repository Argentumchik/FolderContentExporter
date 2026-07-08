using FolderContentExporter.Dto;
using FolderContentExporter.Enums;
using FolderContentExporter.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FolderContentExporter.Services
{
    public class ErrorMapper : IErrorMapper
    {
        public AppError Map(Exception ex)
        {
            return ex switch
            {
                UnauthorizedAccessException => new AppError(AppErrorType.AccessDenied, "Access denied to the selected folder.", ex),
                DirectoryNotFoundException => new AppError(AppErrorType.PathNoFound, "The selected folder was not found.", ex),
                IOException => new AppError(AppErrorType.FileInUse, "One or more files are in use.", ex),
                ArgumentException => new AppError(AppErrorType.InvalidInput, "The input provided was invalid.", ex),
                InvalidOperationException => new AppError(AppErrorType.OperationFailed, "The operation failed due to an invalid operation.", ex),
                _ => new AppError(AppErrorType.Unknown, "An unknown error occurred.", ex)
            };
        }
    }
}
