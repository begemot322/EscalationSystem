using EscalationService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscalationService.Infrastructure.Data.Configurations;

public class NotificationChannelConfiguration : IEntityTypeConfiguration<NotificationChannel>
{
    public void Configure(EntityTypeBuilder<NotificationChannel> builder)
    {
        builder.HasKey(nc => nc.Id);

        builder.Property(nc => nc.Code)
            .IsRequired()
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(nc => nc.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(nc => nc.Code).IsUnique();
        
        builder.HasData(
            new NotificationChannel { Id = 1, Code = "EMAIL", Name = "Электронная почта" },
            new NotificationChannel { Id = 2, Code = "PUSH", Name = "Push-уведомление" },
            new NotificationChannel { Id = 3, Code = "IN_APP", Name = "Внутри приложения" }
        );
    }
}