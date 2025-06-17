using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Dto
{
    public class UnitOfMeasureDto
    {
        //[Required]
        //public int Id { get; set; }
        [Required]
        public Guid? OrgnaisationId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Measurement { get; set; }
    }

    public class GetUnitOfMeasureDto
    {
        [Required]
        public int Id { get; set; }
        public Guid? OrgnaisationId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Measurement { get; set; }
    }
}
