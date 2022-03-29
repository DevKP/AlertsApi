namespace AlertsApi.Domain.Entities;

public class Alert : IEntity
{
    public int Id { get; set; }

    public string? LocationName { get; set; }

    public DateTime? UpdateTime { get; set; }

    public bool Active { get; set; }
}