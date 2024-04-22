using System.ComponentModel.DataAnnotations;

namespace GBC_Travel_Group_50.Models
{
    public class Hotel
    {
        [Key]
        public int HotelID { get; set; }
        public string HotelName { get; set; }
        public string Location { get; set; }
        public float PricePerNight { get; set; }
        public string Amenities { get; set; }
        public bool Availability { get; set; }
        public int Rating { get; set; }
    }
}
