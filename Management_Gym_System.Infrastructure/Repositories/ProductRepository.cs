using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;
using Management_Gym_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(long id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ID == id);
    }

    public async Task<List<Product>> GetFilteredProductsAsync(long? categoryId, string? keyword)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .AsQueryable();

        if (categoryId.HasValue && categoryId.Value > 0)
        {
            query = query.Where(p => p.CategoryID == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var normalizedKeyword = StringHelper.NormalizeText(keyword);
            query = query.Where(p =>
                EF.Functions.ILike(
                    EF.Functions.Unaccent(p.ProductName ?? string.Empty),
                    $"%{normalizedKeyword}%"));
        }

        return await query.OrderBy(p => p.ID).ToListAsync();
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Products.AnyAsync(p => p.ID == id);
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}