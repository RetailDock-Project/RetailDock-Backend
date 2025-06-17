using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class AddSubGroupDTO
    {
        [Required(ErrorMessage = "GroupName is required.")]
        [RegularExpression(@"\S+", ErrorMessage = "GroupName cannot be just whitespace.")]
        public string GroupName { get; set; }

        [Required(ErrorMessage = "ParentId is required.")]
        public Guid ParentId { get; set; }

        
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
    }

}
