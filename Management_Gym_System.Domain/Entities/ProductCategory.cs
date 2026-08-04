using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Management_Gym_System.Domain.Entities;
    public class ProductCategory
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(100)]
        public string? CategoryName { get; set; } = string.Empty;

        public bool? Status { get; set; }

        // Navigation property
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }