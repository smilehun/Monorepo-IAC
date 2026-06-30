using AccessHub.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessHub.Identity.Infrastructure.Persistence.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
  public void Configure(EntityTypeBuilder<AuditLog> builder)
  {
    builder.ToTable("audit_logs");

    builder.HasKey(auditLog => auditLog.Id);

    builder.Property(auditLog => auditLog.Action)
      .HasMaxLength(150)
      .IsRequired();

    builder.Property(auditLog => auditLog.Resource)
      .HasMaxLength(200)
      .IsRequired();

    builder.Property(auditLog => auditLog.IpAddress)
      .HasMaxLength(64)
      .IsRequired();

    builder.Property(auditLog => auditLog.Result)
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(auditLog => auditLog.CreatedAtUtc)
      .IsRequired();
  }
}
