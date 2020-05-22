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

        [HttpGet("Get/{imageName}")]
        public IActionResult GetImages(string imageName)
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), "PublicFiles", "Images", "Tester", imageName);

            return PhysicalFile(file, "image/jpg");
        }

        [HttpPost("UploadFew")]
        public async Task<IActionResult> UploadImages(IFormFile[] files)
        {
            long size = files.Sum(f => f.Length);
            List<string> images = new List<string>();

            // Check directory
            string dirPath = Path.Combine("PublicFiles", "Images", "Tester");
            string subPath = Path.Combine(Directory.GetCurrentDirectory(), dirPath);

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
                        images.Add(file.FileName);
                    }
                }
                else
                {
                    return BadRequest("Incorrect image type");
                }
            }

            if(images.Count > 0)
                return Ok(images);

            return BadRequest("No images found");
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            string image = "";

            // Check directory
            string dirPath = Path.Combine("PublicFiles", "Images", "Tester");
            string subPath = Path.Combine(Directory.GetCurrentDirectory(), dirPath);

            bool exists = Directory.Exists(subPath);
            if (!exists)
                Directory.CreateDirectory(subPath);


            if (file.Length > 0 && file.ContentType.Contains("image"))
            {
                var filePath = Path.Combine(subPath, file.FileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                    image = file.FileName;
                }
            }
            else
            {
                return BadRequest("Incorrect image type");
            }
            

            if (image.Length > 0)
                return Ok(image);

            return BadRequest("No image found");
        }


    }
}