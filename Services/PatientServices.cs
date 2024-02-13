using AutoMapper;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Hangfire;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models;
using System.Threading;


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
        void UpdateBand(BandData model);
        void UpdateBand(int userId, int patientId, BandData model);

        // Notifications 
        void SendMedicineNotifications();
        void SendFcmNotification(string deviceToken, string title, string body);
        bool GetDoseTimesToNotify(Medicine medicine, DateTime executionTime);
        bool AddDoseTimesForDay(Medicine medicine, DateTime executionTime);
    }


    public class PatientServices : IPatientService
    {

        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IWebHostEnvironment env;

        #region Hangfire
        //public PatientServices(DataContext context, IMapper mapper, IBackgroundJobClient backgroundJobClient, IWebHostEnvironment env)
        //{
        //    _context = context;
        //    _mapper = mapper;
        //    _backgroundJobClient = backgroundJobClient;
        //    this.env = env;
        //} 
        #endregion

        public PatientServices(DataContext context, IMapper mapper, IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            this.env = env;
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

        #region Add , Update , Delete Patient
        public void AddPatient(int userId, PatientAddRequest model)
        {

            var user = getUser(userId);
            //Check if the patient with the same details already exists
            if (user.Patients.Any(p => p.PhoneNumber == model.PhoneNumber))
                throw new ApplicationException("Patient with the same details already exists.");

            var patient = _mapper.Map<Patient>(model);

            if (model.Photo != null)
            {
                //Save New Photo and save its name in the database
                #region oldcode
                //var filePath = Path.Combine("assets", model.Photo.FileName);
                //using (var fileStream = new FileStream(filePath, FileMode.Create))
                //{
                //    model.Photo.CopyTo(fileStream);
                //}
                // Set the relative path to the database
                //patient.Photo = $"https://elderpeopleband.scm.azurewebsites.net/api/vfs/site/wwwroot/{filePath}";en
                #endregion
                var FilePath = "Assets\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Millisecond.ToString() + model.Photo.FileName;
                var path = env.WebRootPath + "\\" + FilePath;
                using (FileStream fs = System.IO.File.Create(path))
                {
                    model.Photo.CopyTo(fs);
                }
                patient.Photo = $"https://elderpeopleband.azurewebsites.net/{FilePath}";
            }

            user.Patients.Add(patient);
            _context.SaveChanges();
        }

        public void UpdatePatient(int userId, int patientId, PatientUpdateRequest model)
        {
            var user = getUser(userId);
            var patient = getPatient(user, patientId);

            if (model.Photo != null)
            {
                #region DeleteImage
                //// Delete old photo from server 
                //if (!string.IsNullOrEmpty(patient.Photo))
                //{
                //    if (File.Exists(patient.Photo))
                //    {
                //        File.Delete(patient.Photo);
                //    }
                //} 
                #endregion
                //Save New Photo and save its name in the database
                var FilePath = "Assets\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Millisecond.ToString() + model.Photo.FileName;
                var path = env.WebRootPath + "\\" + FilePath;
                using (FileStream fs = System.IO.File.Create(path))
                {
                    model.Photo.CopyTo(fs);
                }
                patient.Photo = $"https://elderpeopleband.azurewebsites.net/{FilePath}";
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
        #endregion
        //------------------------------------------------ ... Band ... ------------------------------------------------

        public void UpdateBand(BandData model)
        {
            var user = getUser(model.UserId);
            var patient = getPatient(user, model.PatientId);
            // New properties in the PatientUpdateRequest
            patient.Temperature = model.Temperature ?? patient.Temperature;
            if (model.O2 != 0)
            {
                patient.O2 = model.O2 ?? patient.O2;
            }
            patient.HeartRate = model.HeartRate ?? patient.HeartRate;
            patient.Longitude = model.Longitude ?? patient.Longitude;
            patient.Latitude = model.Latitude ?? patient.Latitude;
            patient.SafeZoneLatitude = model.SafeZoneLatitude ?? patient.SafeZoneLatitude;
            patient.SafeZoneLongitude = model.SafeZoneLongitude ?? patient.SafeZoneLongitude;
            patient.Radius = model.Radius ?? patient.Radius;
            patient.Emergerncy = model.Emergerncy;


            _context.SaveChanges();
        }  
        public void UpdateBand(int userId, int patientId, BandData model)
        {
            var user = getUser(model.UserId);
            var patient = getPatient(user, model.PatientId);
            // New properties in the PatientUpdateRequest
            patient.Temperature = model.Temperature ?? patient.Temperature;
            if (model.O2 != 0)
            {
                patient.O2 = model.O2 ?? patient.O2;
            }
            patient.HeartRate = model.HeartRate ?? patient.HeartRate;
            patient.Longitude = model.Longitude ?? patient.Longitude;
            patient.Latitude = model.Latitude ?? patient.Latitude;
            patient.SafeZoneLatitude = model.SafeZoneLatitude ?? patient.SafeZoneLatitude;
            patient.SafeZoneLongitude = model.SafeZoneLongitude ?? patient.SafeZoneLongitude;
            patient.Radius = model.Radius ?? patient.Radius;
            patient.Emergerncy = model.Emergerncy;

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
                if (medicine.Repeat == "Daily")
                {
                    medicine.Saturday = true;
                    medicine.Sunday = true;
                    medicine.Monday = true;
                    medicine.Tuesday = true;
                    medicine.Wednesday = true;
                    medicine.Thursday = true;
                    medicine.Friday = true;
                }
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
            if (medicine.Repeat == "Daily")
            {
                medicine.Saturday = true;
                medicine.Sunday = true;
                medicine.Monday = true;
                medicine.Tuesday = true;
                medicine.Wednesday = true;
                medicine.Thursday = true;
                medicine.Friday = true;
            }
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
        //------------------------------------------------ ... Notifications ... ------------------------------------------------

        #region Notifications


        public void SendMedicineNotifications()
        {
            try
            {
                // Get the execution time of the job
                DateTime executionTime = DateTime.Now;
                executionTime = executionTime.AddHours(2);

                // Get users that have a non-null DeviceToken
                var usersWithDeviceToken = _context.Users
                    .Where(u => !string.IsNullOrEmpty(u.DeviceToken))
                    .Include(u => u.Patients).ThenInclude(p => p.Medicines) // Make sure Patients are loaded
                    .ToList();

                for (int i = 0; i < usersWithDeviceToken.Count; i++)
                {
                    User user = usersWithDeviceToken[i];
                    var deviceToken = user.DeviceToken;

                    foreach (var patient in user.Patients)
                    {
                        #region Notifications of band data

                        if (patient.Emergerncy == true)
                        {
                            SendFcmNotification(deviceToken, "Emergency Alert", $"Patient ({patient.Name}) Needs your help!");
                            patient.Emergerncy = false;
                            _context.SaveChanges();

                        }

                        // Check if patient has geofence coordinates set
                        if (patient.SafeZoneLatitude.HasValue && patient.SafeZoneLongitude.HasValue && patient.Radius.HasValue)
                        {
                            // Check if the patient is outside the geofence
                            if (!IsInsideSafeZone(patient))
                            {
                                SendFcmNotification(deviceToken, "Geofence Alert", $"Patient {patient.Name} is outside the safe zone!");
                            }
                        }
                        // Temperature : 
                        if (patient.Temperature.HasValue && patient.Temperature.Value > 37.0)
                        {
                            SendFcmNotification(deviceToken, "Temperature Alert", $"Temperature is above 37.0°C for {patient.Name}!");
                        }
                        // O2 : 
                        if (patient.O2.HasValue && patient.O2.Value < 95.0)
                        {
                            SendFcmNotification(deviceToken, "Oxygen Alert", $"Oxygen is lower than 95% for {patient.Name}!");
                        }
                        // Hear Rate : 
                        if (patient.HeartRate.HasValue && patient.HeartRate.Value > 100.0)
                        {
                            SendFcmNotification(deviceToken, "HeartRate Alert", $"HeartRate is higher than 100 pbm for {patient.Name}!");
                        }
                        if (patient.HeartRate.HasValue && patient.HeartRate.Value < 60.0)
                        {
                            SendFcmNotification(deviceToken, "HeartRate Alert", $"HeartRate is lower than 60 pbm for {patient.Name}!");

                        }

                        #endregion

                        var medicines = patient.Medicines
                                .Where(m => DateTime.Parse(m.StartDate).Date <= DateTime.Now.Date && DateTime.Parse(m.EndDate).Date >= DateTime.Now.Date)
                                .ToList();

                        foreach (var medicine in medicines)
                        {
                            if (GetDoseTimesToNotify(medicine, executionTime))
                            {
                                SendFcmNotification(deviceToken, "Medicine Reminder", $"Time to take ({medicine.MedicineName}) !");
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending notifications: {ex.Message}");

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

                }
            };

            FirebaseMessaging.DefaultInstance.SendAsync(message);
        }

        public bool GetDoseTimesToNotify(Medicine medicine, DateTime executionTime)
        {
            bool doseFound = false;

            if (medicine.Saturday.HasValue && medicine.Saturday.Value && DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                doseFound = AddDoseTimesForDay(medicine, executionTime);
            }

            else if (medicine.Sunday.HasValue && medicine.Sunday.Value && DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                doseFound = AddDoseTimesForDay(medicine, executionTime);
            }

            else if (medicine.Monday.HasValue && medicine.Monday.Value && DateTime.Now.DayOfWeek == DayOfWeek.Monday)
            {
                doseFound = AddDoseTimesForDay(medicine, executionTime);
            }

            else if (medicine.Tuesday.HasValue && medicine.Tuesday.Value && DateTime.Now.DayOfWeek == DayOfWeek.Tuesday)
            {
                doseFound = AddDoseTimesForDay(medicine, executionTime);
            }

            else if (medicine.Wednesday.HasValue && medicine.Wednesday.Value && DateTime.Now.DayOfWeek == DayOfWeek.Wednesday)
            {
                doseFound = AddDoseTimesForDay(medicine, executionTime);
            }

            else if (medicine.Thursday.HasValue && medicine.Thursday.Value && DateTime.Now.DayOfWeek == DayOfWeek.Thursday)
            {
                doseFound = AddDoseTimesForDay(medicine, executionTime);
            }

            else if (medicine.Friday.HasValue && medicine.Friday.Value && DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            {
                doseFound = AddDoseTimesForDay(medicine, executionTime);
            }

            return doseFound;
        }


        // Parse the string into a DateTime object
        public bool AddDoseTimesForDay(Medicine medicine, DateTime executionTime)
        {
            int reminder;
            switch (medicine.Reminder)
            {
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
            foreach (var time in Times)
            {
                if (!string.IsNullOrEmpty(time))
                {
                    DateTime parsedDoseTime = DateTime.Parse(time);
                    parsedDoseTime = parsedDoseTime.AddMinutes(-reminder);
                    // Check if the parsed dose time represents the current time without seconds
                    if (parsedDoseTime.Hour == executionTime.Hour && parsedDoseTime.Minute == executionTime.Minute)
                    {
                        return true;
                    }
                }
            }
            return false;

        }


        #endregion

        //------------------------------------------------ ...Helper method... ------------------------------------------------

        #region Get User && Get Patient
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

        #endregion

        #region SafeZone

        private bool IsInsideSafeZone(Patient patient)
        {
            // Implement your distance calculation logic, such as Haversine formula
            double distance = CalculateHaversineDistance(
                patient.Latitude.Value, patient.Longitude.Value,
                patient.SafeZoneLatitude.Value, patient.SafeZoneLongitude.Value);

            // Check if the distance is within the safezone radius
            return distance <= (patient.Radius /= 1000);
        }

        // Haversine formula for calculating distance between two points on the Earth
        private double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Earth radius in kilometers

            var dLat = DegToRad(lat2 - lat1);
            var dLon = DegToRad(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegToRad(lat1)) * Math.Cos(DegToRad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // Distance in kilometers
        }

        private double DegToRad(double deg)
        {
            return deg * (Math.PI / 180);
        } 

        #endregion

        //-------------------------------------------------------------------------------------

    }

}