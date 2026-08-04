public class ProductDto
{
    public long Id { get; set; }
    public string? ProductName { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string? Unit { get; set; }
    public string? CategoryName { get; set; } = string.Empty;
    public long? CategoryId { get; set; }
    public int? ThoiHan { get; set; }
    public bool? Status { get; set; }
    public string? ImageProduct { get; set; }
}

public class CreateProductRequest
{
    public string? ProductName { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string? Unit { get; set; }
    public long? CategoryId { get; set; }
    public int? ThoiHan { get; set; }
    public bool? Status { get; set; }
    public string? ImageProduct { get; set; }
}

public class UpdateProductRequest : CreateProductRequest { }