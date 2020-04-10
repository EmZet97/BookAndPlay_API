using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookAndPlay_API.Models
{
    public class Sport
    {
        [Key]
        public int SportId { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public virtual IEnumerable<Facility> Facilities { get; set; }

    }
}
