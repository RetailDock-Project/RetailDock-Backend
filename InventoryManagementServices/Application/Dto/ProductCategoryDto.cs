using System.ComponentModel.DataAnnotations;

namespace Application.Dto
{
    public class ProductCategoryDto
    {
        [Required]
        [MaxLength(100)]
        public string ProductCategoryName { get; set; }
        public Guid? OrgnaisationId { get; set; }
    }
    public class GetProductCategoryDto
    {
        public int ProductCategoryId { get; set; }
        public Guid? OrgnaisationId { get; set; }
        [Required]
        [MaxLength(100)]
        public string ProductCategoryName { get; set; }
    }
}
