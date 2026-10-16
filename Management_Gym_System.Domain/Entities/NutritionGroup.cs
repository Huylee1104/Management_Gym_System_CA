using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Domain.Entities;
public class NutritionGroup
{
    public long Id {get; set;}
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public int? SortOrder { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<NutritionDetail> NutritionDetails { get; set; }
        = new List<NutritionDetail>();
}