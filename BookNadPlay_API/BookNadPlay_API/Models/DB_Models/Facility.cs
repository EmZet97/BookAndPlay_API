using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BookAndPlay_API.Models
{
    public class Facility
    {
        [Key]
        public int FacilityId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MinLength(3, ErrorMessage ="Name require at least 3 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MinLength(3, ErrorMessage = "Description require at least 3 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [MinLength(3, ErrorMessage = "Address require at least 1 character")]
        public string Address { get; set; } //Full adress string

        [Required(ErrorMessage = "City id is required")]
        [ForeignKey("City")]
        public int CityId { get; set; }
        public virtual City City { get; set; }

        //Coordinates
        [Required]
        public double? Lat { get; set; }
        [Required]
        public double? Lon { get; set; }


        [Required]
        [ForeignKey("Sport")]
        public int SportId { get; set; }
        public virtual Sport Sport { get; set; }

        public virtual User Owner { get; set; }

        [JsonIgnore]
        public virtual IEnumerable<Reservation> Reservations { get; set; }

        [JsonIgnore]
        public virtual IEnumerable<AccessPeriod> AccessPeriods { get; set; }


    }
}
