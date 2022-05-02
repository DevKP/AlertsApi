using AlertsApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlertsApi.Infrastructure.Configurations;

public class AlertsConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> builder)
    {
        builder.HasKey(alert => alert.LocationName);
        builder.Property(alert => alert.Id).ValueGeneratedOnAdd();
        builder.Property(alert => alert.StartTime);
        builder.Property(alert => alert.EndTime);
        builder.Property(alert => alert.UpdateTime).IsRequired();
        builder.Property(alert => alert.Active).IsRequired();
        builder.Property(alert => alert.UsersNotified).IsRequired();
    }
}