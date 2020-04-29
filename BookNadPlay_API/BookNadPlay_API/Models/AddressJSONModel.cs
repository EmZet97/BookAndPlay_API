using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAndPlay_API.Models
{
    public partial class AddressJsonModel
    {
        [JsonProperty("place_id")]
        public long PlaceId { get; set; }

        [JsonProperty("licence")]
        public string Licence { get; set; }

        [JsonProperty("osm_type")]
        public string OsmType { get; set; }

        [JsonProperty("osm_id")]
        public long OsmId { get; set; }

        [JsonProperty("lat")]
        public string Lat { get; set; }

        [JsonProperty("lon")]
        public string Lon { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("address")]
        public Address Address { get; set; }

        [JsonProperty("boundingbox")]
        public string[] Boundingbox { get; set; }
    }

    public partial class Address
    {
        [JsonProperty("attraction")]
        public string Attraction { get; set; }

        [JsonProperty("road")]
        public string Road { get; set; }

        [JsonProperty("neighbourhood")]
        public string Neighbourhood { get; set; }

        [JsonProperty("suburb")]
        public string Suburb { get; set; }

        [JsonProperty("city_district")]
        public string CityDistrict { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("municipality")]
        public string Municipality { get; set; }

        [JsonProperty("county")]
        public string County { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("postcode")]
        public string Postcode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }
    }
}
