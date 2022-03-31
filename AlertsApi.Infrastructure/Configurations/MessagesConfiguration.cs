using AlertsApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlertsApi.Infrastructure.Configurations;

public class MessagesConfiguration : IEntityTypeConfiguration<DbMessage>
{
    public void Configure(EntityTypeBuilder<DbMessage> builder)
    {
        builder.HasKey(alert => alert.Id);
        builder.Property(alert => alert.Message).IsRequired().HasMaxLength(500);
        builder.Property(alert => alert.Date).IsRequired();
    }
}