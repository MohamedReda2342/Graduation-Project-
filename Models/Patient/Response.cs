﻿namespace WebApi.Models
{
    public class PatientResponse
    {
        public string Name { get; set; }
        public int? Age { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Illness { get; set; }

        public double? Temperature { get; set; }
        public double? O2 { get; set; }
        public double? HeartRate { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }

        // Response models for related entities
        public List<MedicineResponse> Medicines { get; set; }
    
    }


    public class MedicineResponse
    {
        public string MedicineName { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public int Repeat { get; set; }
        public string Reminder { get; set; }
    }


    
}