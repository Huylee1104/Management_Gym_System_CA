using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_Gym_System.Domain.Entities;

public class ImportReceiptDetail
{
    [Key]
    public long ID { get; set; }

    public long? ImportReceiptID { get; set; }
    public long? ProductID { get; set; }

    public int? Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? ImportPrice { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? Discount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TotalDiscount { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? TaxRate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TotalTax { get; set; }

    public string? BatchCode { get; set; } // Số lô hàng

    public DateTime? ExpiryDate { get; set; } // Ngày hết hạn

    // Navigation properties
    [ForeignKey("ImportReceiptID")]
    public ImportReceipt? ImportReceipt { get; set; } = null!;

    [ForeignKey("ProductID")]
    public Product? Product { get; set; } = null!;
}

