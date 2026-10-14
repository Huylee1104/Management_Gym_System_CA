namespace Management_Gym_System.Application.DTOs.MemberAdvise;
public class RequestMemberAdvise
{
    public long? ProductId { get; set; }
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}

public class AdviseFilterRequest
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Status { get; set; }
    public int? ProductId { get; set; }
    public string? ProductName { get; set; }
    public int? StaffId { get; set; }
    public string? StaffName { get; set; }
    public string? Keyword { get; set; }
}

public class AdviseCustomerDto
{
    public long Id { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? CustomerName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? ProductName { get; set; }
    public DateTime? LastContactDate { get; set; }
    public int ContactCount { get; set; }
    public bool IsRegistered { get; set; }
    public string? StaffName { get; set; }
}