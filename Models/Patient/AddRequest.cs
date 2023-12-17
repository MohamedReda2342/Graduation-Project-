using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class PatientAddRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int? Age { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string Illness { get; set; }
        // i will make another model for them they can't be 
    }
    
    public class BandData
    {
        public double? Temperature { get; set; }
        public double? O2 { get; set; }
        public double? HeartRate { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
    }

    public class MedicineAddRequest
    {
        public string MedicineName { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public int? Repeat { get; set; }
        public string Reminder { get; set; }
    }

   




}
