using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class AddUserDto
    {

        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(30, ErrorMessage = "Name cannot be longer than 30 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [MaxLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        [RegularExpression(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", ErrorMessage = "Email cannot contain spaces.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,50}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; }

        public Guid? OrgRoleId { get; set; }


    }

    public class LoginDto {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [MaxLength(50, ErrorMessage = "Email cannot be longer than 50 characters.")]
        [RegularExpression(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", ErrorMessage = "Email cannot contain spaces.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,50}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; }
    }

    public class UserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class UpdateUserRoleDto
    {
        public Guid UserId { get; set; }
        public Guid NewRoleId { get; set; }
    }

    public class UserStatsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
    }


}
