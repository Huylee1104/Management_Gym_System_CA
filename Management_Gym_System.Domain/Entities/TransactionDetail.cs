using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_Gym_System.Domain.Entities;

public class TransactionDetail
{
    [Key]
    public long ID { get; set; }

    public long? TransactionID { get; set; }
    public long? ProductID { get; set; }

    public int? Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? DiscountRare { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TotalDiscount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? VATRare { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TotalVAT { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? SubTotal { get; set; }

    // Navigation properties
    [ForeignKey("TransactionID")]
    public FinancialTransaction? FinancialTransaction { get; set; } = null!;

    [ForeignKey("ProductID")]
    public Product? Product { get; set; } = null!;
}
