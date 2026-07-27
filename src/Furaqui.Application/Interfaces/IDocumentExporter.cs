using Furaqui.Domain.Entities;
using Furaqui.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furaqui.Application.Interfaces
{
    public interface IDocumentExporter
    {
        Task<Result> Excel(object? listData);
        Task<Result> Word(object? listData);
        Task<Result> Pdf(object? listData);
        Task<Result> Exportar(object? listaDatos, ExportReportType eFuraquiExportReportTypeApi);
    }
}
