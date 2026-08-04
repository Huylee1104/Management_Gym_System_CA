using Management_Gym_System.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Management_Gym_System.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<GymMembershipCard> GymMembershipCards { get; set; }
    public DbSet<FinancialTransaction> FinancialTransactions { get; set; }
    public DbSet<TransactionDetail> TransactionDetails { get; set; }
    public DbSet<ImportReceipt> ImportReceipts { get; set; }
    public DbSet<ImportReceiptDetail> ImportReceiptDetails { get; set; }
    public DbSet<ExportReceipt> ExportReceipts { get; set; }
    public DbSet<ExportReceiptDetail> ExportReceiptDetails { get; set; }
    public DbSet<Checkin> Checkins { get; set; }
    public DbSet<InventoryLot> InventoryLots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình các quan hệ đặc biệt để tránh lỗi Cascade Delete trong EF Core
        modelBuilder.Entity<FinancialTransaction>()
            .HasOne(f => f.Customer)
            .WithMany(u => u.CustomerTransactions)
            .HasForeignKey(f => f.CustomerID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FinancialTransaction>()
            .HasOne(f => f.Staff)
            .WithMany(u => u.StaffTransactions)
            .HasForeignKey(f => f.StaffID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ImportReceipt>()
            .HasOne(i => i.Staff)
            .WithMany(u => u.ImportReceipts)
            .HasForeignKey(i => i.StaffID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ExportReceipt>()
            .HasOne(e => e.Staff)
            .WithMany(u => u.ExportReceipts)
            .HasForeignKey(e => e.StaffID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Checkin>()
            .HasOne(c => c.Card)
            .WithMany(g => g.Checkins)
            .HasForeignKey(c => c.CardID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InventoryLot>()
            .HasOne(il => il.Product)
            .WithMany(p => p.InventoryLots)
            .HasForeignKey(il => il.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
