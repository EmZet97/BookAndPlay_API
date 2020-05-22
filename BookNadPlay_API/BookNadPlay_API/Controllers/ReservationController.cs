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
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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
            var fac = await context.Facilities.Where(f => f.FacilityId == id).Include( f=>f.Reservations).Include(f=>f.Owner).FirstOrDefaultAsync();
            if(fac == null)
            {
                return NotFound();
            }

            return fac.Reservations.ToList();
        }

        //7 days period
        // GET: api/Reservation/Available/Get/{id}
        /// <summary>
        /// Returns available (not booked) hours of facility in next 7 days period. FacilityID given in url {id}
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
                    }); 
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


        //7 days period
        // GET: api/Reservation/Available/GetAll/{id}
        /// <summary>
        /// Returns available (not booked) hours of facility in next 7 days period and also booked ones. FacilityID given in url {id}
        /// </summary>
        [HttpGet("Available/GetAll/{id}")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetAllTerms(int id)
        {
            var fac = await context.Facilities.Where(f => f.FacilityId == id).Include(f => f.Reservations).Include(f => f.AccessPeriods).FirstOrDefaultAsync();
            if (fac == null)
            {
                return NotFound();
            }

            // Get future reservations
            var reservations = fac.Reservations.Where(r => r.EndTime > DateTime.Now && (r.Status == ReservationStatus.Booked || r.Status == ReservationStatus.Inactive)).ToList();

            //Generate future dates and delete already booked or inactive
            var allDates = new List<Reservation>();
            foreach (AccessPeriod ap in fac.AccessPeriods)
            {
                var date = DataHelper.Date.GetNextDayOfWeekDate(ap.DayOfWeek);
                DateTime startTime = new DateTime(date.Year, date.Month, date.Day, ap.StartHour ?? 0, ap.StartMinute ?? 0, 0);
                DateTime endTime = new DateTime(date.Year, date.Month, date.Day, ap.EndHour ?? 0, ap.EndMinute ?? 0, 0);

                var res = reservations.Where(r => r.StartTime == startTime && r.EndTime == endTime).FirstOrDefault();
                if (res == null)
                    allDates.Add(new Reservation()
                    {
                        StartTime = startTime,
                        EndTime = endTime,
                        Status = ReservationStatus.NotBooked
                    });
                else
                    allDates.Add(res);
            }


            return allDates;

        }


        // POST: api/Reservation/Add
        /// <summary>
        /// Adds new reservation
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("Add")]
        public async Task<ActionResult<Reservation>> AddReservation(ReservationModel reservation)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == GetUserIdFromClaim(User));
            if (user == null)
            {
                return BadRequest("Incorrect user id");
            }

            var ap = await context.AccessPeriods.Where(a => a.AccessPeriodId == reservation.AccessPeriodID).Include(a => a.Facility).FirstOrDefaultAsync();
            if(ap == null || user == null || ap.Facility == null)
            {
                return NotFound();
            }
            

            var date = DataHelper.Date.GetNextDayOfWeekDate(ap.DayOfWeek);

            var res = new Reservation()
            {
                AccessPeriodId = ap.AccessPeriodId,
                User = user,
                Facility = ap.Facility,
                Status = ReservationStatus.Booked,
                StartTime = new DateTime(date.Year, date.Month, date.Day, (int)ap.StartHour, (int)ap.StartMinute, 0),
                EndTime = new DateTime(date.Year, date.Month, date.Day, (int)ap.EndHour, (int)ap.EndMinute, 0)
            };

            var reservations = await context.Reservations.Where(r => r.AccessPeriodId == reservation.AccessPeriodID).ToListAsync();
            var res_test = reservations.Where(r => r.StartTime == res.StartTime && r.EndTime == res.EndTime && r.Status == ReservationStatus.Booked).FirstOrDefault();
            if(res_test != null)
            {
                return BadRequest("Date already booked");
            }

            context.Reservations.Add(res);
            context.SaveChanges();

            return CreatedAtAction("GetReservation", new { id = res.ReservationId }, res);
        }

        // DELETE: api/Reservation/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Reservation>> DeleteReservation(int id)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == GetUserIdFromClaim(User));
            if (user == null)
            {
                return BadRequest("Incorrect user id");
            }

            var reservation = await context.Reservations.Where(r => r.ReservationId == id).Include(r => r.User).FirstOrDefaultAsync();
            if (reservation == null)
            {
                return NotFound();
            }

            if(reservation.User.UserId != user.UserId)
            {
                return Unauthorized("Other user own that reservation");
            }

            // Change status to Cancelled
            reservation.Status = ReservationStatus.Cancelled;
            await context.SaveChangesAsync();

            return reservation;
        }

        private bool ReservationExists(int id)
        {
            return context.Reservations.Any(e => e.ReservationId == id);
        }

        private int GetUserIdFromClaim(ClaimsPrincipal user)
        {
            //Get user id from token
            var idClaim = user.Claims.FirstOrDefault(x => x.Type.ToString().Equals("Id"));
            return int.Parse(idClaim.Value);
        }

    }
}
