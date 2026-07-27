using PagoDirecto.Application.Extensions;
using PagoDirecto.Domain.Entities;
using PagoDirecto.Domain.Enums;
using PagoDirecto.Application.Interfaces;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PagoDirecto.Infrastructure.Repositories
{
    internal class DocumentExporterRepository : IDocumentExporter
    {
        private readonly ILogger<DocumentExporterRepository> _logger;

        public DocumentExporterRepository(ILogger<DocumentExporterRepository> logger)
        {
            _logger = logger;
        }
        public Task<Result> Exportar(object? listaDatos, ExportReportType ePagoDirectoExportReportTypeApi)
        {
            if (listaDatos == null || !(listaDatos is System.Collections.IEnumerable enumerableDatos))
            {
                return Task.FromResult(ErrorResult("No se envió una lista de datos válida."));
            }

            var list = enumerableDatos.Cast<object>().ToList();
            if (list.Count < 1)
            {
                return Task.FromResult(ErrorResult("No se encontraron registros para exportar."));
            }

            switch (ePagoDirectoExportReportTypeApi)
            {
                case ExportReportType.Pdf:
                    return Pdf(list);
                case ExportReportType.Excel:
                    return Excel(list);
                case ExportReportType.Word:
                    return Word(list);
                default:
                    return Task.FromResult(ErrorResult("Tipo de exportación no soportado."));
            }
        }

        private Task<Result> Excel(object? listaDatos)
        {
            if (listaDatos == null || !(listaDatos is System.Collections.IEnumerable enumerableDatos))
                return Task.FromResult(ErrorResult("No se envió una lista de datos válida."));

            var list = enumerableDatos.Cast<object>().ToList();
            if (list.Count < 1)
                return Task.FromResult(ErrorResult("No se encontraron registros para exportar."));

            try
            {
                var primerDato = list[0];
                var properties = primerDato.GetType().GetProperties();

                string nombreArchivo = primerDato.GetType().Name
                            + DateTime.Now.ToString("yyyyMMddHHmmssfff")
                            + ".xlsx";

                using (MemoryStream memoryStream = new MemoryStream())
            {
                using (var wbook = new XLWorkbook())
                {
                    var sheet = wbook.Worksheets.Add("Sheet1");

                    #region Cabecera
                    int contadorCabecera = 1;
                    foreach (var item in properties)
                    {
                        sheet.Cell(1, contadorCabecera).Value = item.Name;
                        sheet.Cell(1, contadorCabecera).Style.Font.Bold = true;
                        sheet.Cell(1, contadorCabecera).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        sheet.Cell(1, contadorCabecera).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        sheet.Cell(1, contadorCabecera).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        sheet.Cell(1, contadorCabecera).Style.Fill.BackgroundColor = XLColor.BlueGray;
                        sheet.Cell(1, contadorCabecera).Style.Font.FontColor = XLColor.White;
                        sheet.Column(contadorCabecera).Width = 20;
                        contadorCabecera++;
                    }
                    #endregion

                    #region Body del excel
                    int recordIndex = 2;
                    foreach (var data in list)
                    {
                        int columnIndex = 1;
                        foreach (var item in properties)
                        {
                            var value = item.GetValue(data, null) ?? "";

                            sheet.Cell(recordIndex, columnIndex).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            sheet.Cell(recordIndex, columnIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            switch (EnumExtensions.GetEnumByName<ExportColumnType>(item.PropertyType.Name))
                            {
                                case ExportColumnType.String:
                                    sheet.Cell(recordIndex, columnIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                    break;
                                case ExportColumnType.Int32:
                                    sheet.Cell(recordIndex, columnIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                    break;
                                case ExportColumnType.Boolean:
                                    value = Equals(value, true) ? "Verdadero" : "Falso";
                                    sheet.Cell(recordIndex, columnIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    break;
                                case ExportColumnType.Decimal:
                                    value = string.Format("{0:0.##}", value);
                                    sheet.Cell(recordIndex, columnIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                    break;
                                case ExportColumnType.Datetime:
                                    sheet.Cell(recordIndex, columnIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    break;
                                case ExportColumnType.Int64:
                                    sheet.Cell(recordIndex, columnIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                    break;
                                case ExportColumnType.Int16:
                                    sheet.Cell(recordIndex, columnIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                    break;
                                case ExportColumnType.Double:
                                    value = string.Format("{0:0.##}", value);
                                    sheet.Cell(recordIndex, columnIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                    break;
                                default:
                                    sheet.Cell(recordIndex, columnIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                    break;
                            }

                            sheet.Cell(recordIndex, columnIndex).Value = value?.ToString();

                            columnIndex++;
                        }
                        recordIndex++;
                    }
                    #endregion

                    wbook.SaveAs(memoryStream);
                    memoryStream.Position = 0;

                    var resultadoApi = new Result()
                    {
                        RequestStatus = new RequestStatus()
                        {
                            IsSuccess = true,
                            NotificationType = NotificationType.Success,
                            ResponseMessage = "Exportado correctamente."
                        },
                        Data = new ExportFile()
                        {
                            Content = memoryStream.ToArray(),
                            FileName = nombreArchivo,
                            ContentType = ExportReportType.Excel.GetString()
                        }
                    };
                    return Task.FromResult(resultadoApi);
                }
            }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el reporte en Excel.");
                return Task.FromResult(ErrorResult("Ocurrió un error inesperado al generar el archivo Excel."));
            }
        }

        private Task<Result> Pdf(object? listaDatos)
        {
            if (listaDatos == null || !(listaDatos is System.Collections.IEnumerable enumerableDatos))
                return Task.FromResult(ErrorResult("No se envió una lista de datos válida."));

            var list = enumerableDatos.Cast<object>().ToList();
            if (list.Count < 1)
                return Task.FromResult(ErrorResult("No se encontraron registros para exportar."));

            try
            {
                var primerDato = list[0];
                var properties = primerDato.GetType().GetProperties();

                string nombreArchivo = primerDato.GetType().Name
                            + DateTime.Now.ToString("yyyyMMddHHmmssfff")
                            + ".pdf";

                using (MemoryStream memoryStream = new MemoryStream())
            {
                PdfWriter pdfWriter = new PdfWriter(memoryStream);
                pdfWriter.SetCloseStream(false);
                PdfDocument pdfDocument = new PdfDocument(pdfWriter);
                iText.Layout.Document document = new iText.Layout.Document(pdfDocument, iText.Kernel.Geom.PageSize.A4.Rotate());

                #region Cabecera
                int contadorCabecera = properties.Length;

                iText.Layout.Element.Table tablaGrilla = new iText.Layout.Element.Table(UnitValue.CreatePercentArray(contadorCabecera)).UseAllAvailableWidth();

                foreach (var item in properties)
                {
                    iText.Layout.Element.Cell cell = new iText.Layout.Element.Cell()
                    .SetBackgroundColor(new DeviceGray(0.75f))
                    .Add(new iText.Layout.Element.Paragraph(item.Name))
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                    tablaGrilla.AddCell(cell);
                }
                #endregion

                #region Body del pdf
                foreach (var data in list)
                {
                    foreach (var item in properties)
                    {
                        var value = item.GetValue(data, null) ?? "";
                        var textAlignment = new iText.Layout.Properties.TextAlignment();

                        switch (EnumExtensions.GetEnumByName<ExportColumnType>(item.PropertyType.Name))
                        {
                            case ExportColumnType.String:
                                textAlignment = iText.Layout.Properties.TextAlignment.LEFT;
                                break;
                            case ExportColumnType.Int32:
                                textAlignment = iText.Layout.Properties.TextAlignment.RIGHT;
                                break;
                            case ExportColumnType.Boolean:
                                value = Equals(value, true) ? "Verdadero" : "Falso";
                                textAlignment = iText.Layout.Properties.TextAlignment.CENTER;
                                break;
                            case ExportColumnType.Decimal:
                                value = string.Format("{0:0.##}", value);
                                textAlignment = iText.Layout.Properties.TextAlignment.RIGHT;
                                break;
                            case ExportColumnType.Datetime:
                                textAlignment = iText.Layout.Properties.TextAlignment.CENTER;
                                break;
                            case ExportColumnType.Int64:
                                textAlignment = iText.Layout.Properties.TextAlignment.RIGHT;
                                break;
                            case ExportColumnType.Int16:
                                textAlignment = iText.Layout.Properties.TextAlignment.RIGHT;
                                break;
                            case ExportColumnType.Double:
                                value = string.Format("{0:0.##}", value);
                                textAlignment = iText.Layout.Properties.TextAlignment.RIGHT;
                                break;
                            default:
                                textAlignment = iText.Layout.Properties.TextAlignment.LEFT;
                                break;
                        }

                        iText.Layout.Element.Cell cell = new iText.Layout.Element.Cell()
                        .Add(new iText.Layout.Element.Paragraph(value.ToString()))
                        .SetTextAlignment(textAlignment);
                        tablaGrilla.AddCell(cell);
                    }
                }
                #endregion

                document.Add(tablaGrilla);
                document.Close();

                byte[] exportBinary = memoryStream.ToArray();

                var resultadoApi = new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = true,
                        NotificationType = NotificationType.Success,
                        ResponseMessage = "Exportado correctamente."
                    },
                    Data = new ExportFile()
                    {
                        Content = exportBinary,
                        FileName = nombreArchivo,
                        ContentType = ExportReportType.Pdf.GetString()
                    }
                };
                return Task.FromResult(resultadoApi);
            }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el reporte en PDF.");
                return Task.FromResult(ErrorResult("Ocurrió un error inesperado al generar el archivo PDF."));
            }
        }

        private Task<Result> Word(object? listaDatos)
        {
            if (listaDatos == null || !(listaDatos is System.Collections.IEnumerable enumerableDatos))
                return Task.FromResult(ErrorResult("No se envió una lista de datos válida."));

            var list = enumerableDatos.Cast<object>().ToList();
            if (list.Count < 1)
                return Task.FromResult(ErrorResult("No se encontraron registros para exportar."));

            try
            {
                var primerDato = list[0];
                var properties = primerDato.GetType().GetProperties();

                string nombreArchivo = primerDato.GetType().Name
                            + DateTime.Now.ToString("yyyyMMddHHmmssfff")
                            + ".docx";

                using (var memoryStream = new MemoryStream())
            {
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document, true))
                {
                    //Documento y cuerpo
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());

                    OrientacionHorizontal(body);

                    Table table = new Table();
                    TablaDimension(table);

                    #region Cabecera
                    TableRow tr1 = new TableRow();
                    foreach (var item in properties)
                    {
                        TableCell tableCell = new TableCell();
                        Paragraph paragraph = new Paragraph();
                        Run run = new Run();
                        RunProperties runProperties = new RunProperties();
                        ParagraphProperties paragraphProperties = new ParagraphProperties();
                        paragraphProperties.Justification = new Justification() { Val = JustificationValues.Center };
                        CeldaBorde(tableCell);

                        paragraph.Append(paragraphProperties);
                        runProperties.Bold = new Bold();
                        run.Append(runProperties);
                        run.Append(new Text(item.Name));
                        paragraph.Append(run);
                        tableCell.Append(paragraph);

                        tr1.Append(tableCell);
                    }
                    table.Append(tr1);
                    #endregion

                    #region Body del word
                    foreach (var data in list)
                    {
                        TableRow tableRow = new TableRow();

                        foreach (var item in properties)
                        {
                            var value = item.GetValue(data, null) ?? "";

                            TableCell tableCell = new TableCell();
                            Paragraph paragraph = new Paragraph();
                            Run run = new Run();
                            RunProperties runProperties = new RunProperties();
                            ParagraphProperties paragraphProperties = new ParagraphProperties();

                            CeldaBorde(tableCell);

                            switch (EnumExtensions.GetEnumByName<ExportColumnType>(item.PropertyType.Name))
                            {
                                case ExportColumnType.String:
                                    paragraphProperties.Justification = new Justification() { Val = JustificationValues.Left };
                                    break;
                                case ExportColumnType.Int32:
                                    paragraphProperties.Justification = new Justification() { Val = JustificationValues.Right };
                                    break;
                                case ExportColumnType.Boolean:
                                    value = Equals(value, true) ? "Verdadero" : "Falso";
                                    paragraphProperties.Justification = new Justification() { Val = JustificationValues.Center };
                                    break;
                                case ExportColumnType.Decimal:
                                    value = string.Format("{0:0.##}", value);
                                    paragraphProperties.Justification = new Justification() { Val = JustificationValues.Right };
                                    break;
                                case ExportColumnType.Datetime:
                                    paragraphProperties.Justification = new Justification() { Val = JustificationValues.Center };
                                    break;
                                case ExportColumnType.Int64:
                                    paragraphProperties.Justification = new Justification() { Val = JustificationValues.Right };
                                    break;
                                case ExportColumnType.Int16:
                                    paragraphProperties.Justification = new Justification() { Val = JustificationValues.Right };
                                    break;
                                case ExportColumnType.Double:
                                    value = string.Format("{0:0.##}", value);
                                    paragraphProperties.Justification = new Justification() { Val = JustificationValues.Right };
                                    break;
                                default:
                                    paragraphProperties.Justification = new Justification() { Val = JustificationValues.Left };
                                    break;
                            }

                            paragraph.Append(paragraphProperties);
                            run.Append(runProperties);
                            run.Append(new Text(value?.ToString() ?? string.Empty));
                            paragraph.Append(run);
                            tableCell.Append(paragraph);

                            tableRow.Append(tableCell);
                        }
                        table.Append(tableRow);
                    }
                    #endregion

                    body.Append(table);

                    mainPart.Document.Save();
                } // IMPORTANTE: Cerrar el WordprocessingDocument ANTES de leer el stream

                byte[] exportWord = memoryStream.ToArray();

                var resultadoApi = new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = true,
                        NotificationType = NotificationType.Success,
                        ResponseMessage = "Exportado correctamente."
                    },
                    Data = new ExportFile()
                    {
                        Content = exportWord,
                        FileName = nombreArchivo,
                        ContentType = ExportReportType.Word.GetString()
                    }
                };

                return Task.FromResult(resultadoApi);
            }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el reporte en Word.");
                return Task.FromResult(ErrorResult("Ocurrió un error inesperado al generar el archivo Word."));
            }
        }

        private static Result ErrorResult(string message)
        {
            return new Result()
            {
                RequestStatus = new RequestStatus()
                {
                    IsSuccess = false,
                    NotificationType = NotificationType.Error,
                    ResponseMessage = message
                }
            };
        }

        private void OrientacionHorizontal(Body body)
        {
            SectionProperties sectionProperties = new SectionProperties();
            PageSize pageSize = new PageSize() { Width = 16838U, Height = 11906U, Orient = PageOrientationValues.Landscape };
            sectionProperties.Append(pageSize);
            body.Append(sectionProperties);
        }

        private void OrientacionVertical(Body body)
        {
            SectionProperties sectionProperties = new SectionProperties();
            PageSize pageSize = new PageSize() { Width = 16838U, Height = 11906U, Orient = PageOrientationValues.Portrait };
            sectionProperties.Append(pageSize);
            body.Append(sectionProperties);
        }

        private void TablaDimension(Table table)
        {
            TableProperties tableProperties = new TableProperties();
            TableWidth tableWidth = new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct };
            TableStyle tableStyle = new TableStyle() { Val = "TableGrid" };

            tableProperties.Append(tableStyle, tableWidth);
            table.Append(tableProperties);
        }

        private void CeldaBorde(TableCell tableCell)
        {
            TableCellProperties tableCellProperties = new TableCellProperties();
            TableCellBorders tableCellBorders = new TableCellBorders();
            LeftBorder leftBorder = new LeftBorder() { Color = "000000", Val = new EnumValue<BorderValues>() { Value = BorderValues.Thick } };
            RightBorder rightBorder = new RightBorder() { Color = "000000", Val = new EnumValue<BorderValues>() { Value = BorderValues.Thick } };
            TopBorder topBorder = new TopBorder() { Color = "000000", Val = new EnumValue<BorderValues>() { Value = BorderValues.Thick } };
            BottomBorder bottomBorder = new BottomBorder() { Color = "000000", Val = new EnumValue<BorderValues>() { Value = BorderValues.Thick } };

            tableCellBorders.Append(leftBorder);
            tableCellBorders.Append(rightBorder);
            tableCellBorders.Append(topBorder);
            tableCellBorders.Append(bottomBorder);
            tableCellProperties.Append(tableCellBorders);
            tableCell.Append(tableCellProperties);
        }
    }
}
