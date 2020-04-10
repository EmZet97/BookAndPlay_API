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

        public DateTime Time { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        [JsonIgnore]
        public virtual Facility Facility { get; set; }

    }
}
