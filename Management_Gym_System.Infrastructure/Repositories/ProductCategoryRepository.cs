using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;
using Management_Gym_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Management_Gym_System.Infrastructure.Repositories;

public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly ApplicationDbContext _context;

    public ProductCategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductCategory>> GetCategoriesAsync(string? keyword)
    {
        var query = _context.ProductCategories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            // Logic EF Core (ILike, Unaccent) được cô lập hoàn toàn ở đây!
            query = query.Where(c =>
                EF.Functions.ILike(
                    EF.Functions.Unaccent(c.CategoryName!),
                    $"%{keyword}%"));
        }

        return await query.OrderBy(c => c.ID).ToListAsync();
    }

    public async Task<ProductCategory?> GetByIdAsync(long id)
    {
        return await _context.ProductCategories.FindAsync(id);
    }

    public async Task AddAsync(ProductCategory category)
    {
        await _context.ProductCategories.AddAsync(category);
    }

    public void Update(ProductCategory category)
    {
        _context.ProductCategories.Update(category);
    }

    public void Delete(ProductCategory category)
    {
        _context.ProductCategories.Remove(category);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}