namespace AlertsApi.Domain.Entities;

public class DbMessage : IEntity
{
    public int Id { get; set; }

    public string? Message { get; set; }

    public DateTime? Date { get; set; }
}