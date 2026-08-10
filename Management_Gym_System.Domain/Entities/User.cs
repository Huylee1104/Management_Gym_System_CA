using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_Gym_System.Domain.Entities;

public class User
{
    [Key]
    public long ID { get; set; }

    public long? RoleID { get; set; }

    [Required]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    // Bổ sung PasswordHash lưu mật khẩu đã mã hóa
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string? FullName { get; set; } = string.Empty;

    public string? Avatar { get; set; }

    [StringLength(15)]
    public string? PhoneNumber { get; set; }

    public bool? Status { get; set; }

    // Navigation properties
    [ForeignKey("RoleID")]
    public UserRole? Role { get; set; } = null!;

    public ICollection<GymMembershipCard> Memberships { get; set; } = new List<GymMembershipCard>();
    public ICollection<FinancialTransaction> CustomerTransactions { get; set; } = new List<FinancialTransaction>();
    public ICollection<FinancialTransaction> StaffTransactions { get; set; } = new List<FinancialTransaction>();
    public ICollection<ImportReceipt> ImportReceipts { get; set; } = new List<ImportReceipt>();
    public ICollection<ExportReceipt> ExportReceipts { get; set; } = new List<ExportReceipt>();
}
