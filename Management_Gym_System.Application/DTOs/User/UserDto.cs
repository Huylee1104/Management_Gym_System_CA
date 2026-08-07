public class UserDto
{
    public long Id { get; set; }

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public long? RoleID { get; set; }

    public string? RoleName { get; set; }

    public string? Avatar { get; set; }

    public bool? Status { get; set; }
    public long? GoiTapID { get; set; }
    public string? GoiTapName { get; set; }
}

public class UserCreateUpdateDto
{
    public long? Id { get; set; }

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public long? RoleID { get; set; }

    public long? GoiTapID { get; set; }

    public int? ThoiHan { get; set; }

    public string? Avatar { get; set; }

    public bool? Status { get; set; }
}