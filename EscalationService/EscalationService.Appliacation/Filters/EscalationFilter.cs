using Models;

namespace EscalationService.Appliacation.Filters;

public class EscalationFilter
{
    public EscalationStatus? Status { get; set; }
    public int? AuthorId { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
}