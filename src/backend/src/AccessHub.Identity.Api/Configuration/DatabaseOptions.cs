using System.ComponentModel.DataAnnotations;

namespace AccessHub.Identity.Api.Configuration;

public sealed class DatabaseOptions
{
  public const string SectionName = "Database";

  [Required]
  public string ConnectionString { get; init; } = string.Empty;
}
