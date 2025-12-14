using EscalationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscalationService.Infrastructure.Data.Configurations;

public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.FilePath)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(a => a.FileSize)
            .IsRequired();

        builder.Property(a => a.UploadedAt)
            .IsRequired();

        builder.HasOne(a => a.Escalation)
            .WithMany() 
            .HasForeignKey(a => a.EscalationId)
            .OnDelete(DeleteBehavior.SetNull); 

        builder.HasOne(a => a.AttachmentType)
            .WithMany(at => at.Attachments)
            .HasForeignKey(a => a.AttachmentTypeId)
            .OnDelete(DeleteBehavior.Restrict); 
        
        
    }
}