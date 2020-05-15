using BookAndPlay_API.Models;
using Nancy.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BookAndPlay_API.Helpers
{
    public class LocalizationHelper
    {
        public static AddressJsonModel GetAddress(double lat, double lon)
        {
            string s_lat = lat.ToString().Replace(",", ".");
            string s_lon = lon.ToString().Replace(",", ".");

            string url = "https://nominatim.openstreetmap.org/reverse?format=json&lat=" + s_lat + "&lon=" + s_lon;
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

                var json = wc.DownloadString(url);
                var JSONObj = new JavaScriptSerializer().Deserialize<AddressJsonModel>(json);
                return JSONObj;
            }
        }
    }
}
