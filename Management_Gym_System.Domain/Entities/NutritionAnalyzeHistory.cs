using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Domain.Entities;
public class NutritionAnalyzeHistory
{
    public long Id {get; set;}
    public long? MemberId { get; set; }

    public string? ImageUrl { get; set; } = string.Empty;

    public string? FoodName { get; set; } = string.Empty;

    public decimal? EstimatedWeight { get; set; }

    public string? WeightUnit { get; set; } = "g";

    public decimal? EstimatedCalories { get; set; }

    public decimal? EstimatedProtein { get; set; }

    public decimal? EstimatedCarb { get; set; }

    public decimal? EstimatedFat { get; set; }

    public string? AnalyzeResult { get; set; } = string.Empty;

    //80%
    public decimal? Confidence { get; set; }

    public virtual User Member { get; set; } = new User();
}