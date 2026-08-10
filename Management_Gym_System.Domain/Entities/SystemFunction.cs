namespace Management_Gym_System.Domain.Entities;

public class SystemFunction
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty; // Ví dụ: QL_PRODUCT, QL_USER
    public string Name { get; set; } = string.Empty;
    public string? Controller { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public int? DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}