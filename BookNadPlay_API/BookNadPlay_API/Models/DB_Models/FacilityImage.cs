using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookAndPlay_API.Models.DB_Models
{
    public class FacilityImage
    {
        [Key]
        public int ImageId { get; set; }
        public string ImageUrl { get; set; }

        [ForeignKey("Facility")]
        public int FacilityId { get; set; }
    }
}
