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
        public IFormFile Photo { get; set; }

    }

    public class BandData
    {
        public int UserId { get; set; }
        public int PatientId { get; set; }
        public double? Temperature { get; set; }
        public double? O2 { get; set; }
        public double? HeartRate { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }

        public double? SafeZoneLatitude { get; set; }
        public double? SafeZoneLongitude { get; set; }
        public double? Radius { get; set; }

    }

    public class MedicineAddRequest
    {
        public string MedicineName { get; set; }

        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public bool? Saturday { get; set; } = false;
        public bool? Sunday { get; set; } = false;
        public bool? Monday { get; set; } = false;
        public bool? Tuesday { get; set; } = false;
        public bool? Wednesday { get; set; } = false;
        public bool? Thursday { get; set; } = false;
        public bool? Friday { get; set; } = false;



        public string Time1 { get; set; }
        public string Time2 { get; set; }
        public string Time3 { get; set; }
        public string Time4 { get; set; }

        public string Repeat { get; set; }
        public string Reminder { get; set; }

    }






}
