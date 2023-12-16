using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Authorization;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Services;

[Authorize]
[ApiController]
[Route("[controller]")]
public class PatientsController : ControllerBase
{
    private IPatientService _patientService;
    private IMapper _mapper;

    public PatientsController(IPatientService patientService,IMapper mapper)
    {
        _mapper = mapper;
    }

    //http://localhost:4000/patients/users/1
    // Done
    [HttpGet("users/{userId}")]
    public IActionResult GetPatientsByUserId(int userId)
    {
        var patients = _patientService.GetPatientsByUserId(userId);
        return Ok(patients);
    }


    //http://localhost:4000/patients/[patientID]/users/[userid]/
    // Done
    [HttpGet("{patientId}/users/{userId}/")]
    public IActionResult GetPatientById(int userId, int patientId)
    {
        var patient = _patientService.GetPatientById(userId, patientId);
        return Ok(patient);
    }

    //http://localhost:4000/patients/users/[userID]
    // Done
    [HttpPost("users/{userId}")]
    public IActionResult AddPatient(int userId, PatientAddRequest model)
    {
        _patientService.AddPatient(userId, model);
        return Ok(new { message = "Patient added successfully" });
    }


    //http://localhost:4000/patients/[patientID]/users/[userid]/
    // Done
    [HttpPut("{patientId}/users/{userId}")]
    public IActionResult UpdatePatient(int userId, int patientId, PatientUpdateRequest model)
    {
        _patientService.UpdatePatient(userId, patientId, model);
        return Ok(new { message = "Patient updated successfully" });
    }


    //http://localhost:4000/patients/[userid]/[patientID]
    // i suggest using Soft Delete in the next update 
    [HttpDelete("{patientId}/users/{userId}")]
    public IActionResult DeletePatient(int userId, int patientId)
    {
        _patientService.DeletePatient(userId, patientId);
        return Ok(new { message = "Patient deleted successfully" });
    }


    //------------------------------------------------ ...CRUD operations for Medicine... ------------------------------------------------

    [HttpGet("{userId}/patients/{patientId}/medicines")]
    public IActionResult GetMedicines(int userId, int patientId)
    {
        var medicines = _patientService.GetMedicinesByPatientId(userId, patientId);
        return Ok(medicines);
    }

    [HttpPost("{userId}/patients/{patientId}/medicines")]
    public IActionResult AddMedicine(int userId, int patientId, MedicineAddRequest model)
    {
        _patientService.AddMedicine(userId, patientId, model);
        return Ok(new { Message = "Medicine added successfully." });
    }


    [HttpGet("{userId}/patients/{patientId}/medicines/{medicineId}")]
    public IActionResult GetMedicineById(int userId, int patientId, int medicineId)
    {
        var medicine = _patientService.GetMedicineById(userId, patientId, medicineId);
        return Ok(medicine);
    }

    [HttpPut("{userId}/patients/{patientId}/medicines/{medicineId}")]
    public IActionResult UpdateMedicine(int userId, int patientId, int medicineId, [FromBody] MedicineUpdateRequest model)
    {
        _patientService.UpdateMedicine(userId, patientId, medicineId, model);
        return Ok(new { Message = "Medicine updated successfully." });
    }



    [HttpDelete("{userId}/patients/{patientId}/medicines/{medicineId}")]
    public IActionResult DeleteMedicine(int userId, int patientId, int medicineId)
    {
        _patientService.DeleteMedicine(userId, patientId, medicineId);
        return Ok(new { Message = "Medicine deleted successfully." });
    }





}
