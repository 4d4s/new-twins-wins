using System.Net;
using TwinsWins.Core.Enums;

namespace TwinsWins.Core.Entities;

public class AuditLog : BaseEntity
{
    public Guid? UserId { get; set; }
    public User? User { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string? Details { get; set; } // JSON
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public AuditLevel Level { get; set; } = AuditLevel.INFO;
}
