using System;
using System.Collections.Generic;
using System.Text;

namespace FolderContentExporter.Enums
{
    public enum OperationState
    {
        Idle,
        Loading,
        Cancelling,
        Completed,
        Cancelled,
        Failed
    }
}
