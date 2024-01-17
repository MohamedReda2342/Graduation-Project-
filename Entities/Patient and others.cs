using Microsoft.EntityFrameworkCore;

namespace WebApi.Entities
{
    public class Patient
    {
        public int    PatientId { get; set; }
        public int    UserId { get; set; }
        public string Name { get; set; }
        public int?   Age { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Illness { get; set; }

        public double? Temperature { get; set; }
        public double? O2 { get; set; }
        public double? HeartRate { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }

        public double? SafeZoneLatitude { get; set; }
        public double? SafeZoneLongitude { get; set; }
        public double? Radius { get; set; }
        public string  Photo { get; set; }

        public User User { get; set; }
        public List<Medicine> Medicines { get; set; }

    }

    public class Medicine
    {
        public int PatientId { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }

        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public bool? Saturday { get; set; }
        public bool? Sunday { get; set; } 
        public bool? Monday { get; set; } 
        public bool? Tuesday { get; set; } 
        public bool? Wednesday { get; set; }
        public bool? Thursday { get; set; }
        public bool? Friday { get; set; } 



        public string Time1 { get; set; }
        public string Time2 { get; set; }
        public string Time3 { get; set; }
        public string Time4 { get; set; }

        public string Repeat { get; set; }
        public string Reminder { get; set; }





        public Patient Patient { get; set; }

    }
}
