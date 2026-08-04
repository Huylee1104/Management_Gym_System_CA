using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_Gym_System.Domain.Entities;

public class Product
{
    [Key]
    public long ID { get; set; }

    public long? CategoryID { get; set; }

    [Required]
    [StringLength(200)]
    public string? ProductName { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Price { get; set; }

    [Required]
    [StringLength(50)]
    public string? Unit { get; set; } = string.Empty;

    public int? ThoiHan { get; set; } // Thời hạn (ngày) chỉ áp dụng cho gói tập

    public bool? Status { get; set; }

    public string? ImageProduct { get; set; }

    // Navigation properties
    [ForeignKey("CategoryID")]
    public ProductCategory? Category { get; set; } = null!;

    public ICollection<GymMembershipCard> GymMembershipCards { get; set; } = new List<GymMembershipCard>();
    public ICollection<TransactionDetail> TransactionDetails { get; set; } = new List<TransactionDetail>();
    public ICollection<ImportReceiptDetail> ImportReceiptDetails { get; set; } = new List<ImportReceiptDetail>();
    public ICollection<ExportReceiptDetail> ExportReceiptDetails { get; set; } = new List<ExportReceiptDetail>();
    public ICollection<InventoryLot> InventoryLots { get; set; } = new List<InventoryLot>();
}
