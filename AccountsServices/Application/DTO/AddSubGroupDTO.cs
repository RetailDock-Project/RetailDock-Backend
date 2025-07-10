using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class AddSubGroupDTO
    {
        [Required(ErrorMessage = "GroupName is required.")]
      
        public string GroupName { get; set; }

        [Required(ErrorMessage = "ParentId is required.")]
        public Guid ParentId { get; set; }

        [JsonIgnore]
        public Guid CreatedBy { get; set; }
     
        [JsonIgnore] 
        public string? Nature {  get; set; }
    }

}
