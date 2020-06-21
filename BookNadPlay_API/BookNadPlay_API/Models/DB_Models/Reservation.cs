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
        NotBooked = 0, Booked = 1, CancelledByOwner = 2, Inactive = 3, CancelledByUser = 4
    }

    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public int? AccessPeriodId { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.NotBooked;

        [NotMapped]
        public bool Archives { 
            get {
                return EndTime < DateTime.Now;
            }
        }

        public virtual User User { get; set; }

        public virtual Facility Facility { get; set; }


    }
}
