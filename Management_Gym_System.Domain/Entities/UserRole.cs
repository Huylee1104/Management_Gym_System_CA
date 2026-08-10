using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Management_Gym_System.Domain.Entities;

public class UserRole
{
    [Key]
    public long ID { get; set; }

    [Required]
    [StringLength(50)]
    public string? RoleName { get; set; } = string.Empty;

    public bool? Status { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
