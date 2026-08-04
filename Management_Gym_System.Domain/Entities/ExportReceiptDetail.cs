using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_Gym_System.Domain.Entities;

public class ExportReceiptDetail
{
    [Key]
    public long ID { get; set; }

    public long? ExportReceiptID { get; set; }

    public long? ProductID { get; set; }

    public int? Quantity { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? Discount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TotalDiscount { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? TaxRate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TotalTax { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? ExportPrice { get; set; }

    // Navigation properties
    [ForeignKey("ExportReceiptID")]
    public ExportReceipt? ExportReceipt { get; set; } = null!;

    [ForeignKey("ProductID")]
    public Product? Product { get; set; } = null!;
}
