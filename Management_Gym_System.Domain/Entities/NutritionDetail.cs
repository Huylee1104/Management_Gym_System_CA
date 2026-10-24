using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Domain.Entities;
public class NutritionDetail
{
    public long Id {get; set;}
    public long? NutritionGroupId { get; set; }

    public string? Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public decimal? Calories { get; set; }

    public decimal? Protein { get; set; }

    public decimal? Carb { get; set; }

    public decimal? Fat { get; set; }

    public decimal? Fiber { get; set; }

    public decimal? Sugar { get; set; }

    public decimal? Sodium { get; set; }

    public decimal? ServingSize { get; set; }

    public string? ServingUnit { get; set; } = "g";

    public string? RecommendedTime { get; set; }

    public string? RecommendedTarget { get; set; }

    public string? Pros { get; set; }

    public string? Cons { get; set; }

    public bool? IsPublished { get; set; }

    public virtual NutritionGroup? NutritionGroup { get; set; }
}