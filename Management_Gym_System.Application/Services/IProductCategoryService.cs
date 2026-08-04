using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Application.Services;

public interface IProductCategoryService
{
    Task<List<ProductCategory>> GetCategoriesAsync(string? keyword);
    Task<ProductCategory?> GetByIdAsync(long id);
    Task<bool> CreateCategoryAsync(ProductCategory category);
    Task<bool> UpdateCategoryAsync(long id, ProductCategory category);
    Task<bool> ToggleStatusAsync(long id);
    Task<bool> DeleteCategoryAsync(long id);
}