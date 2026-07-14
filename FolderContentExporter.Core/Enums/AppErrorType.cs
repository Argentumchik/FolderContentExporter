using System;
using System.Collections.Generic;
using System.Text;

namespace FolderContentExporter.Enums
{
    public enum AppErrorType
    {
        None,
        AccessDenied,
        PathNoFound,
        FileInUse,
        InvalidInput,
        OperationFailed,
        Unknown
    }
}
