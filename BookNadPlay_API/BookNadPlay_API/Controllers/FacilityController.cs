﻿using System;
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
            return await context.Facilities.Include(f => f.Owner).Include(f => f.Sport).Include(f => f.City).ToListAsync();
        }

        // GET: api/Facility/Get/{id}
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
        [HttpGet("Names/{name}")]
        public async Task<ActionResult<IEnumerable<Facility>>> GetFacilitiesByName(string name)
        {
            var facilities = await context.Facilities.Where(f => f.Name.StartsWith(name)).Include(f => f.Owner).Include(f => f.Sport).Include(f => f.City).ToListAsync();
            return facilities;
        }

        // GET: api/Facility/Names/
        [HttpGet("Names/")]
        public ActionResult<IEnumerable<Facility>> GetAllFacilities_2()
        {
            return context.Facilities.Include(f => f.Owner).Include(f => f.Sport).Include(f => f.City).ToList();
        }

        // GET: api/Facility/Filter/
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
            var city = await context.Cities.FirstOrDefaultAsync(c => c.Name.ToLower() == facility_model.Name.ToLower());
            if(city == null)
            {
                //return BadRequest("Incorrect city");
                city = new City() { Name = DataHelper.FirstLetterToUpper(facility_model.City) };
                context.Cities.Add(city);
                context.SaveChanges();
            }

            //Get or create sport
            var sport = await context.Sports.FirstOrDefaultAsync(s => s.Name.ToLower() == facility_model.Sport.ToLower());
            if (sport == null)
            {
                return BadRequest("Incorrect sport");
                sport = new Sport() { Name = DataHelper.FirstLetterToUpper(facility_model.Sport) };
                context.Sports.Add(sport);
                context.SaveChanges();
            }


            var facility = new Facility()
            {
                Owner = user,
                Name = facility_model.Name,
                Address = facility_model.Address,
                Sport = sport,
                SportId = sport.SportId,
                City = city,
                CityId = city.CityId
            };

            context.Facilities.Add(facility);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetFacility", new { id = facility.FacilityId }, facility);
        }

        // DELETE: api/Facility/Delete/5
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
