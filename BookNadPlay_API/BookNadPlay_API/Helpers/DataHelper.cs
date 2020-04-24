﻿using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
