//// Inside MedicineService

//using AutoMapper;
//using Microsoft.EntityFrameworkCore;
//using WebApi.Entities;
//using WebApi.Helpers;
//using WebApi.Models;
//using static WebApi.Models.PatientUpdateRequest;

//public interface IMedicineService
//{
//    IEnumerable<Medicine> GetMedicinesByPatientId(int patientId);
//    Medicine GetMedicineById(int patientId, int medicineId);
//    void AddMedicine(int patientId, MedicineAddRequest model);
//    void UpdateMedicine(int patientId, int medicineId, MedicineUpdateRequest model);
//    void DeleteMedicine(int patientId, int medicineId);
//}
//public class MedicineService : IMedicineService
//{
//    private readonly DataContext _context;
//    private readonly IMapper _mapper;




//    public MedicineService(DataContext context, IMapper mapper)
//    {
//        _context = context;
//        _mapper = mapper;
//    }




//    public IEnumerable<Medicine> GetMedicinesByPatientId(int patientId)
//    {
//        var medicines = _context.Medicines.Where(m => m.PatientId == patientId).ToList();
//        return medicines;
//    }



//    public Medicine GetMedicineById(int patientId, int medicineId)
//    {
//        var medicine = _context.Medicines.FirstOrDefault(m => m.PatientId == patientId && m.MedicineId == medicineId);
//        if (medicine == null)
//        {
//            throw new KeyNotFoundException("Medicine not found");
//        }
//        return medicine;
//    }

//    public void AddMedicine(int patientId, MedicineAddRequest model)
//    {
//        var patient = getPatient(patientId);

//        var medicine = _mapper.Map<Medicine>(model);
//        medicine.PatientId = patientId;

//        patient.Medicines.Add(medicine);
//        _context.SaveChanges();
//    }

//    public void UpdateMedicine(int patientId, int medicineId, MedicineUpdateRequest model)
//    {
//        var patient = getPatient(patientId);
//        var medicine = getMedicine(patient, medicineId);

//        _mapper.Map(model, medicine);

//        _context.SaveChanges();
//    }

//    public void DeleteMedicine(int patientId, int medicineId)
//    {
//        var patient = getPatient(patientId);
//        var medicine = getMedicine(patient, medicineId);

//        patient.Medicines.Remove(medicine);
//        _context.SaveChanges();
//    }


//    //------------------------------------------------ ...Helper method... ------------------------------------------------


//    private Patient getPatient(int patientId)
//    {
//        var patient = _context.Patients
//            .Include(p => p.Medicines) // Ensure Medicines are loaded
//            .FirstOrDefault(p => p.PatientId == patientId);

//        if (patient == null)
//        {
//            throw new KeyNotFoundException("Patient not found");
//        }

//        return patient;
//    }

//    private Medicine getMedicine(Patient patient, int medicineId)
//    {
//        var medicine = patient.Medicines.FirstOrDefault(m => m.MedicineId == medicineId);

//        if (medicine == null)
//        {
//            throw new KeyNotFoundException("Medicine not found");
//        }

//        return medicine;
//    }
//}
