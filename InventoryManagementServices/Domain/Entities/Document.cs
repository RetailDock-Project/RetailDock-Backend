using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Document
    {
        [Key]
        public Guid Id { get; set; }


        [Required]
        [Column(TypeName = "MEDIUMBLOB")]
        public byte[] DocumentData { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; }
        [Required]
        public string FileNote { get; set; }

        [Required]
        [MaxLength(100)]
        public string ContentType { get; set; }
        public DateTime CreatedAt { get; set; }=DateTime.UtcNow;
    }
}
