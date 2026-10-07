using System.Globalization;
using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepo;
    private readonly IMemoryCache _cache;

    public ProductService(IProductRepository productRepo, IMemoryCache cache)
    {
        _productRepo = productRepo;
        _cache = cache;
    }

    private const string PRODUCT_CACHE_KEY = "PRODUCT_LIST";


    public async Task<List<ProductDto>> GetProductsAsync(long? categoryId, string? keyword)
    {
        if (!_cache.TryGetValue(PRODUCT_CACHE_KEY, out List<ProductDto>? cachedProducts))
        {
            var products = await _productRepo.GetFilteredProductsAsync();
            cachedProducts = products.Select(p => new ProductDto
            {
                Id = p.ID,
                ProductName = p?.ProductName,
                Price = p?.Price,
                Unit = p?.Unit ?? string.Empty,
                CategoryName = p?.Category?.CategoryName ?? string.Empty,
                CategoryId = p?.CategoryID,
                ThoiHan = p?.ThoiHan,
                Status = p?.Status,
                ImageProduct = p?.ImageProduct,
                Description = p?.Description,
                Review = p?.Review
            }).ToList();
            _cache.Set(PRODUCT_CACHE_KEY, cachedProducts);
        }

        if (categoryId.HasValue && categoryId.Value > 0 && cachedProducts != null)
        {
            cachedProducts = cachedProducts.Where(x =>
                x.CategoryId == categoryId.Value
            ).ToList();
        }

        if (!string.IsNullOrWhiteSpace(keyword) && cachedProducts != null)
        {
            var compareInfo = CultureInfo.GetCultureInfo("vi-VN").CompareInfo;

            cachedProducts = cachedProducts.Where(x =>
                !string.IsNullOrWhiteSpace(x.ProductName) &&
                compareInfo.IndexOf(
                    x.ProductName,
                    keyword,
                    CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace
                ) >= 0
            ).ToList();
        }

        return cachedProducts ?? new List<ProductDto>();
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
            ImageProduct = dto.ImageProduct,
            Description = dto.Description
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
        existing.Description = dto.Description;
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