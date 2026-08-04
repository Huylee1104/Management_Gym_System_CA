using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Domain.Interfaces;

public interface IProductCategoryRepository
{
    Task<List<ProductCategory>> GetCategoriesAsync(string? keyword);
    Task<ProductCategory?> GetByIdAsync(long id);
    Task AddAsync(ProductCategory category);
    void Update(ProductCategory category);
    void Delete(ProductCategory category);
    Task<bool> SaveChangesAsync();
}