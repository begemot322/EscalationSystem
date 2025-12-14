using EscalationService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscalationService.Infrastructure.Data.Configurations;

public class AttachmentTypeConfiguration : IEntityTypeConfiguration<AttachmentType>
{
    public void Configure(EntityTypeBuilder<AttachmentType> builder)
    {
        builder.HasKey(at => at.Id);

        builder.Property(at => at.Code)
            .IsRequired()
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(at => at.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(at => at.Code).IsUnique();
        
        // Тестовые данные
        builder.HasData(
            new AttachmentType { Id = 1, Code = "IMAGE", Name = "Изображение" },
            new AttachmentType { Id = 2, Code = "PDF", Name = "PDF-документ" },
            new AttachmentType { Id = 3, Code = "LOG", Name = "Лог-файл" }
        );
    }
}