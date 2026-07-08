using FolderContentExporter.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FolderContentExporter.Dto
{
    public class AppError
    {
        public AppErrorType ErrorType { get; }
        public string Message { get; }
        public Exception? Exception { get; }

        public AppError(AppErrorType errorType, string message, Exception? exception = null)
        {
            ErrorType = errorType;
            Message = message;
            Exception = exception;
        }
    }
}
