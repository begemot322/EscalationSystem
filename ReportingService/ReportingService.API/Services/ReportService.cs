using System.Security.Claims;
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
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ReportService(IMessageBusPublisher publisher, IHttpContextAccessor httpContextAccessor)
    {
        _publisher = publisher;
        _httpContextAccessor = httpContextAccessor;
        QuestPDF.Settings.License = LicenseType.Community;
    }
    
    public async Task<byte[]> GenerateReportAsync(DateTime from, DateTime to, EscalationStatus? status)
    {
        if (!CanCreateReport())
            throw new UnauthorizedAccessException("У вас нет прав для генерации отчетов");

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
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Helvetica"));
                
                page.Header()
                    .Height(2.5f, Unit.Centimetre)
                    .BorderBottom(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .AlignCenter()
                    .AlignMiddle()
                    .Text("WaveForm - отчет по эскалациям")
                    .Bold().FontSize(16).FontColor(Colors.Black);
                
                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Item()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(10)
                            .Row(row =>
                            {
                                row.RelativeItem().Text($"Период: {from:dd.MM.yyyy} - {to:dd.MM.yyyy}");
                                row.RelativeItem().Text($"Статус: {GetStatusText(status)}").AlignRight();
                                row.RelativeItem().Text($"Всего: {escalations.Count}").AlignRight();
                            });
                        
                        column.Item().PaddingTop(15);

                        if (escalations.Any())
                        {
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2); 
                                    columns.RelativeColumn(1.5f); 
                                    columns.RelativeColumn(3); 
                                    columns.RelativeColumn(1.2f); 
                                });
                                
                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5)
                                        .Text("Название").Bold().FontColor(Colors.Black);
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5)
                                        .Text("Статус").Bold().FontColor(Colors.Black);
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5)
                                        .Text("Описание").Bold().FontColor(Colors.Black);
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5)
                                        .Text("Дата создания").Bold().FontColor(Colors.Black);
                                });
                                
                                for (int i = 0; i < escalations.Count; i++)
                                {
                                    var esc = escalations[i];
                                    var bgColor = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;
                                    
                                    table.Cell().Background(bgColor).Padding(5).Text(esc.Name);
                                    table.Cell().Background(bgColor).Padding(5)
                                        .Text(GetStatusText(esc.Status)).FontColor(GetStatusColor(esc.Status));
                                    table.Cell().Background(bgColor).Padding(5).Text(esc.Description);
                                    table.Cell().Background(bgColor).Padding(5).Text(esc.CreatedAt.ToString("dd.MM.yyyy"));
                                }
                            });
                        }
                        else
                        {
                            column.Item()
                                .Background(Colors.Grey.Lighten3)
                                .Padding(20)
                                .Border(1)
                                .BorderColor(Colors.Grey.Lighten2)
                                .AlignCenter()
                                .Text("Нет данных за выбранный период")
                                .Italic().FontSize(14).FontColor(Colors.Grey.Medium);
                        }
                    });
                
                page.Footer()
                    .Height(1.2f, Unit.Centimetre)
                    .BorderTop(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .AlignCenter()
                    .AlignMiddle()
                    .Text(text =>
                    {
                        text.Span("Сгенерировано: ").FontSize(13).FontColor(Colors.Black);
                        text.Span(DateTime.Now.ToString("dd.MM.yyyy HH:mm")).FontSize(13).FontColor(Colors.Black);
                        text.Span(" | Страница ").FontSize(13).FontColor(Colors.Black);
                        text.CurrentPageNumber().FontSize(13).FontColor(Colors.Black);
                        text.Span(" из ").FontSize(13).FontColor(Colors.Black);
                        text.TotalPages().FontSize(13).FontColor(Colors.Black);
                    });
            });
        });
        
        return document.GeneratePdf();
    }
    
    private string GetStatusText(EscalationStatus? status)
    {
        return status switch
        {
            EscalationStatus.New => "Новая",
            EscalationStatus.InProgress => "В работе",
            EscalationStatus.OnReview => "На проверке",
            EscalationStatus.Completed => "Завершена",
            EscalationStatus.Rejected => "Отклонена",
            _ => "Все статусы"
        };
    }
    
    private string GetStatusColor(EscalationStatus status)
    {
        return status switch
        {
            EscalationStatus.New => Colors.Blue.Darken2,      
            EscalationStatus.InProgress => Colors.Orange.Darken2, 
            EscalationStatus.OnReview => Colors.Purple.Darken2,   
            EscalationStatus.Completed => Colors.Green.Darken2,  
            EscalationStatus.Rejected => Colors.Red.Darken2,    
            _ => Colors.Black
        };
    }
    
    private bool CanCreateReport()
    {
        var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
    
        if (roleClaim == null || roleClaim.Value == "Junior")
            return false;

        return true;
    }
}