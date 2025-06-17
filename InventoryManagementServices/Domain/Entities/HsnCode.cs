using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class HsnCode
    {
        [Key]
       public int HsnCodeId { get; set; }
        public int HSNCodeNumber { get; set; }
        public Guid? OrgnaisationId { get; set; }

        [Required(ErrorMessage = "GST rate is required.")]
        [Range(0, 100, ErrorMessage = "GST rate must be between 0 and 100.")]
        [Column(TypeName = "decimal(5,2)")] 
        public decimal GstRate { get; set; }
        public bool IsDeleted {  get; set; }

       
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
