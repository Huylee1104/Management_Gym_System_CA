public interface IProductService
{
    Task<List<ProductDto>> GetProductsAsync(long? categoryId, string? keyword);
    Task<ProductDto> CreateAsync(CreateProductRequest dto);
    Task<bool> UpdateAsync(long id, UpdateProductRequest dto);
    Task<bool> ToggleStatusAsync(long id);
    Task<bool> DeleteAsync(long id);
}