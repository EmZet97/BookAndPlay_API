using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BookAndPlay_API.Models
{
    public enum UserRoles
    {
        Admin = 0, Moder = 1, Normal = 2, Guest = 3
    }

    public class UserRole
    {
        public string Name { get; set; }
    }

    public class User
    {
        public User()
        {
            RoleId = (int)UserRoles.Normal;
            RoleName = UserRoles.Normal.ToString();
        }

        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage ="Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password is too short")]
        [DataType(DataType.Password)]
        [JsonIgnore]
        public string Password { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MinLength(3, ErrorMessage = "Name is too short")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        [MinLength(3, ErrorMessage = "Surname is too short")]
        [DataType(DataType.Text)]
        public string Surname { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [NotMapped]
        public string Avatar
        {
            get
            {
                char[] charsToTrim = { '*', ' ', '\'', ' ' };
                return "Files/Avatars/" + Name.ToLower().Trim(charsToTrim)[0] + ".png";
            }
        }

        public int RoleId { get; set; }
        public string RoleName { get; set; }

        [JsonIgnore]
        public virtual IEnumerable<Facility> Facilities { get; set; }

        [JsonIgnore]
        public virtual IEnumerable<Reservation> Reservations { get; set; }
    }
}
