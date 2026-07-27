using Furaqui.Application.Extensions;
using Furaqui.Domain.Entities;
using Furaqui.Domain.Enums;
using Furaqui.Application.Interfaces;
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
using System.Text;
using System.Threading.Tasks;

namespace Furaqui.Infrastructure.Repositories
{
    internal class DocumentExporterRepository : IDocumentExporter
    {
        public async Task<Result> Exportar(object? listaDatos, ExportReportType eFuraquiExportReportTypeApi)
        {
            await Task.Delay(0);

            Result resultadoApi = new Result();

            if (!listaDatos.GetType().IsSerializable)
            {
                resultadoApi = new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = false,
                        NotificationTypeId = NotificationType.Error,
                        ResponseMessage = "No se envió una lista de datos."
                    }
                };

                return resultadoApi;
            }

            var list = (System.Collections.IList)listaDatos;
            if (list.Count < 1)
            {
                resultadoApi = new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = false,
                        NotificationTypeId = NotificationType.Error,
                        ResponseMessage = "No se encontraron registros para exportar."
                    }
                };

                return resultadoApi;
            }

            switch (eFuraquiExportReportTypeApi)
            {
                case ExportReportType.Pdf:
                    resultadoApi = await Pdf(listaDatos);
                    break;
                case ExportReportType.Excel:
                    resultadoApi = await Excel(listaDatos);
                    break;
                case ExportReportType.Word:
                    resultadoApi = await Word(listaDatos);
                    break;
                default:
                    break;
            }

            return resultadoApi;
        }
        public async Task<Result> Excel(object? listaDatos)
        {
            await Task.Delay(0);

            Result resultadoApi = new Result();

            if (!listaDatos.GetType().IsSerializable)
            {
                resultadoApi = new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = false,
                        NotificationTypeId = NotificationType.Error,
                        ResponseMessage = "No se envió una lista de datos."
                    }
                };

                return resultadoApi;
            }

            var list = (System.Collections.IList)listaDatos;
            if (list.Count < 1)
            {
                resultadoApi = new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = false,
                        NotificationTypeId = NotificationType.Error,
                        ResponseMessage = "No se encontraron registros para exportar."
                    }
                };

                return resultadoApi;
            }

            var primerDato = ((System.Collections.IList)listaDatos)[0];

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
                    foreach (var item in primerDato.GetType().GetProperties())
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
                    foreach (var data in (System.Collections.IEnumerable)listaDatos)
                    {
                        int columnIndex = 1;
                        foreach (var item in primerDato.GetType().GetProperties())
                        {
                            var value = data.GetType().GetProperty(item.Name).GetValue(data, null) ?? "";

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

                    resultadoApi = new Result()
                    {
                        RequestStatus = new RequestStatus()
                        {
                            IsSuccess = true,
                            NotificationTypeId = NotificationType.Success,
                            ResponseMessage = "Exportado correctamente."
                        },
                        Data = new ExportFile()
                        {
                            Content = memoryStream.ToArray(),
                            FileName = nombreArchivo,
                            ContentType = ExportReportType.Excel.GetString()
                        }
                    };
                }
            }

            return resultadoApi;
        }

        public async Task<Result> Pdf(object? listaDatos)
        {
            await Task.Delay(0);

            Result resultadoApi = new Result();

            if (!listaDatos.GetType().IsSerializable)
            {
                resultadoApi = new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = false,
                        NotificationTypeId = NotificationType.Error,
                        ResponseMessage = "No se envió una lista de datos."
                    }
                };

                return resultadoApi;
            }

            var list = (System.Collections.IList)listaDatos;
            if (list.Count < 1)
            {
                resultadoApi = new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = false,
                        NotificationTypeId = NotificationType.Error,
                        ResponseMessage = "No se encontraron registros para exportar."
                    }
                };

                return resultadoApi;
            }

            var primerDato = ((System.Collections.IList)listaDatos)[0];

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
                int contadorCabecera = 0;

                foreach (var item in primerDato.GetType().GetProperties())
                {
                    contadorCabecera++;
                }
                iText.Layout.Element.Table tablaGrilla = new iText.Layout.Element.Table(UnitValue.CreatePercentArray(contadorCabecera)).UseAllAvailableWidth();

                foreach (var item in primerDato.GetType().GetProperties())
                {
                    iText.Layout.Element.Cell cell = new iText.Layout.Element.Cell()
                    .SetBackgroundColor(new DeviceGray(0.75f))
                    .Add(new iText.Layout.Element.Paragraph(item.Name))
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                    tablaGrilla.AddCell(cell);
                }
                #endregion

                #region Body del pdf
                foreach (var data in (System.Collections.IEnumerable)listaDatos)
                {
                    foreach (var item in primerDato.GetType().GetProperties())
                    {
                        var value = data.GetType().GetProperty(item.Name).GetValue(data, null) ?? "";
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
                memoryStream.Write(exportBinary, 0, exportBinary.Length);
                memoryStream.Position = 0;

                resultadoApi = new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = true,
                        NotificationTypeId = NotificationType.Success,
                        ResponseMessage = "Exportado correctamente."
                    },
                    Data = new ExportFile()
                    {
                        Content = exportBinary,
                        FileName = nombreArchivo,
                        ContentType = ExportReportType.Pdf.GetString()
                    }
                };
            }

            return resultadoApi;
        }

        public async Task<Result> Word(object? listaDatos)
        {
            await Task.Delay(0);

            Result resultadoApi = new Result();

            if (!listaDatos.GetType().IsSerializable)
            {
                resultadoApi = new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = false,
                        NotificationTypeId = NotificationType.Error,
                        ResponseMessage = "No se envió una lista de datos."
                    }
                };

                return resultadoApi;
            }

            var list = (System.Collections.IList)listaDatos;
            if (list.Count < 1)
            {
                resultadoApi = new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = false,
                        NotificationTypeId = NotificationType.Error,
                        ResponseMessage = "No se encontraron registros para exportar."
                    }
                };

                return resultadoApi;
            }

            var primerDato = ((System.Collections.IList)listaDatos)[0];

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
                    foreach (var item in primerDato.GetType().GetProperties())
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
                    foreach (var data in (System.Collections.IEnumerable)listaDatos)
                    {
                        TableRow tableRow = new TableRow();

                        foreach (var item in primerDato.GetType().GetProperties())
                        {
                            var value = data.GetType().GetProperty(item.Name).GetValue(data, null) ?? "";

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
                            run.Append(new Text(value.ToString()));
                            paragraph.Append(run);
                            tableCell.Append(paragraph);

                            tableRow.Append(tableCell);
                        }
                        table.Append(tableRow);
                    }
                    #endregion

                    body.Append(table);

                    mainPart.Document.Save();
                    wordDocument.Dispose();

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    using (MemoryStream memoryStreamAux = new MemoryStream())
                    {
                        memoryStream.CopyTo(memoryStreamAux);
                        byte[] exportWord = memoryStreamAux.ToArray();

                        resultadoApi = new Result()
                        {
                            RequestStatus = new RequestStatus()
                            {
                                IsSuccess = true,
                                NotificationTypeId = NotificationType.Success,
                                ResponseMessage = "Exportado correctamente."
                            },
                            Data = new ExportFile()
                            {
                                Content = exportWord,
                                FileName = nombreArchivo,
                                ContentType = ExportReportType.Word.GetString()
                            }
                        };
                    }
                }
            }

            return resultadoApi;
        }
        private void Cabecera()
        {
            Paragraph paragraph = new Paragraph();
            Run run = new Run();
            RunProperties runProperties = new RunProperties()
            {
                Bold = new Bold(),
                FontSize = new FontSize()
                {
                    Val = new StringValue("32")
                }
            };
            ParagraphProperties paragraphProperties = new ParagraphProperties()
            {
                Justification = new Justification()
                {
                    Val = JustificationValues.Center
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
