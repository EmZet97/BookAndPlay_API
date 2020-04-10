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

        public string Name { get; set; }

        public string Street { get; set; }

        public string BuildingNumber { get; set; }

        [ForeignKey("City")]
        public int CityId { get; set; }
        public virtual City City { get; set; }

        [ForeignKey("Sport")]
        public int SportId { get; set; }
        public virtual Sport Sport { get; set; }


        public virtual User Owner { get; set; }

        [JsonIgnore]
        public virtual IEnumerable<AccessPeriod> AccessPeriods { get; set; }


    }
}
