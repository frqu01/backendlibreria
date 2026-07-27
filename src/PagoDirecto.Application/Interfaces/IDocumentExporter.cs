using PagoDirecto.Domain.Entities;
using PagoDirecto.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagoDirecto.Application.Interfaces
{
    public interface IDocumentExporter
    {
        Task<Result> Excel(object? listData);
        Task<Result> Word(object? listData);
        Task<Result> Pdf(object? listData);
        Task<Result> Exportar(object? listaDatos, ExportReportType ePagoDirectoExportReportTypeApi);
    }
}

