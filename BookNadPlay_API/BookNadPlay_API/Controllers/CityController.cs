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
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using BookNadPlay_API.Helpers;

namespace BookNadPlay_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        public IConfiguration configuration;
        private readonly DatabaseContext context;

        public CityController(IConfiguration config, DatabaseContext context)
        {
            this.configuration = config;
            this.context = context;
        }

        // GET: api/City/Names/{name}
        /// <summary>
        /// Returns all cities whose names starts with parameter {name}
        /// </summary>
        [HttpGet("Names/{name}")]
        public async Task<ActionResult<IEnumerable<City>>> GetCities(string name)
        {
            var cities = await context.Cities.Where(c => c.Name.StartsWith(name)).ToListAsync();
            return cities;
        }

        // GET: api/City/Names/
        /// <summary>
        /// Returns all cities
        /// </summary>
        [HttpGet("Names/")]
        public ActionResult<IEnumerable<City>> GetAllCities()
        {
            return context.Cities;
        }

        // POST: api/City/Add
        /// <summary>
        /// Adds new city
        /// </summary>
        [Authorize]
        [HttpPost("Add/")]
        public async Task<ActionResult<City>> AddCity(City city)
        {
            var _city = await context.Cities.FirstOrDefaultAsync(c => c.Name.ToLower() == city.Name.ToLower());
            if(_city != null)
            {
                return BadRequest("City already exists");
            }

            city.Name = DataHelper.FirstLetterToUpper(city.Name);
            context.Cities.Add(city);
            await context.SaveChangesAsync();

            return CreatedAtAction("AddCity", new { id = city.CityId }, city);
        }

        // GET: api/City/Get/Id/{id}
        /// <summary>
        /// Returns city. CityID given in url {id}
        /// </summary>
        [HttpGet("Get/Id/{id}")]
        public async Task<ActionResult<City>> GetCityById(int id)
        {
            var city = await context.Cities.FindAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            return city;
        }

        // GET: api/City/Get/Name/{name}
        /// <summary>
        /// Returns city. City name given in url {name}
        /// </summary>
        [HttpGet("Get/Name/{name}")]
        public async Task<ActionResult<City>> GetCityByName(string name)
        {
            var city = await context.Cities.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());

            if (city == null)
            {
                return NotFound();
            }

            return Ok(city);
        }

        [HttpPost("Get/Name")]
        /// <summary>
        /// Returns city. City name given in body.
        /// </summary>
        public async Task<ActionResult<City>> GetCityByName_Post([FromBody] string name)
        {
            var city = await context.Cities.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());

            if (city == null)
            {
                return NotFound();
            }

            return Ok(city);
        }


        // DELETE: api/City/Delete/{id}
        /// <summary>
        /// Removes city. CityID given in url {id}.
        /// </summary>
        [Authorize]
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<City>> DeleteCity(int id)
        {
            var city = await context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            context.Cities.Remove(city);
            await context.SaveChangesAsync();

            return Ok(city);
        }

        
    }
}
