namespace Management_Gym_System.Application.DTOs.Inventory;

public class ImportRequestDto
{
    public string Supplier { get; set; }
    public List<ImportDetailDto> Details { get; set; }
}

public class ImportDetailDto
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? BatchCode { get; set; }
    public string? ExpiryDate { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxRate { get; set; }
    public string? Unit { get; set; }
}

// TẢI LỊCH SỬ PHIẾU NHẬP KHO
public class ImportHistoryFilterDto
{
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }
    public int? ProductId { get; set; }
    public string? Supplier { get; set; }
}

public class ImportHistoryItemDto
{
    public long Id { get; set; }

    public string? BatchCode { get; set; }

    public string? ImportDate { get; set; }

    public string? StaffName { get; set; }

    public string? Supplier { get; set; }

    public decimal TotalAmount { get; set; }

    public bool IsCancelled { get; set; }
}