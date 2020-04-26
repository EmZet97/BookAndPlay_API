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
using BookNadPlay_API.Helpers;

namespace BookNadPlay_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessPeriodController : ControllerBase
    {
        public IConfiguration configuration;
        private readonly DatabaseContext context;

        public AccessPeriodController(IConfiguration config, DatabaseContext context)
        {
            this.configuration = config;
            this.context = context;
        }

        // GET: api/AccessPeriod/Facility/Get/{id}
        [HttpGet("Facility/Get/{id}")]
        public async Task<ActionResult<IEnumerable<AccessPeriod>>> GetAccessPeriodsFromFacility(int id)
        {
            var fac = await context.Facilities.Where(f => f.FacilityId == id).Include(f => f.AccessPeriods).FirstOrDefaultAsync();
            if(fac == null)
            {
                return NotFound();
            }

            return fac.AccessPeriods.ToList();
        }

        // GET: api/AccessPeriod/Get/{id}
        [HttpGet("Get/{id}")]
        public async Task<ActionResult<AccessPeriod>> GetAccessPeriodByID(int id)
        {
            var accessPeriod = await context.AccessPeriods.FindAsync(id);

            if (accessPeriod == null)
            {
                return NotFound();
            }

            return accessPeriod;
        }

        // POST: api/AccessPeriod/Add
        [Authorize]
        [HttpPost]
        [Route("Add")]
        public async Task<ActionResult<AccessPeriod>> AddAccesPeriod(AccessPeriod accessPeriod)
        {
            var user = await context.Users.Where(u => u.UserId == GetUserIdFromClaim(User)).Include(u => u.Facilities).FirstOrDefaultAsync();
            if (user == null)
            {
                return BadRequest("Incorrect user");
            }

            Facility fac = user.Facilities.Where(f => f.FacilityId == accessPeriod.FacilityId).FirstOrDefault();
            if(fac == null)
            {
                return BadRequest("User doesn't own that facility");
            }

            if(!DataHelper.CheckTime(accessPeriod.StartHour ?? 0, accessPeriod.StartMinute ?? 0, accessPeriod.EndHour ?? 0, accessPeriod.EndHour ?? 0))
            {
                return BadRequest("Incorrect time data");
            }

            accessPeriod.Facility = fac;

            context.AccessPeriods.Add(accessPeriod);
            await context.SaveChangesAsync();

            return Ok(accessPeriod);
        }

        // DELETE: api/AccessPeriod/Delete/5
        [Authorize]
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<AccessPeriod>> DeleteAccessPeriod(int id)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == GetUserIdFromClaim(User));
            if (user == null)
            {
                return BadRequest("Incorrect user");
            }

            var accessPeriod = await context.AccessPeriods.FindAsync(id);
            if (accessPeriod == null || accessPeriod.Facility.Owner.UserId != user.UserId)
            {
                return NotFound();
            }

            context.AccessPeriods.Remove(accessPeriod);
            await context.SaveChangesAsync();

            return accessPeriod;
        }

        private bool AccessPeriodExists(int id)
        {
            return context.AccessPeriods.Any(e => e.AccessPeriodId == id);
        }

        private int GetUserIdFromClaim(ClaimsPrincipal user)
        {
            //Get user id from token
            var idClaim = user.Claims.FirstOrDefault(x => x.Type.ToString().Equals("Id"));
            return int.Parse(idClaim.Value);
        }
    }
}