using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookAndPlay_API.Models
{
    public enum ReservationStatus
    {
        NotBooked = 0, Booked = 1, Cancelled = 2, Inactive = 3
    }

    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.NotBooked;

        public virtual User User { get; set; }

        public virtual Facility Facility { get; set; }


    }
}
