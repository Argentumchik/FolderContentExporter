using FolderContentExporter.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace FolderContentExporter.Interfaces
{
    public interface IErrorMapper
    {
        AppError Map(Exception ex);
    }
}
