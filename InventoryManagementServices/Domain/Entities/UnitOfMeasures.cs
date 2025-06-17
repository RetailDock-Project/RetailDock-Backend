using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class UnitOfMeasures
    {
        [Key]
        public int Id { get; set; }
        public Guid? OrgnaisationId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Measurement { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
