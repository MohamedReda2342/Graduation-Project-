//using AutoMapper;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
//using WebApi.Models;
//using WebApi.Services;
//using System.Collections.Generic;
//using static WebApi.Models.PatientUpdateRequest;

//namespace WebApi.Controllers
//{
//    [Authorize]
//    [ApiController]
//    [Route("patients/{patientId}/medicines")]
//    public class MedicinesController : ControllerBase
//    {
//        private readonly IMedicineService _medicineService;
//        private readonly IMapper _mapper;

//        public MedicinesController(IMedicineService medicineService, IMapper mapper)
//        {
//            _medicineService = medicineService;
//            _mapper = mapper;
//        }

//        [HttpGet()]
//        public IActionResult GetMedicines(int patientId)
//        {
//            try
//            {
//                var medicines = _medicineService.GetMedicinesByPatientId(patientId);
//                return Ok(_mapper.Map<IEnumerable<MedicineResponse>>(medicines));
//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(new { message = ex.Message });
//            }
//        }

//        [HttpGet("{medicineId}")]
//        public IActionResult GetMedicine(int patientId, int medicineId)
//        {
//            try
//            {
//                var medicine = _medicineService.GetMedicineById(patientId, medicineId);
//                return Ok(_mapper.Map<MedicineResponse>(medicine));
//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(new { message = ex.Message });
//            }
//        }

//        [HttpPost]
//        public IActionResult AddMedicine(int patientId, [FromBody] MedicineAddRequest model)
//        {
//            try
//            {
//                _medicineService.AddMedicine(patientId, model);
//                return Ok(new { message = "Medicine added successfully" });
//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(new { message = ex.Message });
//            }
//        }

//        [HttpPut("{medicineId}")]
//        public IActionResult UpdateMedicine(int patientId, int medicineId, [FromBody] MedicineUpdateRequest model)
//        {
//            try
//            {
//                _medicineService.UpdateMedicine(patientId, medicineId, model);
//                return Ok(new { message = "Medicine updated successfully" });
//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(new { message = ex.Message });
//            }
//        }

//        [HttpDelete("{medicineId}")]
//        public IActionResult DeleteMedicine(int patientId, int medicineId)
//        {
//            try
//            {
//                _medicineService.DeleteMedicine(patientId, medicineId);
//                return Ok(new { message = "Medicine deleted successfully" });
//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(new { message = ex.Message });
//            }
//        }
//    }
//}




