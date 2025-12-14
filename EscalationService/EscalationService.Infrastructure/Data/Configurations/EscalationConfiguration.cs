using EscalationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace EscalationService.Infrastructure.Data.Configurations;

public class EscalationConfiguration : IEntityTypeConfiguration<Escalation>
{
    public void Configure(EntityTypeBuilder<Escalation> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(e => e.Description)
            .HasMaxLength(1000);

        builder.Property(e => e.Status)
            .IsRequired();
        
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();
        
        builder.Property(e => e.AuthorId)
            .IsRequired(); 
        
        builder.HasData(
            new Escalation
            {
                Id = 1,
                Name = "Проблема с доступом",
                Description = "Пользователь не может войти в систему.",
                Status = EscalationStatus.New, // ← убедись, что это int-значение (0, 1, ...)
                CreatedAt = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc),
                IsFeatured = false,
                AuthorId = 1 
            }
        );
    }
}