using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BookAndPlay_API.Helpers;
using BookNadPlay_API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BookAndPlay_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        public IConfiguration configuration;
        private readonly DatabaseContext context;        

        public ImagesController(IConfiguration config, DatabaseContext context)
        {
            this.configuration = config;
            this.context = context;
        }

        [HttpGet("Get")]
        public IActionResult GetImage()
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(),
                            "PublicFiles", "Images", "test.jpg");

            return PhysicalFile(file, "image/jpg");
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> UploadImage(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);
            List<string> images = new List<string>();

            // Check directory
            string subPath = Path.Combine(Directory.GetCurrentDirectory(),
                            "PublicFiles", "Images", "User001");
            bool exists = Directory.Exists(subPath);
            if (!exists)
                Directory.CreateDirectory(subPath);


            foreach (var file in files)
            {
                if (file.Length > 0 && file.ContentType.Contains("image"))
                {
                    var filePath = Path.Combine(subPath, file.FileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                        images.Add(filePath);
                    }
                }
                else
                {
                    return BadRequest("Incorrect image type");
                }
            }

            return Ok(images);
        }


    }
}