using Management_Gym_System.Application.Services;
using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class NutritionAnalyzeHistoryService : INutritionAnalyzeHistoryService
{
    private readonly INutritionAnalyzeHistoryRepository _nutritionAnalyzeHistoryRepository;
    public NutritionAnalyzeHistoryService(INutritionAnalyzeHistoryRepository nutritionAnalyzeHistoryRepository)
    {
        _nutritionAnalyzeHistoryRepository = nutritionAnalyzeHistoryRepository;
    }
}