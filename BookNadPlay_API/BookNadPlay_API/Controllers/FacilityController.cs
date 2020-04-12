using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookAndPlay_API.Models;
using BookNadPlay_API;

namespace BookNadPlay_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacilityController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public FacilityController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Facility
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Facility>>> GetFacilities()
        {
            return await _context.Facilities.ToListAsync();
        }

        // GET: api/Facility/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Facility>> GetFacility(int id)
        {
            var facility = await _context.Facilities.FindAsync(id);

            if (facility == null)
            {
                return NotFound();
            }

            return facility;
        }

        // PUT: api/Facility/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFacility(int id, Facility facility)
        {
            if (id != facility.FacilityId)
            {
                return BadRequest();
            }

            _context.Entry(facility).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FacilityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Facility
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Facility>> PostFacility(Facility facility)
        {
            _context.Facilities.Add(facility);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFacility", new { id = facility.FacilityId }, facility);
        }

        // DELETE: api/Facility/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Facility>> DeleteFacility(int id)
        {
            var facility = await _context.Facilities.FindAsync(id);
            if (facility == null)
            {
                return NotFound();
            }

            _context.Facilities.Remove(facility);
            await _context.SaveChangesAsync();

            return facility;
        }

        private bool FacilityExists(int id)
        {
            return _context.Facilities.Any(e => e.FacilityId == id);
        }
    }
}
