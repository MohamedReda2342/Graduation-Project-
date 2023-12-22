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

    }

        public class MedicineUpdateRequest
        {
            public string MedicineName { get; set; }
            public string Date { get; set; }
            public string Time { get; set; }
            public string Repeat { get; set; }
            public string Reminder { get; set; }
            public int? NumOfDays { get; set; }

    }






}