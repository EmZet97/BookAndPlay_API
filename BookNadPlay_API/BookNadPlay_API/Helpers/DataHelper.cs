using System;
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
    }
}
