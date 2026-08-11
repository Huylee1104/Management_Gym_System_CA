using System.ComponentModel.DataAnnotations;

namespace Management_Gym_System.Domain.Entities;

public class SystemFunctionAction
{
    [Key]
    public long Id { get; set; }

    public long FunctionId { get; set; }

    [Required]
    [StringLength(100)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ActionName { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public int? DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual SystemFunction Function { get; set; } = null!;

    public virtual ICollection<RolePermission> RolePermissions { get; set; }
        = new List<RolePermission>();
}