using BookAndPlay_API.Models;
using BookNadPlay_API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAndPlay_API.Helpers
{
    public class ReservationsHelper
    {
        public static bool AreDatesCollapsing(Reservation r1, Reservation r2)
        {
            if (r1.EndTime <= r2.StartTime || r2.EndTime <= r1.StartTime)
                return false;

            return true;
        }

        public static Reservation GetEmptyIfAvailable(AccessPeriod accessPeriod, List<Reservation> booked, List<Reservation> inactive)
        {
            var date = DataHelper.Date.GetNextDayOfWeekDate(accessPeriod.DayOfWeek);
            DateTime startTime = new DateTime(date.Year, date.Month, date.Day, accessPeriod.StartHour ?? 0, accessPeriod.StartMinute ?? 0, 0);
            DateTime endTime = new DateTime(date.Year, date.Month, date.Day, accessPeriod.EndHour ?? 0, accessPeriod.EndMinute ?? 0, 0);

            //Check if term already booked
            var res = booked.Where(r => r.StartTime == startTime && r.EndTime == endTime).FirstOrDefault();
            if (res != null)
                return null;

            //Check if access period date expired
            if (!((accessPeriod.FromDate != null && accessPeriod.FromDate <= startTime) || (accessPeriod.ToDate != null && accessPeriod.ToDate > endTime)))
            {
                return null;
            }

            //Check if collaps with inactive date
            foreach (var inact in inactive)
            {
                if (AreDatesCollapsing(inact, new Reservation() { StartTime = startTime, EndTime = endTime }))
                    return null;
            }

            return new Reservation()
            {
                StartTime = startTime,
                EndTime = endTime,
                Status = ReservationStatus.NotBooked,
                AccessPeriodId = accessPeriod.AccessPeriodId
            };
            
        }

        public static Reservation GetExistingOrEmpty(AccessPeriod accessPeriod, List<Reservation> booked, List<Reservation> inactive)
        {
            var date = DataHelper.Date.GetNextDayOfWeekDate(accessPeriod.DayOfWeek);
            DateTime startTime = new DateTime(date.Year, date.Month, date.Day, accessPeriod.StartHour ?? 0, accessPeriod.StartMinute ?? 0, 0);
            DateTime endTime = new DateTime(date.Year, date.Month, date.Day, accessPeriod.EndHour ?? 0, accessPeriod.EndMinute ?? 0, 0);

            //Check if term already booked
            var res = booked.Where(r => r.StartTime == startTime && r.EndTime == endTime).FirstOrDefault();
            if (res != null)
                return res;

            //Check if access period date expired
            if (!((accessPeriod.FromDate != null && accessPeriod.FromDate <= startTime) || (accessPeriod.ToDate != null && accessPeriod.ToDate > endTime)))
            {
                return null;
            }

            //Check if collaps with inactive date
            foreach (var inact in inactive)
            {
                if (AreDatesCollapsing(inact, new Reservation() { StartTime = startTime, EndTime = endTime }))
                    return inact;
            }

            return new Reservation()
            {
                StartTime = startTime,
                EndTime = endTime,
                Status = ReservationStatus.NotBooked,
                AccessPeriodId = accessPeriod.AccessPeriodId,
                
            };

        }
    }
}
