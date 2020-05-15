using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BookNadPlay_API.Helpers
{
    public class DataHelper
    {
        public static string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        public static bool CheckTime(int startHour, int startMinute, int endHour, int endMinute)
        {
            if (startHour > 23 || startHour < 0 || endHour > 23 || endHour < 0)
                return false;

            if (startMinute > 59 || startMinute < 0 || endMinute > 59 || endMinute < 0)
                return false;

            if (startHour > endHour || (startHour == endHour && startMinute > endMinute))
                return false;


            return true;
        }
    

        public class Date
        {
            // Find date of day in next 7 days
            public static DateTime GetNextDayOfWeekDate(DayOfWeek dayOfWeek)
            {
                int addedDays = dayOfWeek == DateTime.Today.DayOfWeek ? 7 : 0;
                int day_diff = (((int)dayOfWeek - (int)DateTime.Today.DayOfWeek) % 7 + 7) % 7;
                DateTime nextDayOfWeek = DateTime.Today.AddDays(day_diff + addedDays);

                return nextDayOfWeek;
            }
        }

        public static bool IsTimeOverlapping(DateTime from1, DateTime to1, DateTime from2, DateTime to2)
        {
            if(from1 > from2 && from1 < to2)
            {
                return true;
            }

            if(to1 > from2 && to1 < to2)
            {
                return true;
            }

            return false;
        }


        public static bool IsPhoneNumber(string number)
        {
            return true;
            return Regex.Match(number, @"^(\+[0-9]{9})$").Success;
        }
    }
}
