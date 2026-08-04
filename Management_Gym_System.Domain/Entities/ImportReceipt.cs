using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_Gym_System.Domain.Entities;

public class ImportReceipt
{
    [Key]
    public long ID { get; set; }

    public long? StaffID { get; set; }

    public long? TransactionID { get; set; }

    public DateTime? ImportDate { get; set; }

    // Navigation properties
    [ForeignKey("StaffID")]
    public User? Staff { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TotalAmount { get; set; }

    public string? SupplySource { get; set; }

    [ForeignKey("TransactionID")]
    public FinancialTransaction? FinancialTransaction { get; set; }

    public ICollection<ImportReceiptDetail> ImportReceiptDetails { get; set; } = new List<ImportReceiptDetail>();
}
