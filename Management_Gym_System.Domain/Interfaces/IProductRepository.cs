using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Domain.Interfaces;

public interface IProductRepository
{
    // Truy vấn
    Task<Product?> GetByIdAsync(long id);
    Task<List<Product>> GetFilteredProductsAsync();
    Task<bool> ExistsAsync(long id);

    // Thao tác dữ liệu
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);
    Task SaveChangesAsync();
}