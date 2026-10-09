using System.Globalization;
using Management_Gym_System.Application.DTOs.MemberAdvise;
using Management_Gym_System.Application.Services;
using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

public class MemberAdviseService : IMemberAdviseService
{
    private readonly IMemberAdviseRepository _memberAdviseRepo;
    private readonly IProductRepository _productRepo;
    public MemberAdviseService(IMemberAdviseRepository memberAdviseRepo, IProductRepository productRepo)
    {
        _memberAdviseRepo = memberAdviseRepo;
        _productRepo = productRepo;
    }

    public async Task<List<MemberAdvise>> GetListMemberAdviseAsync(DateTime? fromDate, DateTime? toDate, string? fullName)
    {
        var memberAdvises = await _memberAdviseRepo.GetListAdviseAsync(fromDate, toDate);

        if (!string.IsNullOrWhiteSpace(fullName))
        {
            var compareInfo = CultureInfo.GetCultureInfo("vi-VN").CompareInfo;

            memberAdvises = memberAdvises.Where(x =>
                !string.IsNullOrWhiteSpace(x.FullName) &&
                compareInfo.IndexOf(
                    x.FullName,
                    fullName,
                    CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace
                ) >= 0
            ).ToList();
        }

        return memberAdvises;
    }

    public async Task<List<MemberAdvise>> GetListMemberAdviseContactedAsync(DateTime? fromDate, DateTime? toDate, string? fullName)
    {
        var memberAdvises = await _memberAdviseRepo.GetListAdviseContactedAsync(fromDate, toDate);

        if (!string.IsNullOrWhiteSpace(fullName))
        {
            var compareInfo = CultureInfo.GetCultureInfo("vi-VN").CompareInfo;

            memberAdvises = memberAdvises.Where(x =>
                !string.IsNullOrWhiteSpace(x.FullName) &&
                compareInfo.IndexOf(
                    x.FullName,
                    fullName,
                    CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace
                ) >= 0
            ).ToList();
        }

        return memberAdvises;
    }

    public async Task<ServiceResult> UpdateMemberAdviseAsync(MemberAdvise memberAdvise)
    {
        try
        {
            if (memberAdvise.Id > 0)
            {
                var existingMemberAdvise = await _memberAdviseRepo.GetByIdAsync(memberAdvise.Id);

                if (existingMemberAdvise == null)
                {
                    return ServiceResult.Failure("Không tìm thấy thông tin tư vấn.");
                }

                existingMemberAdvise.FullName = memberAdvise.FullName;
                existingMemberAdvise.PhoneNumber = memberAdvise.PhoneNumber;
                existingMemberAdvise.Email = memberAdvise.Email;
                existingMemberAdvise.IsContacted = memberAdvise.IsContacted;
                existingMemberAdvise.ContactedDate = memberAdvise.ContactedDate;
                existingMemberAdvise.ContactedCount = memberAdvise.ContactedCount;

                await _memberAdviseRepo.UpdateAsync(existingMemberAdvise);
            }
            await _memberAdviseRepo.SaveChangesAsync();
            return ServiceResult.Success("Thành công!");
        }
        catch (Exception ex)
        {
            return ServiceResult.Failure($"Tạo thẻ thất bại: {ex.Message}");
        }
    }

    public async Task<ServiceResult> AddMemberAdviseAsync(RequestMemberAdvise requestMemberAdvise)
    {
        try
        {
            var newMemberAdvise = new MemberAdvise
            {
                ProductId = requestMemberAdvise.ProductId,
                Date = DateTime.Now,
                FullName = requestMemberAdvise.FullName,
                PhoneNumber = requestMemberAdvise.PhoneNumber,
                Email = requestMemberAdvise.Email,
                IsContacted = false,
                ContactedDate = null,
                ContactedCount = 0
            };

            await _memberAdviseRepo.AddAsync(newMemberAdvise);
            await _memberAdviseRepo.SaveChangesAsync();

            return ServiceResult.Success("Thêm thông tin tư vấn thành công!");
        }
        catch (Exception ex)
        {
            return ServiceResult.Failure($"Thêm thông tin tư vấn thất bại: {ex.Message}");
        }
    }

    public async Task<ServiceResult> ReviewProductAsync(long ProductId, string Review)
    {
        var product = await _productRepo.GetByIdAsync(ProductId);
        if (product == null)
        {
            return ServiceResult.Failure("Không tìm thấy sản phẩm.");
        }

        product.Review = product.Review + Review;
        await _productRepo.UpdateAsync(product);
        await _productRepo.SaveChangesAsync();
        return ServiceResult.Success("Đánh giá sản phẩm thành công!");
    }
}