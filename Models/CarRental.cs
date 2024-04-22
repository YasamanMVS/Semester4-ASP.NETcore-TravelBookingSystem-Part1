using System.ComponentModel.DataAnnotations;

namespace GBC_Travel_Group_50.Models
{
    public class CarRental
    {
        [Key]
        public int RentalID { get; set; }
        public string RentalCompany {  get; set; }
        public String RentalModel { get; set; }
        public float PricePerDay { get; set; }
        public string Location { get; set; }
        public bool Availability { get; set; }
    }
}
