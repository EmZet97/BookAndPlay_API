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
using BookNadPlay_API.Models;
using BookNadPlay_API.Helpers;
using System.Net;
using System.Runtime.Serialization.Json;
using Nancy.Json;
using BookAndPlay_API.Helpers;

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
        /// <summary>
        /// Returns all facilities
        /// </summary>
        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<IEnumerable<Facility>>> GetAllFacilities()
        {
            return await context.Facilities.Include(f => f.Owner).Include(f => f.Sport).Include(f => f.City).ToListAsync();
        }

        // GET: api/Facility/Get/{id}
        /// <summary>
        /// Returns facility where id is equal to parameter given in url {id}
        /// </summary>
        [HttpGet("Get/{id}")]
        public async Task<ActionResult<Facility>> GetFacility(int id)
        {
            var facility = await context.Facilities.Include(f => f.Owner).Include(f => f.Sport).Include(f => f.City).FirstOrDefaultAsync(f => f.FacilityId == id);
            if (facility == null)
            {
                return BadRequest("Incorrect facility id");
            }
            return facility;
        }

        // GET: api/Facility/User/{id}
        /// <summary>
        /// Returns facilities of user. UserID given in url {id}
        /// </summary>
        [HttpGet("User/{id}")]
        public async Task<ActionResult<IEnumerable<Facility>>> GetUserFacilities(int id)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if(user == null)
            {
                return BadRequest("Incorrect user id");
            }
            return user.Facilities.ToList();
        }



        // GET: api/Facility/Names/{name}
        /// <summary>
        /// Returns facilities whose name starts with parameter given in url {name}
        /// </summary>
        [HttpGet("Names/{name}")]
        public async Task<ActionResult<IEnumerable<Facility>>> GetFacilitiesByName(string name)
        {
            var facilities = await context.Facilities.Where(f => f.Name.StartsWith(name)).Include(f => f.Owner).Include(f => f.Sport).Include(f => f.City).ToListAsync();
            return facilities;
        }

        // GET: api/Facility/Names/
        /// <summary>
        /// Returns all facilities
        /// </summary>
        [HttpGet("Names/")]
        public ActionResult<IEnumerable<Facility>> GetAllFacilities_2()
        {
            return context.Facilities.Include(f => f.Owner).Include(f => f.Sport).Include(f => f.City).ToList();
        }

        // GET: api/Facility/Filter/
        /// <summary>
        /// Returns filtered facilities (filter parameters aren't required)
        /// </summary>
        [HttpGet("Filter")]
        public async Task<ActionResult<IEnumerable<Facility>>> GetFilteredFacilities(FacilityFilterModel filter)
        {
            var sport = await context.Facilities.FirstOrDefaultAsync(s => s.Name.ToLower() == filter.Sport.ToLower());
            var city = await context.Cities.FirstOrDefaultAsync(c => c.Name.ToLower() == filter.City.ToLower());
            var facilities = await (filter.Name != null ? context.Facilities.Where(f => f.Name.ToLower().StartsWith(filter.Name.ToLower())).Include(f => f.Owner).Include(f => f.Sport).Include(f => f.City).ToListAsync() : context.Facilities.Include(f => f.Owner).Include(f => f.Sport).Include(f => f.City).ToListAsync());

            //City filter
            if(city != null)
                facilities = facilities.Where(c => c.CityId == city.CityId).ToList();

            //City filter
            if (sport != null)
                facilities = facilities.Where(s => s.SportId == sport.SportId).ToList();


            return Ok(facilities);
        }

        // POST: api/Facility/Add
        /// <summary>
        /// Adds facility
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("Add")]
        public async Task<ActionResult<Facility>> AddFacility(FacilityModel facility_model)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == GetUserIdFromClaim(User));
            if(user == null)
            {
                return BadRequest("Incorrect user id");
            }
            
            if(user.Facilities != null)
            {
                var fac = context.Facilities.FirstOrDefault(f => f.Name.ToLower() == facility_model.Name.ToLower());
                if (fac != null)
                {
                    return BadRequest("Facility name already exists in your objects list");
                }

            }

            //Get or create city
            string city_name = LocalizationHelper.GetAddress(facility_model.Lat ?? 0.0, facility_model.Lon ?? 0.0).Address.City;
            var city = await context.Cities.FirstOrDefaultAsync(c => c.Name.ToLower() == city_name);
            if(city == null)
            {
                //return BadRequest("Incorrect city");
                city = new City() { Name = city_name };
                context.Cities.Add(city);
                context.SaveChanges();
            }

            //Get sport
            var sport = await context.Sports.FirstOrDefaultAsync(s => s.Name.ToLower() == facility_model.Sport.ToLower());
            if (sport == null)
            {
                return BadRequest("Sport doesn't exists");
                //sport = new Sport() { Name = DataHelper.FirstLetterToUpper(facility_model.Sport) };
                //context.Sports.Add(sport);
                //context.SaveChanges();
            }


            var facility = new Facility()
            {
                Owner = user,
                Description = facility_model.Description,
                Name = facility_model.Name,
                Address = facility_model.Address,
                Sport = sport,
                SportId = sport.SportId,
                City = city,
                CityId = city.CityId,
                Lat = facility_model.Lat,
                Lon = facility_model.Lon
            };

            context.Facilities.Add(facility);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetFacility", new { id = facility.FacilityId }, facility);
        }

        // DELETE: api/Facility/Delete/5
        /// <summary>
        /// Delete facility by id given in url
        /// </summary>
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<Facility>> DeleteFacility(int id)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == GetUserIdFromClaim(User));
            if (user == null)
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

        private bool Exists(string name, IEnumerable<object> array)
        {
            return true;
        }
    }
}
