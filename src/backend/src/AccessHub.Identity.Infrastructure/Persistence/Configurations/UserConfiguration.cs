using AccessHub.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessHub.Identity.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder.ToTable("users");

    builder.HasKey(user => user.Id);

    builder.Property(user => user.Email)
      .HasMaxLength(320)
      .IsRequired();

    builder.Property(user => user.Username)
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(user => user.PasswordHash)
      .HasMaxLength(500)
      .IsRequired();

    builder.Property(user => user.CreatedAtUtc)
      .IsRequired();

    builder.Property(user => user.UpdatedAtUtc)
      .IsRequired();

    builder.HasIndex(user => user.Email)
      .IsUnique();

    builder.HasIndex(user => user.Username)
      .IsUnique();
  }
}
