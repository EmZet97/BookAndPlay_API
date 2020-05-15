
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookAndPlay_API.Models
{
    public class FileModel
    {
        [Required]
        public int? ID { get; set; }
        public List<IFormFile> Files { get; set; }
    }
}
