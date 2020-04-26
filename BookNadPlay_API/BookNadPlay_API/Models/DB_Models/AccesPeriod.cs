using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BookAndPlay_API.Models
{
    public class AccessPeriod
    {
        [Key]
        public int AccessPeriodId { get; set; }

        [Required(ErrorMessage = "Start hour required")]
        [Range(0,23, ErrorMessage = "Incorrect start hour value")]
        public int? StartHour { get; set; }    
        
        [Required]
        [Range(0, 59, ErrorMessage = "Incorrect start minute value")]
        public int? StartMinute { get; set; }

        [Required]
        [Range(0, 23, ErrorMessage = "Incorrect end hour value")]
        public int? EndHour { get; set; }

        [Required]
        [Range(0, 59, ErrorMessage = "Incorrect end minute value")]
        public int? EndMinute { get; set; }

        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        [ForeignKey("Facility")]
        public int FacilityId { get; set; }
        [JsonIgnore]
        public virtual Facility Facility { get; set; }

    }
}
