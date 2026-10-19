using Management_Gym_System.Application.Services;
using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class NutritionDetailService : INutritionDetailService
{
    private readonly INutritionDetailRepository _nutritionDetailRepository;
    public NutritionDetailService(INutritionDetailRepository nutritionDetailRepository)
    {
        _nutritionDetailRepository = nutritionDetailRepository;
    }

    public async Task<List<NutritionDetail>> GetDetailsByGroupIdAsync(long groupId, bool isAdmin)
    {
        var query = await _nutritionDetailRepository.GetDetailsByGroupIdAsync(groupId, isAdmin);

        return query;
    }

    public async Task<NutritionDetail?> GetDetailByIdAsync(long id)
    {
        NutritionDetail? detail = await _nutritionDetailRepository.GetDetailByIdAsync(id);

        return detail;
    }

    public async Task<NutritionDetail> CreateDetailAsync(NutritionDetail model)
    {
        await _nutritionDetailRepository.AddAsync(model);
        await _nutritionDetailRepository.SaveChangesAsync();
        return model;
    }

    public async Task<bool> UpdateDetailAsync(NutritionDetail model)
    {
        NutritionDetail? existingDetail = await _nutritionDetailRepository.GetDetailByIdAsync(model.Id);

        if (existingDetail == null)
        {
            return false;
        }

        existingDetail.NutritionGroupId = model.NutritionGroupId;
        existingDetail.Name = model.Name;
        existingDetail.Description = model.Description;
        existingDetail.Calories = model.Calories;
        existingDetail.Protein = model.Protein;
        existingDetail.Carb = model.Carb;
        existingDetail.Fat = model.Fat;
        existingDetail.Fiber = model.Fiber;
        existingDetail.Sugar = model.Sugar;
        existingDetail.Sodium = model.Sodium;
        existingDetail.ServingSize = model.ServingSize;
        existingDetail.ServingUnit = string.IsNullOrWhiteSpace(model.ServingUnit) ? "g" : model.ServingUnit;
        existingDetail.RecommendedTime = model.RecommendedTime;
        existingDetail.RecommendedTarget = model.RecommendedTarget;
        existingDetail.Pros = model.Pros;
        existingDetail.Cons = model.Cons;
        existingDetail.IsPublished = model.IsPublished;

        if (!string.IsNullOrEmpty(model.ImageUrl))
        {
            existingDetail.ImageUrl = model.ImageUrl;
        }

        await _nutritionDetailRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteDetailAsync(long id)
    {
        NutritionDetail? existingDetail = await _nutritionDetailRepository.GetDetailByIdAsync(id);

        if (existingDetail == null)
        {
            return false;
        }

        await _nutritionDetailRepository.DeleteAsync(existingDetail);
        await _nutritionDetailRepository.SaveChangesAsync();
        return true;
    }
}