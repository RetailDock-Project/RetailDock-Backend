using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.DTO
{
    public  class AddParentGroupDTO
    {




        [Required(ErrorMessage = "GroupName is required.")]
        [RegularExpression(@"\S+", ErrorMessage = "GroupName cannot be just whitespace.")]
        public string GroupName { get; set; }
        [Required]
        public Guid AccountsMasterGroupId { get; set; }
        
        
       
       
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        [JsonIgnore]
        public string Nature {  get; set; }


    }
}
