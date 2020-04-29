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
using BookNadPlay_API.Helpers;
using BookNadPlay_API.Migrations;

namespace BookNadPlay_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        public IConfiguration configuration;
        private readonly DatabaseContext context;

        public ReservationController(IConfiguration config, DatabaseContext context)
        {
            this.configuration = config;
            this.context = context;
        }

        // GET: api/Reservation/Get/{id}
        /// <summary>
        /// Returns reservations of facility. FacilityID given in url {id}
        /// </summary>
        [HttpGet("Get/{id}")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservationsOfFacility(int id)
        {
            var fac = await context.Facilities.Where(f => f.FacilityId == id).Include( f=>f.Reservations).FirstOrDefaultAsync();
            if(fac == null)
            {
                return NotFound();
            }

            return fac.Reservations.ToList();
        }

        //7 days period
        // GET: api/Reservation/Available/Get/{id}
        /// <summary>
        /// Returns available (not booked) hours of facility. FacilityID given in url {id}
        /// </summary>
        [HttpGet("Available/Get/{id}")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetAvailableTerms(int id)
        {
            var fac = await context.Facilities.Where(f => f.FacilityId == id).Include(f => f.Reservations).Include(f => f.AccessPeriods).FirstOrDefaultAsync();
            if(fac == null)
            {
                return NotFound();
            }

            // Get future reservations
            var reservations = fac.Reservations.Where(r => r.EndTime > DateTime.Now && (r.Status == ReservationStatus.Booked || r.Status == ReservationStatus.Inactive)).ToList();

            //Generate future dates and delete already booked or inactive
            var availableDates = new List<Reservation>();
            foreach(AccessPeriod ap in fac.AccessPeriods)
            {
                var date = DataHelper.Date.GetNextDayOfWeekDate(ap.DayOfWeek);
                DateTime startTime = new DateTime(date.Year, date.Month, date.Day, ap.StartHour ?? 0, ap.StartMinute ?? 0, 0);
                DateTime endTime = new DateTime(date.Year, date.Month, date.Day, ap.EndHour ?? 0, ap.EndMinute ?? 0, 0);

                var res = reservations.Where(r => r.StartTime == startTime && r.EndTime == endTime).FirstOrDefault();
                if (res == null)
                    availableDates.Add(new Reservation()
                    {
                        StartTime = startTime,
                        EndTime = endTime,
                        Status = ReservationStatus.NotBooked
                    }); ;
            }
            return availableDates;

        }

        // GET: api/Reservation/5
        /// <summary>
        /// Returns reservation. ReservationID given in url {id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var reservation = await context.Reservations.FindAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            return reservation;
        }


        // POST: api/Reservation/Add
        /// <summary>
        /// Adds new reservation
        /// </summary>
        [HttpPost]
        [Route("Add")]
        public async Task<ActionResult<Reservation>> PostReservation(ReservationModel reservation)
        {
            //TODO
            return Ok();
            //return CreatedAtAction("GetReservation", new { id = reservation.ReservationId }, reservation);
        }

        // DELETE: api/Reservation/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Reservation>> DeleteReservation(int id)
        {
            var reservation = await context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            context.Reservations.Remove(reservation);
            await context.SaveChangesAsync();

            return reservation;
        }

        private bool ReservationExists(int id)
        {
            return context.Reservations.Any(e => e.ReservationId == id);
        }
    }
}
