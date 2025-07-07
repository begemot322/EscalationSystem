namespace Models.DTOs;

public class ReportRequest
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public EscalationStatus? Status { get; set; }
}