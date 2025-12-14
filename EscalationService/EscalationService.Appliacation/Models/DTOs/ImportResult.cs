using EscalationService.Domain.Entities;

namespace EscalationService.Appliacation.Models.DTOs;

public class ImportResult
{
    public List<Escalation> Successful { get; } = new();
    public List<(EscalationImportDto Dto, IEnumerable<string> Errors)> Failures { get; } = new();

    public void AddSuccess(Escalation escalation) => Successful.Add(escalation);

    public void AddError(EscalationImportDto dto, IEnumerable<string> errors) =>
        Failures.Add((dto, errors));

    public override string ToString()
    {
        return $@"
            Import completed:
            ✅ Successfully imported: {Successful.Count}
            ❌ Failed records: {Failures.Count}
            
            Failed details:
            {string.Join("\n", Failures.Select((f, i) => $"#{i+1}: {string.Join("; ", f.Errors)}"))}
            ";
    }
}