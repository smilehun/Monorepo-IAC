using AccessHub.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessHub.Identity.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
  public void Configure(EntityTypeBuilder<RefreshToken> builder)
  {
    builder.ToTable("refresh_tokens");

    builder.HasKey(refreshToken => refreshToken.Id);

    builder.Property(refreshToken => refreshToken.TokenHash)
      .HasMaxLength(500)
      .IsRequired();

    builder.Property(refreshToken => refreshToken.CreatedAtUtc)
      .IsRequired();

    builder.Property(refreshToken => refreshToken.ExpiresAtUtc)
      .IsRequired();

    builder.HasIndex(refreshToken => refreshToken.TokenHash)
      .IsUnique();

    builder.HasOne(refreshToken => refreshToken.User)
      .WithMany(user => user.RefreshTokens)
      .HasForeignKey(refreshToken => refreshToken.UserId);

    builder.HasOne(refreshToken => refreshToken.ReplacedByToken)
      .WithMany()
      .HasForeignKey(refreshToken => refreshToken.ReplacedByTokenId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
