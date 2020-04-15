using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookNadPlay_API.Models
{
    public class FacilityModel
    {
        [Required(ErrorMessage = "Name is required")]
        [MinLength(3, ErrorMessage = "Name require at least 3 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [MinLength(1, ErrorMessage = "Address require at least 1 character")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City name is required")]
        [MinLength(2, ErrorMessage = "City name require at least 2 characters")]
        public string City { get; set; }

        [Required(ErrorMessage = "Sport name is required")]
        [MinLength(2, ErrorMessage = "Sport name require at least 2 characters")]
        public string Sport { get; set; }
    }
}
