using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class PatientUpdateRequest
    {

        public string Name { get; set; }
        public int? Age { get; set; }

        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Illness { get; set; }
        public IFormFile Photo { get; set; }

    }

    public class MedicineUpdateRequest
    {

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