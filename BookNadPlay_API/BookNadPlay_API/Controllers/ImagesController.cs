using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BookAndPlay_API.Helpers;
using BookAndPlay_API.Models.DB_Models;
using BookNadPlay_API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet("Facility/{FacilityId}")]
        public async Task<ActionResult<IEnumerable<FacilityImage>>> GetFacilityImages(int facilityId)
        {
            var facility = await context.Facilities.Where(f => f.FacilityId.Equals(facilityId)).Include(f => f.FacilityImages).FirstOrDefaultAsync();
            if (facility == null)
                return NotFound("Facility not found");

            return Ok(facility.FacilityImages);
        }


        [Authorize]
        [HttpPost("Upload/Facility/{facilityId}")]
        public async Task<ActionResult<FacilityImage>> UploadImages(int facilityId, IFormFile[] files)
        {
            var user = await context.Users.Where(u => u.UserId == GetUserIdFromClaim(User)).Include(u => u.Facilities).FirstOrDefaultAsync();
            if (user == null)
            {
                return BadRequest("Incorrect user id");
            }

            var facility = await context.Facilities.Where(f => f.FacilityId.Equals(facilityId)).Include(f => f.Owner).Include(f => f.FacilityImages).FirstOrDefaultAsync();
            if (facility == null)
                return NotFound("Facility not found");

            if (facility.Owner.UserId != user.UserId)
                return BadRequest("User isn't owner of that facility");

            int imageCount = facility.FacilityImages.Count();

            long size = facility.FacilityImages.Count();
            

            // Check directory
            string dirPath = Path.Combine("wwwroot", "Images", "Facility" + facilityId);
            string subPath = Path.Combine(Directory.GetCurrentDirectory(), dirPath);
            string dbPath = Path.Combine("Files", "Images", "Facility" + facilityId);  //Forst parameter in Startup.cs

            bool exists = Directory.Exists(subPath);
            if (!exists)
                Directory.CreateDirectory(subPath);

            List<FacilityImage> images = new List<FacilityImage>();

            foreach (var file in files)
            {
                if (!(file.Length > 0 && file.ContentType.Contains("image")))
                {
                    return BadRequest("Incorrect image type");
                }
            }

            foreach (var file in files)
            {
                var localPath = Path.Combine(subPath, "image_" + imageCount + "_" + file.FileName);
                var finalDbPath = Path.Combine(dbPath, "image_" + imageCount + "_" + file.FileName);
                imageCount++;

                using (var stream = System.IO.File.Create(localPath))
                {
                    await file.CopyToAsync(stream);
                    finalDbPath = finalDbPath.Replace("\\", "/");
                    var image = new FacilityImage() { FacilityId = facilityId, ImageUrl = finalDbPath };
                    //TODO
                    images.Add(image);
                }             
            }            

            if(images.Count > 0)
            {
                await context.FacilityImages.AddRangeAsync(images);
                await context.SaveChangesAsync();
                return Ok(images);
            }                

            return BadRequest("No images found");
        }

       
        private int GetUserIdFromClaim(ClaimsPrincipal user)
        {
            //Get user id from token
            var idClaim = user.Claims.FirstOrDefault(x => x.Type.ToString().Equals("Id"));
            return int.Parse(idClaim.Value);
        }


    }
}