using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_Gym_System.Domain.Entities;

public class ExportReceipt
{
    [Key]
    public long ID { get; set; }

    public long? StaffID { get; set; }

    public DateTime? ExportDate { get; set; }

    [StringLength(500)]
    public string? Note { get; set; }

    // Navigation properties
    [ForeignKey("StaffID")]
    public User? Staff { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TotalAmount { get; set; }

    public ICollection<ExportReceiptDetail> ExportReceiptDetails { get; set; } = new List<ExportReceiptDetail>();
}
