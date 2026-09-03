public class ActivityLogRequest
{
    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public long? StaffId { get; set; }

    public string? Action { get; set; }

    public string? Keyword { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}

public class ActivityLogResponse
{
    public List<ActivityLogDto> Items { get; set; } = new();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalItems { get; set; }

    public int TotalPages { get; set; }
}

public class ActivityLogDto
{
    public long Id { get; set; }

    public DateTime Date { get; set; }

    public string? StaffName { get; set; }

    public string Action { get; set; } = string.Empty;

    public string? MemberName { get; set; }

    public string? Note { get; set; }
}