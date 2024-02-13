namespace WebApi.Models
{


    public class PatientResponse
    {
        public int PatientId { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Illness { get; set; }
        public int? Temperature { get; set; }
        public int? O2 { get; set; }
        public int? HeartRate { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }

        public double? SafeZoneLatitude { get; set; }
        public double? SafeZoneLongitude { get; set; }
        public double? Radius { get; set; }

        public string Photo { get; set; }
    }


    public class MedicineResponse
    {
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

    }



}
