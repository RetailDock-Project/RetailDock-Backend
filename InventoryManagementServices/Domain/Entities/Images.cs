using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Images
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Product")]
        public Guid ProductId { get; set; }

        [Required]
        [Column(TypeName = "MEDIUMBLOB")]
        public byte[] ImageData { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; }

        [Required]
        [MaxLength(100)]
        public string ContentType { get; set; }
        public DateTime CreatedAt { get; set; }

        public Product Product { get; set; }
    }
}
