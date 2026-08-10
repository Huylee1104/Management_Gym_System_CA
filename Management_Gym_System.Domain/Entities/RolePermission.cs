namespace Management_Gym_System.Domain.Entities;

public class RolePermission
{
    public long Id { get; set; }
    
    public long RoleId { get; set; }
    public virtual UserRole Role { get; set; } = null!;

    public long? FunctionId { get; set; }
    public virtual SystemFunction Function { get; set; } = null!;

    public bool CanView { get; set; }
    public bool CanCreate { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanExport { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}