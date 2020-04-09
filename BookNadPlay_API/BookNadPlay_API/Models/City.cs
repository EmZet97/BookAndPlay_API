﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookAndPlay_API.Models
{
    public class City
    {
        [Key]
        public int CityId { get; set; }
        public string Name { get; set; }

        public virtual IEnumerable<Facility> Facilities { get; set; }
    }
}