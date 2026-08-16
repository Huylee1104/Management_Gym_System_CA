namespace Management_Gym_System.Application.DTOs.SystemFunction;

public class SystemFunctionDto
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Controller { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int? DisplayOrder { get; set; }

    public List<SystemFunctionActionDto> Actions { get; set; }
        = new();
}

public class SystemFunctionActionDto
{
    public long Id { get; set; }
    public long FunctionId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int? DisplayOrder { get; set; }
}