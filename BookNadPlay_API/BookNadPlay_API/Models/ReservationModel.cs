using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookAndPlay_API.Models
{
    public class ReservationModel
    {
        [Required]
        public int? AccessPeriodID { get; set; }
        [Required]
        public int? UserId { get; set; }
    }
}
