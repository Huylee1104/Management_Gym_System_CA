using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_Gym_System.Domain.Entities;

public class FinancialTransaction
{
    [Key]
    public long ID { get; set; }

    public long? CustomerID { get; set; }
    public long? StaffID { get; set; }

    public DateTime? TransactionDate { get; set; }

    // 1 = Income, 0 = Expense
    public bool? TransactionType { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TotalAmount { get; set; }

    [StringLength(500)]
    public string? Note { get; set; }

    // Navigation properties
    public User? Customer { get; set; } = null!;
    public User? Staff { get; set; } = null!;

    public ICollection<TransactionDetail> TransactionDetails { get; set; } = new List<TransactionDetail>();
    public ICollection<ImportReceipt> ImportReceipts { get; set; } = new List<ImportReceipt>();
}
