using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookAndPlay_API.Models;
using BookNadPlay_API;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BookNadPlay_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacilityController : ControllerBase
    {
        public IConfiguration configuration;
        private readonly DatabaseContext context;

        public FacilityController(IConfiguration config, DatabaseContext context)
        {
            this.configuration = config;
            this.context = context;
        }

        // GET: api/Facility/GetAll
        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<IEnumerable<Facility>>> GetAllFacilities()
        {
            return await context.Facilities.ToListAsync();
        }

        // GET: api/Facility/Get/{id}
        [HttpGet("Get/{id}")]
        public async Task<ActionResult<Facility>> GetFacility(int id)
        {
            var facility = await context.Facilities.FirstOrDefaultAsync(f => f.FacilityId == id);
            if (facility != null)
            {
                return BadRequest("Incorrect facility id");
            }
            return facility;
        }

        // GET: api/Facility/User/{id}
        [HttpGet("User/{id}")]
        public async Task<ActionResult<IEnumerable<Facility>>> GetUserFacilities(int id)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if(user != null)
            {
                return BadRequest("Incorrect user id");
            }
            return user.Facilities.ToList();
        }

        // GET: api/Facility/Names/{name}
        [HttpGet("Names/{name}")]
        public async Task<ActionResult<IEnumerable<Facility>>> GetFacilitiesByName(string name)
        {
            var facilities = await context.Facilities.Where(f => f.Name.StartsWith(name)).ToListAsync();
            return facilities;
        }

        // GET: api/Facility/Names/
        [HttpGet("Names/")]
        public ActionResult<IEnumerable<Facility>> GetAllFacilities_2()
        {
            return context.Facilities;
        }

        // POST: api/Facility
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Facility>> AddFacility(Facility facility)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == GetUserIdFromClaim(User));
            if(user != null)
            {
                return BadRequest("Incorrect user id");
            }

            facility.Owner = user;

            context.Facilities.Add(facility);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetFacility", new { id = facility.FacilityId }, facility);
        }

        // DELETE: api/Facility/Delete/5
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<Facility>> DeleteFacility(int id)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == GetUserIdFromClaim(User));
            if (user != null)
            {
                return BadRequest("Incorrect user id");
            }

            var facility = await context.Facilities.FindAsync(id);
            if (facility == null)
            {
                return NotFound();
            }

            if(facility.Owner.UserId != user.UserId)
            {
                return BadRequest("Incorrect facility id");
            }

            context.Facilities.Remove(facility);
            await context.SaveChangesAsync();

            return Ok("Facility succesfully deleted");
        }

        private bool FacilityExists(int id)
        {
            return context.Facilities.Any(e => e.FacilityId == id);
        }

        private int GetUserIdFromClaim(ClaimsPrincipal user)
        {
            //Get user id from token
            var idClaim = user.Claims.FirstOrDefault(x => x.Type.ToString().Equals("Id"));
            return int.Parse(idClaim.Value);
        }
    }
}
