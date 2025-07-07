using Models;
using Models.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReportingService.API.Contracts;

namespace ReportingService.API.Services;

public class ReportService
{
    private readonly IMessageBusPublisher _publisher;

    public ReportService(IMessageBusPublisher publisher)
    {
        _publisher = publisher;
        QuestPDF.Settings.License = LicenseType.Community;
    }
    
    public async Task<byte[]> GenerateReportAsync(DateTime from, DateTime to, EscalationStatus? status)
    {
        var escalations = await _publisher.GetEscalationsAsync(
            new ReportRequest { FromDate = from, ToDate = to, Status = status}
        );
        
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));
                
                page.Header()
                    .Text("Отчет по эскалациям WaveAccess")
                    .SemiBold().FontSize(24).FontColor(Colors.Green.Medium);
                
                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Item().Text($"Период: {from:d} - {to:d}");
                        column.Item().Text($"Статус: {status?.ToString() ?? "Все"}");
                        column.Item().PaddingTop(10).Text("Список эскалаций:");

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3); // Название
                                columns.RelativeColumn(2); // Статус
                                columns.RelativeColumn(3); // Описание
                                columns.RelativeColumn(2); // Дата
                            });
                            
                            // Заголовок таблицы
                            table.Header(header =>
                            {
                                header.Cell().Text("Название");
                                header.Cell().Text("Статус");
                                header.Cell().Text("Описание");
                                header.Cell().Text("Дата");
                                
                                header.Cell().ColumnSpan(4)
                                    .PaddingTop(5).BorderBottom(1)
                                    .BorderColor(Colors.Black);
                            });
                            
                            // Данные таблицы
                            foreach (var esc in escalations)
                            {
                                table.Cell().Text(esc.Name);
                                table.Cell().Text(esc.Status.ToString());
                                table.Cell().Text(esc.Description);
                                table.Cell().Text(esc.CreatedAt.ToString("d"));
                            }
                        });
                    });
                
                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Страница ");
                        x.CurrentPageNumber();
                        x.Span(" из ");
                        x.TotalPages();
                    });
            });
        });
        return document.GeneratePdf();
    }
}