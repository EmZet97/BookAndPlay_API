using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BookAndPlay_API.Models
{
    public class Sport
    {
        [Key]
        public int SportId { get; set; }
        [MinLength(3, ErrorMessage = "Sport name require at least 3 characters")]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual IEnumerable<Facility> Facilities { get; set; }

    }
}
