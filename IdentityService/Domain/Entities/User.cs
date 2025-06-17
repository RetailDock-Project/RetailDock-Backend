using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }


        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [MaxLength(50, ErrorMessage = "Email cannot be longer than 50 characters.")]
        [RegularExpression(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", ErrorMessage = "Email cannot contain spaces.")]
        public string Email { get; set; }

        [MaxLength(100)]
        public string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; }= DateTime.UtcNow;

        public Guid? OrganisationId { get; set; }

        public string? EmailVerificationToken { get; set; }
        public bool IsEmailConfirmed { get; set;} = false;


        [MaxLength(100)]
        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiry { get; set; }

        public List<UserOrganizationRole> userOrganizationRole { get; set; }
    }
}
