namespace AlertsApi.Domain.Entities;

public class Subscription : IEntity
{
    public int Id { get; set; }
    public long UserId { get; set; }

    public string AlertHashTag { get; set; }
    public virtual Alert? Alert { get; set; }
}