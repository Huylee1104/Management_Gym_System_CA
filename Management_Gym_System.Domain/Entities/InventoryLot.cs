using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_Gym_System.Domain.Entities;

public class InventoryLot
{
    [Key]
    public long Id { get; set; }

    public long ProductId { get; set; }

    //public long WarehouseId { get; set; }

    public long ImportDetailId { get; set; }

    [StringLength(100)]
    public string? BatchCode { get; set; }

    public DateTime? ExpiryDate { get; set; } // Ngày hết hạn

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitCost { get; set; }

    public int OriginalQuantity { get; set; } // Số lượng ban đầu khi nhập kho

    public int CurrentQuantity { get; set; } // Số lượng kho còn lại

    public int ReservedQuantity { get; set; } // Số lượng đã được đặt hàng nhưng chưa xuất kho

    [StringLength(50)]
    public string Status { get; set; } = "active"; // active/expired/out

    public DateTime CreatedAt { get; set; } = DateTime.Now; // Ngày tạo lô hàng

    // Navigation properties
    public virtual Product? Product { get; set; }

    //public virtual Warehouse? Warehouse { get; set; }

    public virtual ImportReceiptDetail? ImportDetail { get; set; }
}
