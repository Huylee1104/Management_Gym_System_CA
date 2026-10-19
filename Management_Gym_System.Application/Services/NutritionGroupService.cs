using Management_Gym_System.Application.Services;
using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class NutritionGroupService : INutritionGroupService
{
    private readonly INutritionGroupRepository _nutritionGroupRepository;
    public NutritionGroupService(INutritionGroupRepository nutritionGroupRepository)
    {
        _nutritionGroupRepository = nutritionGroupRepository;
    }

    public async Task<IReadOnlyList<NutritionGroup>> GetAllGroupsAsync(bool isAdmin)
    {
        var query = await _nutritionGroupRepository.GetAllGroupsAsync(isAdmin);
        return query;
    }

    public async Task<NutritionGroup?> GetGroupByIdAsync(long id)
    {
        NutritionGroup? group = await _nutritionGroupRepository.GetGroupByIdAsync(id);

        return group;
    }

    public async Task<NutritionGroup> CreateGroupAsync(NutritionGroup model)
    {
        await _nutritionGroupRepository.AddAsync(model);
        await _nutritionGroupRepository.SaveChangesAsync();
        return model;
    }

    public async Task<bool> UpdateGroupAsync(NutritionGroup model)
    {
        NutritionGroup? existingGroup = await _nutritionGroupRepository.GetGroupByIdAsync(model.Id);

        if (existingGroup == null)
        {
            return false;
        }

        existingGroup.Name = model.Name;
        existingGroup.Description = model.Description;
        existingGroup.SortOrder = model.SortOrder;
        existingGroup.IsActive = model.IsActive;

        if (!string.IsNullOrEmpty(model.ImageUrl))
        {
            existingGroup.ImageUrl = model.ImageUrl;
        }

        await _nutritionGroupRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteGroupAsync(long id)
    {
        NutritionGroup? existingGroup = await _nutritionGroupRepository.GetGroupByIdAsync(id);


        if (existingGroup == null)
        {
            return false;
        }

        // Xóa các thực phẩm con trước khi xóa nhóm (hoặc bạn có thể đổi thành chặn xóa nếu có thực phẩm con)
        if (existingGroup.NutritionDetails != null && existingGroup.NutritionDetails.Count > 0)
        {
            await _nutritionGroupRepository.DeleteRangeAsync(existingGroup.NutritionDetails);
        }

        await _nutritionGroupRepository.DeleteAsync(existingGroup);
        await _nutritionGroupRepository.SaveChangesAsync();
        return true;
    }
}