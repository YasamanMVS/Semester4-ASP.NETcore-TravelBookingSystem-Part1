using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GBC_Travel_Group_50.Models
{
    public enum ServiceType
    {
        Flight,
        Hotel,
        CarRental
    }
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }
        public ServiceType ServiceType { get; set; }
        public int SelectedServiceID { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public double TotalPrice { get; set; }
        public int NumberOfPassengers { get; set; }
    }
}
