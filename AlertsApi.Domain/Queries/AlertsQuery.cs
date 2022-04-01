namespace AlertsApi.Domain.Queries;

public class AlertsQuery
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public bool? Active { get; set; }
}