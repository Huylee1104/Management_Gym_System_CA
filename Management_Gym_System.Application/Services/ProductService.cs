using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepo;

    public ProductService(IProductRepository productRepo)
    {
        _productRepo = productRepo;
    }

    public async Task<List<ProductDto>> GetProductsAsync(long? categoryId, string? keyword)
    {
        var products = await _productRepo.GetFilteredProductsAsync(categoryId, keyword);
        return products.Select(p => new ProductDto
        {
            Id = p.ID,
            ProductName = p?.ProductName,
            Price = p?.Price,
            Unit = p?.Unit ?? string.Empty,
            CategoryName = p?.Category?.CategoryName ?? string.Empty,
            CategoryId = p?.CategoryID,
            ThoiHan = p?.ThoiHan,
            Status = p?.Status,
            ImageProduct = p?.ImageProduct
        }).ToList();
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest dto)
    {
        if (dto.Price <= 0) throw new ArgumentException("Giá sản phẩm phải lớn hơn 0.");

        var entity = new Product
        {
            ProductName = dto.ProductName,
            CategoryID = dto.CategoryId,
            Price = dto.Price,
            Unit = dto.Unit,
            ThoiHan = dto.ThoiHan,
            Status = dto.Status,
            ImageProduct = dto.ImageProduct
        };

        await _productRepo.AddAsync(entity);
        return new ProductDto { Id = entity.ID, ProductName = entity.ProductName };
    }

    public async Task<bool> UpdateAsync(long id, UpdateProductRequest dto)
    {
        if (dto.Price <= 0) throw new ArgumentException("Giá sản phẩm phải lớn hơn 0.");

        var existing = await _productRepo.GetByIdAsync(id);
        if (existing == null) return false;

        existing.ProductName = dto.ProductName;
        existing.CategoryID = dto.CategoryId;
        existing.Price = dto.Price;
        existing.Unit = dto.Unit;
        existing.ThoiHan = dto.ThoiHan;
        existing.Status = dto.Status;
        existing.ImageProduct = dto.ImageProduct;

        await _productRepo.UpdateAsync(existing);
        return true;
    }

    public async Task<bool> ToggleStatusAsync(long id)
    {
        var existing = await _productRepo.GetByIdAsync(id);
        if (existing == null) return false;

        existing.Status = !existing.Status;
        await _productRepo.UpdateAsync(existing);
        return true;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var existing = await _productRepo.GetByIdAsync(id);
        if (existing == null) return false;

        await _productRepo.DeleteAsync(existing);
        return true;
    }
}