using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;

namespace Management_Gym_System.Application.Services;

public class ProductCategoryService : IProductCategoryService
{
    private readonly IProductCategoryRepository _repository;

    public ProductCategoryService(IProductCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProductCategory>> GetCategoriesAsync(string? keyword)
    {
        // Bạn có thể xử lý NormalizeText ở đây trước khi truyền vào Repository
        return await _repository.GetCategoriesAsync(keyword);
    }

    public async Task<ProductCategory?> GetByIdAsync(long id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<bool> CreateCategoryAsync(ProductCategory category)
    {
        await _repository.AddAsync(category);
        return await _repository.SaveChangesAsync();
    }

    public async Task<bool> UpdateCategoryAsync(long id, ProductCategory category)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;

        existing.CategoryName = category.CategoryName;
        existing.Status = category.Status;

        _repository.Update(existing);
        return await _repository.SaveChangesAsync();
    }

    public async Task<bool> ToggleStatusAsync(long id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;

        existing.Status = !existing.Status;
        _repository.Update(existing);
        return await _repository.SaveChangesAsync();
    }

    public async Task<bool> DeleteCategoryAsync(long id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null) return false;

        _repository.Delete(category);
        return await _repository.SaveChangesAsync();
    }
}