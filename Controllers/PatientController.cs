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
        _patientService = patientService;
        _mapper = mapper;
    }


    // Done
    [HttpPost("AddPatient")]
    public IActionResult AddPatient([FromForm] int userId,[FromForm] PatientAddRequest model)
    {
        _patientService.AddPatient(userId, model);
        return Ok(new { message = "Patient added successfully" });
    }
        
    // Done
    [HttpGet("get-all-patients-of-user")]
    public IActionResult GetPatientsByUserId([FromQuery] int userId)
    {
        var patients = _patientService.GetPatientsByUserId(userId);
        return Ok(patients);
    }


    [HttpGet("Get-Specific-Patient")]
    public IActionResult GetPatientById([FromQuery] int userId, [FromQuery] int patientId)
    {
        var patient = _patientService.GetPatientById(userId, patientId);
        return Ok(patient);
    }


    [HttpPut("UpdatePatient")]
    public IActionResult UpdatePatient([FromForm] int userId, [FromForm] int patientId, [FromForm] PatientUpdateRequest model)
    {
        _patientService.UpdatePatient(userId, patientId, model);
        return Ok(new { message = "Patient updated successfully" });
    }


    [HttpDelete("DeletePatient")]
    public IActionResult DeletePatient([FromForm] int userId, [FromForm] int patientId)
    {
        _patientService.DeletePatient(userId, patientId);
        return Ok(new { message = "Patient deleted successfully" });
    }

    //------------------------------------------------ ... Band ... ------------------------------------------------


    #region
    [HttpPut("UpdateBand")]
    public IActionResult UpdateBand([FromForm] int userId, [FromForm] int patientId, [FromForm] BandData model)
    {
        _patientService.UpdateBand(userId, patientId, model);
        return Ok(new { message = "Band Data updated successfully" });
    }
    #endregion

    //------------------------------------------------ ...CRUD operations for Medicine... ------------------------------------------------

    #region  Medicine
    [HttpPost("AddMedicine")]
    public IActionResult AddMedicine([FromForm] int userId, [FromForm] int patientId, [FromForm] MedicineAddRequest model)
    {
        _patientService.AddMedicine(userId, patientId, model);
        return Ok(new { Message = "Medicine added successfully." });
    }

    [HttpGet("GetAllMedicines")]
    public IActionResult GetMedicines([FromQuery] int userId, [FromQuery] int patientId)
    {
        var medicines = _patientService.GetMedicinesByPatientId(userId, patientId);
        return Ok(medicines);
    }


    [HttpGet("GetSpecificMedicine")]
    public IActionResult GetMedicineById([FromQuery] int userId, [FromQuery] int patientId, [FromQuery] int medicineId)
    {
        var medicine = _patientService.GetMedicineById(userId, patientId, medicineId);
        return Ok(medicine);
    }

    [HttpPut("UpdateMedicine")]
    public IActionResult UpdateMedicine([FromForm] int userId, [FromForm] int patientId, [FromForm] int medicineId, [FromForm] MedicineUpdateRequest model)
    {
        _patientService.UpdateMedicine(userId, patientId, medicineId, model);
        return Ok(new { Message = "Medicine updated successfully." });
    }



    [HttpDelete("DeleteMedicine")]
    public IActionResult DeleteMedicine([FromForm] int userId, [FromForm] int patientId, [FromForm] int medicineId)
    {
        _patientService.DeleteMedicine(userId, patientId, medicineId);
        return Ok(new { Message = "Medicine deleted successfully." });
    }
    #endregion




}
