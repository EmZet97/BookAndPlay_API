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
using BookNadPlay_API.Helpers;

namespace BookNadPlay_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SportController : ControllerBase
    {
        public IConfiguration configuration;
        private readonly DatabaseContext context;

        public SportController(IConfiguration config, DatabaseContext context)
        {
            this.configuration = config;
            this.context = context;
        }

        // GET: api/Sport/Names/{name}
        /// <summary>
        /// Returns all sports whose names starts with parameter given in url {name}
        /// </summary>
        [HttpGet("Names/{name}")]
        public async Task<ActionResult<IEnumerable<Sport>>> GetSport(string name)
        {
            var sports = await context.Sports.Where(c => c.Name.StartsWith(name)).ToListAsync();
            return sports;
        }

        // GET: api/Sport/Names/
        /// <summary>
        /// Returns all sports
        /// </summary>
        [HttpGet("Names/")]
        public ActionResult<IEnumerable<Sport>> GetAllSports()
        {
            return context.Sports;
        }

        // POST: api/Sport/Add
        /// <summary>
        /// Adds new sport if not exists
        /// </summary>
        [Authorize]
        [HttpPost("Add/")]
        public async Task<ActionResult<Sport>> AddSport(Sport sport)
        {
            var _sport = await context.Sports.FirstOrDefaultAsync(c => c.Name.ToLower() == sport.Name.ToLower());
            if (_sport != null)
            {
                return BadRequest("Sport already exists");
            }

            sport.Name = DataHelper.FirstLetterToUpper(sport.Name.ToLower());
            context.Sports.Add(sport);
            await context.SaveChangesAsync();

            return CreatedAtAction("AddSport", new { id = sport.SportId }, sport);
        }

        // GET: api/Sport/Get/Id/{id}
        /// <summary>
        /// Returns sport whose ID is equal to parameter given in url {id}
        /// </summary>
        [HttpGet("Get/Id/{id}")]
        public async Task<ActionResult<Sport>> GetSportById(int id)
        {
            var sport = await context.Sports.FindAsync(id);

            if (sport == null)
            {
                return NotFound();
            }

            return sport;
        }

        // GET: api/Sport/Get/Name/{name}
        /// <summary>
        /// Returns sport whose name is squal to parameter given in url {name}
        /// </summary>
        [HttpGet("Get/Name/{name}")]
        public async Task<ActionResult<Sport>> GetSportByName(string name)
        {
            var sport = await context.Sports.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());

            if (sport == null)
            {
                return NotFound();
            }

            return Ok(sport);
        }

        // POST: api/Sport/Get/Name
        /// <summary>
        /// Returns sport whose name is squal to parameter given in body {name}
        /// </summary>
        [HttpPost("Get/Name")]
        public async Task<ActionResult<Sport>> GetSportByName_Post([FromBody] string name)
        {
            var sport = await context.Sports.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());

            if (sport == null)
            {
                return NotFound();
            }

            return Ok(sport);
        }


        // DELETE: api/Sport/Delete/{id}
        /// <summary>
        /// Removes sport. Uses id given in url {id}
        /// </summary>
        [Authorize]
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<Sport>> DeleteSport(int id)
        {
            var sport = await context.Sports.FindAsync(id);
            if (sport == null)
            {
                return NotFound();
            }

            context.Sports.Remove(sport);
            await context.SaveChangesAsync();

            return Ok(sport);
        }
    }
}
