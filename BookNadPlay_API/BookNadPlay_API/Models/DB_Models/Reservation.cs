using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookAndPlay_API.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }

        public DateTime DateTime { get; set; }


        public virtual User User { get; set; }

        public virtual Facility Facility { get; set; }

    }
}
