using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{

    public class ProductCategory
    {
        [Key]
        public int ProductCategoryId { get; set; }

        public Guid? OrgnaisationId { get; set; }


        [Required]
        [MaxLength(100)]
        public string ProductCategoryName { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
