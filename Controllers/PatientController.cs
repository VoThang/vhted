using System;
using System.Threading.Tasks;
using VHTED.Api.Model;
using VHTED.Api.Service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Autofac;
using VHTED.Api.Service.Service.Implement;

namespace VHTED.Api.Controllers
{
    [Authorize]
    [Route("api/patient")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly ILifetimeScope _scope;

        public PatientController(ILifetimeScope scope)
        {
            _scope = scope;
        }

        [HttpGet]
        public async Task<IActionResult> GetPatients()
        {
            var patientService = _scope.Resolve<IPatientService>();
            var patients = await patientService.GetPatients();
            return Ok(patients);
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetPatient(int id)
        {
            var patientService = _scope.Resolve<IPatientService>();
            var patient = await patientService.GetPatient(id);
            return Ok(patient);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody]PatientModel patientModel)
        {
            var patientService = _scope.Resolve<IPatientService>();
            var createdPatient = await patientService.CreatePatient(patientModel);
            return Ok(createdPatient);
        }

        [AllowAnonymous]
        [Route("patient_registration")]
        [HttpPost]
        public async Task<IActionResult> PatientRegistration([FromBody]PatientModel patientModel)
        {
            var patientService = _scope.Resolve<IPatientService>();
            var createdPatient = await patientService.CreatePatient(patientModel);
            return Ok(createdPatient);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePatient([FromBody]PatientModel patientModel)
        {
            var patientService = _scope.Resolve<IPatientService>();
            var updatedPatient = await patientService.UpdatePatient(patientModel);
            return Ok(updatedPatient);
        }

        [Route("medicaton_history")]
        [HttpGet]
        public IActionResult GetMedicationHistories([FromQuery] int patientId)
        {
            var patientService = _scope.Resolve<IPatientService>();
            var medicationHistory = patientService.GetMedicationHistoryOfPatient(patientId);

            return Ok(medicationHistory);
        }

        [Route("call_histories")]
        [HttpGet]
        public IActionResult GetCallHistories([FromQuery] int patientId)
        {
            var callingService = _scope.Resolve<ICallTransactionService>();
            var callHistories = callingService.GetHistoryCallByPatient(patientId);

            return Ok(callHistories);
        }

        // TODO: obsolete?
        [Route("by-patient/last-medication-history/{patientId}")]
        [HttpGet]
        public IActionResult GetLastMedicationHistoryPatient(int patientId)
        {
            var medicationHistoryService = _scope.Resolve<IPatientMedicationHistoryService>();
            var medicationHistory = medicationHistoryService.GetLastPatientMedicationHistory(patientId).Result;
            return Ok(medicationHistory);
        }

        // TODO next
        [AllowAnonymous]
        [Route("by-patient/create-medication-history")]
        [HttpPost]
        public async Task<IActionResult> CreateMedicationHistoryPatient([FromBody] PatientMedicationHistoryModel model)
        {
            var medicationHistoryService = _scope.Resolve<IPatientMedicationHistoryService>();
            await medicationHistoryService.CreatePatientMedicationHistory(model);
            return Ok();
        }
    }
}