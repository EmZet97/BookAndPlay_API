using BookAndPlay_API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookNadPlay_API.Models
{
    public class User_RegisterModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password is too short")]
        [DataType(DataType.Password)]
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

    }
}
