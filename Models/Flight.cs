using System.ComponentModel.DataAnnotations;

namespace GBC_Travel_Group_50.Models
{
    public class Flight
    {
        [Key]
        public int FlightID { get; set; }
        public string Airline { get; set; }
        public DateTime Departure { get; set; }
        public DateTime Arrival { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public float Price { get; set; }
        public int Capacity { get; set; }
    }
}
