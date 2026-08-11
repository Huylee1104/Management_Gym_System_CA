using System.ComponentModel.DataAnnotations;

namespace Management_Gym_System.Domain.Entities;

public class RolePermission
{
    [Key]
    public long Id { get; set; }

    public long RoleId { get; set; }

    public long ActionId { get; set; }

    public bool IsAllowed { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public virtual UserRole Role { get; set; } = null!;

    public virtual SystemFunctionAction Action { get; set; } = null!;
}