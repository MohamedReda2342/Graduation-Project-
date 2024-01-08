using AutoMapper;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Services
{

    public interface IPatientService
    {
        // Existing patient-related methods
        List<PatientResponse> GetPatientsByUserId(int userId);
        PatientResponse GetPatientById(int userId, int patientId);
        void AddPatient(int userId, PatientAddRequest model);
        void UpdatePatient(int userId, int patientId, PatientUpdateRequest model);
        void DeletePatient(int userId, int patientId);

        // Medicine-related methods
        List<MedicineResponse> GetMedicinesByPatientId(int userId, int patientId);
        MedicineResponse GetMedicineById(int userId, int patientId, int medicineId);
        void AddMedicine(int userId, int patientId, MedicineAddRequest model);
        void UpdateMedicine(int userId, int patientId, int medicineId, MedicineUpdateRequest model);
        void DeleteMedicine(int userId, int patientId, int medicineId);
        void UpdateBand(int userId, int patientId, BandData model);

        // Notifications 
        void SendMedicineNotifications();
        void SendFcmNotification(string deviceToken, string title, string body);
        List<DateTime> GetDoseTimesToNotify(Medicine medicine);
        void AddDoseTimesForDay(Medicine medicine, List<DateTime> doseTimes);
    }


    public class PatientServices : IPatientService
    {

        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public PatientServices(DataContext context, IMapper mapper, IBackgroundJobClient backgroundJobClient)
        {
            _context = context;
            _mapper = mapper;
            _backgroundJobClient = backgroundJobClient;
        }


        #region Get All & Get one patient
        public List<PatientResponse> GetPatientsByUserId(int userId)
        {
            var user = getUser(userId);

            // Map Patient entities to PatientResponse objects
            var patientResponses = _mapper.Map<List<PatientResponse>>(user.Patients);

            return patientResponses;
        }

        public PatientResponse GetPatientById(int userId, int patientId)
        {
            var user = getUser(userId);
            var patient = getPatient(user, patientId);
            // Map Patient entity to PatientResponse object
            var patientResponse = _mapper.Map<PatientResponse>(patient);

            return patientResponse;
        }
        #endregion

        public void AddPatient(int userId, PatientAddRequest model)
        {

            var user = getUser(userId);
            //Check if the patient with the same details already exists
            if (user.Patients.Any(p => p.PhoneNumber == model.PhoneNumber))
                throw new ApplicationException("Patient with the same details already exists.");

            var patient = _mapper.Map<Patient>(model);
            user.Patients.Add(patient);



            if (model.Photo != null)
            {
                //upload to azure but does not work
                //Save New Photo and save its name in the database
                var filePath = Path.Combine("assets", model.Photo.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
                // Set the relative path to the database
                patient.Photo = $"https://elderpeopleband.scm.azurewebsites.net/api/vfs/site/wwwroot/{filePath}";
            }


            _context.SaveChanges();
        }

        public void UpdatePatient(int userId, int patientId, PatientUpdateRequest model)
        {
            var user = getUser(userId);
            var patient = getPatient(user, patientId);

            if (model.Photo != null)
            {
                // Delete old photo from server 
                if (!string.IsNullOrEmpty(patient.Photo))
                {
                    if (File.Exists(patient.Photo))
                    {
                        File.Delete(patient.Photo);
                    }
                }
                //Save New Photo and save its name in the database
                var filePath = Path.Combine("assets", model.Photo.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
                // Set the relative path to the database
                patient.Photo = $"https://elderpeopleband.scm.azurewebsites.net/api/vfs/site/wwwroot/{filePath}";
            }

            // Update patient properties if provided, otherwise keep the existing values
            patient.Name = model.Name ?? patient.Name;
            patient.Age = model.Age ?? patient.Age;
            patient.Gender = model.Gender ?? patient.Gender;
            patient.PhoneNumber = model.PhoneNumber ?? patient.PhoneNumber;
            patient.Illness = model.Illness ?? patient.Illness;

            _context.SaveChanges();
        }

        public void DeletePatient(int userId, int patientId)
        {
            var user = getUser(userId);
            var patient = getPatient(user, patientId);

            user.Patients.Remove(patient);
            _context.SaveChanges();

        }


        //------------------------------------------------ ... Band ... ------------------------------------------------


        public void UpdateBand(int userId, int patientId, BandData model)
        {
            var user = getUser(userId);
            var patient = getPatient(user, patientId);
            // New properties in the PatientUpdateRequest
            patient.Temperature = model.Temperature ?? patient.Temperature;
            patient.O2 = model.O2 ?? patient.O2;
            patient.HeartRate = model.HeartRate ?? patient.HeartRate;
            patient.Longitude = model.Longitude ?? patient.Longitude;
            patient.Latitude = model.Latitude ?? patient.Latitude;
            patient.SafeZoneLatitude = model.SafeZoneLatitude ?? patient.SafeZoneLatitude;
            patient.SafeZoneLongitude = model.SafeZoneLongitude ?? patient.SafeZoneLongitude;
            patient.Radius = model.Radius ?? patient.Radius;

            _context.SaveChanges();
        }

        //------------------------------------------------ ...CRUD operations for Medicine... ------------------------------------------------
        #region Medicine 
        public List<MedicineResponse> GetMedicinesByPatientId(int userId, int patientId)
        {
            var user = getUser(userId);
            var patient = getPatient(user, patientId);
            var medicineResponses = _mapper.Map<List<MedicineResponse>>(patient.Medicines);
            if (!patient.Medicines.Any())
                throw new KeyNotFoundException("Medicines not found");
            return medicineResponses;
        }

        public MedicineResponse GetMedicineById(int userId, int patientId, int medicineId)
        {
            var user = getUser(userId);
            var patient = getPatient(user, patientId);
            var medicineResponses = _mapper.Map<MedicineResponse>(getMedicine(patient, medicineId));
            return medicineResponses;
        }

        public void AddMedicine(int userId, int patientId, MedicineAddRequest model)
        {
            var user = getUser(userId);
            var patient = getPatient(user, patientId);

            if (patient != null)
            {
                var medicine = _mapper.Map<Medicine>(model);
                patient.Medicines.Add(medicine);
            }
            else
            {
                // Handle the case where patient is null
                throw new ArgumentNullException(nameof(patient));
            }
            _context.SaveChanges();
        }

        public void UpdateMedicine(int userId, int patientId, int medicineId, MedicineUpdateRequest model)
        {
            var user = getUser(userId);
            var patient = getPatient(user, patientId);
            var medicine = getMedicine(patient, medicineId);

            //// Use AutoMapper to map the properties from the DTO to the entity
            //_mapper.Map(model, medicine);
            if (medicine == null)
            {
                throw new ArgumentNullException(nameof(patient));

            }
            medicine.MedicineName = model.MedicineName ?? medicine.MedicineName;

            medicine.StartDate = model.StartDate ?? medicine.StartDate;
            medicine.EndDate = model.EndDate ?? medicine.EndDate;

            medicine.Time1 = model.Time1 ?? medicine.Time1;
            medicine.Time2 = model.Time2 ?? medicine.Time2;
            medicine.Time3 = model.Time3 ?? medicine.Time3;
            medicine.Time4 = model.Time4 ?? medicine.Time4;

            medicine.Saturday = model.Saturday ?? medicine.Saturday;
            medicine.Sunday = model.Sunday ?? medicine.Sunday;
            medicine.Monday = model.Monday ?? medicine.Monday;
            medicine.Tuesday = model.Tuesday ?? medicine.Tuesday;
            medicine.Wednesday = model.Wednesday ?? medicine.Wednesday;
            medicine.Thursday = model.Thursday ?? medicine.Thursday;
            medicine.Friday = model.Friday ?? medicine.Friday;


            medicine.Repeat = model.Repeat ?? medicine.Repeat;
            medicine.Reminder = model.Reminder ?? medicine.Reminder;

            //// Use AutoMapper to map the properties from the DTO to the entity
            //_mapper.Map(model, patient);
            _context.Patients.Update(patient);
            _context.SaveChanges();
        }

        public void DeleteMedicine(int userId, int patientId, int medicineId)
        {
            var user = getUser(userId);
            var patient = getPatient(user, patientId);
            var medicine = getMedicine(patient, medicineId);

            patient.Medicines.Remove(medicine);
            _context.SaveChanges();
        }

        // Helper method for getting Medicine
        private Medicine getMedicine(Patient patient, int medicineId)
        {
            var medicine = patient.Medicines.FirstOrDefault(m => m.MedicineId == medicineId);
            if (medicine == null) throw new KeyNotFoundException("Medicine not found");
            return medicine;
        }

        #endregion

        //------------------------------------------------ ... Notifications... ------------------------------------------------

        #region Notifications

        public void SendMedicineNotifications()
        {
            try
            {
                // Get users that have a non-null DeviceToken
                var usersWithDeviceToken = _context.Users
                    .Where(u => !string.IsNullOrEmpty(u.DeviceToken))
                    .Include(u => u.Patients).ThenInclude(p => p.Medicines) // Make sure Patients are loaded
                    .ToList();

                foreach (var user in usersWithDeviceToken)
                {
                    // Assuming each user has a list of associated patients
                    foreach (var patient in user.Patients)
                    {
                        // Get medicines for the current patient that need notifications
                        var medicines = patient.Medicines
                            .Where(m => DateTime.Parse(m.StartDate) <= DateTime.Now && DateTime.Parse(m.EndDate) >= DateTime.Now)
                            .ToList();

                        foreach (var medicine in medicines)
                        {
                            List<DateTime> DoseTimes = GetDoseTimesToNotify(medicine);
                            if (DoseTimes.Count > 0)
                            {
                                foreach (var doseTime in DoseTimes)
                                {
                                    // Assuming the user's device token is stored in the User entity
                                    var deviceToken = user.DeviceToken;

                                    // Send FCM notification
                                    SendFcmNotification(deviceToken, "Medicine Reminder", $"Time to take {medicine.MedicineName}!");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending medicine notifications: {ex.Message}");

            }

        }

        public void SendFcmNotification(string deviceToken, string title, string body)
        {
            var message = new Message
            {
                Token = deviceToken,
                Notification = new Notification
                {
                    Title = title,
                    Body = body,
                },
            };

            FirebaseMessaging.DefaultInstance.SendAsync(message);
        }



        public List<DateTime> GetDoseTimesToNotify(Medicine medicine)
        {
            var doseTimes = new List<DateTime>();

            if (medicine.Saturday.HasValue && medicine.Saturday.Value && DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                AddDoseTimesForDay(medicine, doseTimes);
            }

            if (medicine.Sunday.HasValue && medicine.Sunday.Value && DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                AddDoseTimesForDay(medicine, doseTimes);
            }

            if (medicine.Monday.HasValue && medicine.Monday.Value && DateTime.Now.DayOfWeek == DayOfWeek.Monday)
            {
                AddDoseTimesForDay(medicine, doseTimes);
            }

            if (medicine.Tuesday.HasValue && medicine.Tuesday.Value && DateTime.Now.DayOfWeek == DayOfWeek.Tuesday)
            {
                AddDoseTimesForDay(medicine, doseTimes);
            }

            if (medicine.Wednesday.HasValue && medicine.Wednesday.Value && DateTime.Now.DayOfWeek == DayOfWeek.Wednesday)
            {
                AddDoseTimesForDay(medicine, doseTimes);
            }

            if (medicine.Thursday.HasValue && medicine.Thursday.Value && DateTime.Now.DayOfWeek == DayOfWeek.Thursday)
            {
                AddDoseTimesForDay(medicine, doseTimes);
            }

            if (medicine.Friday.HasValue && medicine.Friday.Value && DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            {
                AddDoseTimesForDay(medicine, doseTimes);
            }

            return doseTimes;
        }


        // Parse the string into a DateTime object
        public void AddDoseTimesForDay(Medicine medicine, List<DateTime> doseTimes)
        {
            int reminder;
            switch (medicine.Reminder)
            {
                case "None":
                    reminder = 0;
                    break;
                case "On time":
                    reminder = 0;
                    break;
                case "5 mins early":
                    reminder = 5;
                    break;
                case "30 mins early":
                    reminder = 30;
                    break;
                case "1 hr early":
                    reminder = 60;
                    break;
                case "1 day early":
                    reminder = 1440;
                    break;
                default:
                    reminder = 0;
                    break;
            }

            List<string> Times = new List<string>() { medicine.Time1, medicine.Time2, medicine.Time3, medicine.Time4 };

            // Helper function to parse the dose time with reminder
            DateTime ParseDoseTimeWithReminder(string time)
            {
                string fullDateTime = medicine.StartDate + " " + time;
                var doseTime = DateTime.Parse(ParseDoseTime(fullDateTime));
                return doseTime.AddMinutes(-reminder);
            }


            foreach (var time in Times)
            {
                if (!string.IsNullOrEmpty(time))
                {
                    DateTime parsedDoseTime = ParseDoseTimeWithReminder(time);
                    // Check if the parsed dose time represents the current time without seconds
                    if (parsedDoseTime.Hour == DateTime.Now.Hour && parsedDoseTime.Minute == DateTime.Now.Minute)
                    {
                        doseTimes.Add(parsedDoseTime);
                    }
                }
            }

        }

        #region OLD Add doseTimes Function
        // Helper function to get reminder minutes based on the selected reminder option


        //public void AddDoseTimesForDay(Medicine medicine, List<DateTime> doseTimes) 
        //{
        //    // Convert the medicine times to DateTime and add them to the list
        //    if (!string.IsNullOrEmpty(medicine.Time1))
        //    {
        //        doseTimes.Add(DateTime.Parse(ParseDoseTime(medicine.StartDate + " " + medicine.Time1)));
        //    }

        //    if (!string.IsNullOrEmpty(medicine.Time2))
        //    {
        //        doseTimes.Add(DateTime.Parse(ParseDoseTime(medicine.StartDate + " " + medicine.Time2)));
        //    }

        //    if (!string.IsNullOrEmpty(medicine.Time3))
        //    {
        //        doseTimes.Add(DateTime.Parse(ParseDoseTime(medicine.StartDate + " " + medicine.Time3)));
        //    }

        //    if (!string.IsNullOrEmpty(medicine.Time4))
        //    {
        //        doseTimes.Add(DateTime.Parse(ParseDoseTime(medicine.StartDate + " " + medicine.Time4)));
        //    }
        //} 
        #endregion



        #endregion

        //------------------------------------------------ ...Helper method... ------------------------------------------------

        private User getUser(int userId)
        {
            var user = _context.Users
                .Include(u => u.Patients).ThenInclude(p => p.Medicines) // Make sure Patients are loaded
                .FirstOrDefault(u => u.Id == userId);

            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }

        private Patient getPatient(User user, int patientId)
        {
            //var patient = user.Patients.FirstOrDefault(p => p.PatientId == patientId);

            var patient = _context.Users
                .Where(u => u.Id == user.Id)  // Ensure we are working with the correct user
                .SelectMany(u => u.Patients)  // Flatten the Patients list
                .Include(p => p.Medicines)    // Eager load the Medicines property
                .FirstOrDefault(p => p.PatientId == patientId);

            if (patient == null) throw new KeyNotFoundException("Patient not found");
            return patient;
        }

        public string ParseDoseTime(string input)
        {
            //EX :  "2024-01-07T00:00:00.000 1:36";
            // Split the input string into date/time and duration parts
            string[] parts = input.Split(' ');

            // Parse the date/time part
            DateTime dateTime = DateTime.ParseExact(parts[0], "yyyy-MM-ddTHH:mm:ss.fff", null);

            // Parse the duration part
            if (parts.Length > 1)
            {
                string durationString = parts[1];
                TimeSpan duration = ParseDuration(durationString);
                dateTime = dateTime.Add(duration);
            }
            string d = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            return d;
        }

        TimeSpan ParseDuration(string durationString)
        {
            // Split the duration string into hours and minutes
            string[] timeParts = durationString.Split(':');

            if (timeParts.Length != 2)
            {
                throw new FormatException("Invalid duration format");
            }

            int hours = int.Parse(timeParts[0]);
            int minutes = int.Parse(timeParts[1]);

            // Create a TimeSpan object representing the duration
            return new TimeSpan(hours, minutes, 0);
        }


        //-------------------------------------------------------------------------------------

    }
}