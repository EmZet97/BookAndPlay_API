using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookNadPlay_API.Models
{
    public class FacilityFilterModel
    {
        public string Name { get; set; }

        public string City { get; set; }

        public string Sport { get; set; }

        public int? Day { get; set; }
    }
}
