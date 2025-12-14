using System.Globalization;
using EscalationService.Appliacation.Common.Interfaces;
using EscalationService.Appliacation.Models.DTOs;
using EscalationService.Appliacation.Services.Interfaces;
using EscalationService.Appliacation.Validators;
using EscalationService.Domain.Entities;
using Microsoft.Extensions.Logging;
using Models;

namespace EscalationService.Appliacation.Services.Implementation;

public class EtlService : IEtlService
{
     private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EtlService> _logger;

    public EtlService(IUnitOfWork unitOfWork, ILogger<EtlService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ImportResult> ImportEscalationsFromCsvAsync(string filePath)
    {
        var result = new ImportResult();
        var records = await ReadCsvAsync(filePath);

        foreach (var dto in records)
        {
            try
            {
                // Validate
                var validator = new EscalationImportDtoValidator();
                var validationResult = await validator.ValidateAsync(dto);
                
                if (!validationResult.IsValid)
                {
                    result.AddError(dto, validationResult.Errors.Select(e => e.ErrorMessage));
                    continue;
                }

                // Transform
                var escalation = MapToEntity(dto, validator);

                // Load
                await _unitOfWork.Escalations.AddAsync(escalation);
                await _unitOfWork.SaveChangesAsync();

                // Добавляем связи с пользователями
                foreach (var userId in dto.UserIds)
                {
                    var escalationUser = new EscalationUser
                    {
                        EscalationId = escalation.Id,
                        UserId = userId
                    };
                    await _unitOfWork.EscalationUsers.AddAsync(escalationUser);
                }

                await _unitOfWork.SaveChangesAsync();
                result.AddSuccess(escalation);

            }
            catch (Exception ex)
            {
                result.AddError(dto, new[] { $"Unexpected error: {ex.Message}" });
                _logger.LogError(ex, "Error processing record: {@Dto}", dto);
            }
        }

        return result;
    }

    private Escalation MapToEntity(EscalationImportDto dto, EscalationImportDtoValidator validator)
    {
        var normalizedStatus = EscalationImportDtoValidator.NormalizeStatus(dto.Status);
        var status = Enum.Parse<EscalationStatus>(normalizedStatus, true);

        DateTime createdAt;
        if (dto.CreatedAt.HasValue)
        {
            createdAt = dto.CreatedAt.Value.Kind switch
            {
                DateTimeKind.Unspecified => DateTime.SpecifyKind(dto.CreatedAt.Value, DateTimeKind.Utc),
                DateTimeKind.Local => dto.CreatedAt.Value.ToUniversalTime(),
                _ => dto.CreatedAt.Value
            };
        }
        else
        {
            createdAt = DateTime.UtcNow;
        }

        return new Escalation
        {
            Name = dto.Name,
            Description = dto.Description,
            Status = status,
            CreatedAt = createdAt,        
            UpdatedAt = DateTime.UtcNow,    
            IsFeatured = dto.IsFeatured,
            AuthorId = dto.AuthorId
        };
    }

    private async Task<List<EscalationImportDto>> ReadCsvAsync(string filePath)
    {
        var records = new List<EscalationImportDto>();
        using var reader = new StreamReader(filePath);
        using var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);

        // Пропускаем заголовок, если он есть
        await csv.ReadAsync();
        csv.ReadHeader();

        while (await csv.ReadAsync())
        {
            var record = new EscalationImportDto
            {
                Name = csv.GetField<string>("Name") ?? "",
                Description = csv.GetField<string>("Description") ?? "",
                Status = csv.GetField<string>("Status") ?? "New",
                CreatedAt = csv.GetField<DateTime?>("CreatedAt"),
                IsFeatured = csv.GetField<bool?>("IsFeatured") ?? false,
                AuthorId = csv.GetField<int?>("AuthorId") ?? 0,
                UserIds = ParseUserIds(csv.GetField<string>("UserIds"))
            };
            records.Add(record);
        }

        return records;
    }

    private List<int> ParseUserIds(string? userIdsString)
    {
        if (string.IsNullOrWhiteSpace(userIdsString))
            return new List<int>();

        return userIdsString
            .Split(',', ';', '|')
            .Select(s => s.Trim())
            .Where(s => int.TryParse(s, out _))
            .Select(int.Parse)
            .ToList();
    }
}