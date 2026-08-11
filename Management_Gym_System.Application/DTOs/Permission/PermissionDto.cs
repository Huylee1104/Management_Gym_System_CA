namespace Management_Gym_System.Application.DTOs.Permission;

public class PermissionItemRequest
{
    public long ActionId { get; set; }

    public bool IsAllowed { get; set; }
}

public class PermissionTreeResponse
{
    public long RoleId { get; set; }

    public List<PermissionFunctionDto> Functions { get; set; }
        = new();
}

public class PermissionFunctionDto
{
    public long Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public List<PermissionActionDto> Actions { get; set; }
        = new();
}

public class PermissionActionDto
{
    public long Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string ActionName { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public bool IsAllowed { get; set; }
}