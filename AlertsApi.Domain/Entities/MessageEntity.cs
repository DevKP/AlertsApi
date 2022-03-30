namespace AlertsApi.Domain.Entities;

public class MessageEntity : IEntity
{
    public int Id { get; set; }

    public string? Message { get; set; }

    public DateTime? Date { get; set; }
}