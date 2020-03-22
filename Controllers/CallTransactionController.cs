using System.Threading.Tasks;
using VHTED.Api.Model;
using VHTED.Api.Service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Autofac;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Hosting;

namespace VHTED.Api.Controllers
{
    [Authorize]
    [Route("api/call")]
    [ApiController]
    public class CallTransactionController : ControllerBase
    {
        private readonly ILifetimeScope _scope;
        private readonly IHostingEnvironment _hostingEnvironment;

        public CallTransactionController(ILifetimeScope scope,
            IHostingEnvironment hostingEnvironment)
        {
            _scope = scope;
            _hostingEnvironment = hostingEnvironment;
        }

        [Route("{callTransactionId}")]
        [HttpGet]
        public IActionResult GetCallTransaction(Guid callTransactionId)
        {
            var callTransactionService = _scope.Resolve<ICallTransactionService>();
            var callTransactionModel = callTransactionService.GetByTransactionId(callTransactionId);

            return Ok(callTransactionModel);
        }

        [Route("patient/request")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateRequestCall([FromBody] EncounterModel encounterModel)
        {
            var callTransactionService = _scope.Resolve<ICallTransactionService>();
            var callTransactionModel = await callTransactionService.CreateCall(encounterModel.PatientUsername, encounterModel.DoctorUsername, encounterModel.Symptoms, encounterModel.Allergies);

            return Ok(new
            {
                TransactionId = callTransactionModel.TransactionId
            });
        }

        [Route("doctor/ready_in_minutes")]
        [HttpPost]
        public async Task<IActionResult> ReadyToCall([FromQuery] Guid callTransactionId, [FromQuery] int minutes)
        {
            var doctorUsername = User.Identity.Name;
            var callTransactionService = _scope.Resolve<ICallTransactionService>();
            await callTransactionService.ReadyToCall(callTransactionId, doctorUsername, minutes);

            return Ok();
        }

        [Route("doctor/technical_issue")]
        [HttpPost]
        public async Task<IActionResult> TechnicalIssue([FromQuery] Guid callTransactionId)
        {
            var callTransactionService = _scope.Resolve<ICallTransactionService>();
            await callTransactionService.TechnicalIssue(callTransactionId);
            return Ok();
        }

        [Route("doctor/unavailable_to_call")]
        [HttpPost]
        public async Task<IActionResult> UnavailableToCall([FromQuery] Guid callTransactionId)
        {
            var callTransactionService = _scope.Resolve<ICallTransactionService>();
            await callTransactionService.UnavailableToCall(callTransactionId);
            return Ok();
        }

        [Route("doctor/pickup")]
        [HttpPost]
        public async Task<IActionResult> Pickup([FromQuery] Guid callTransactionId)
        {
            var doctorUsername = User.Identity.Name;
            var callTransactionService = _scope.Resolve<ICallTransactionService>();
            await callTransactionService.PickupCall(callTransactionId, doctorUsername);
            return Ok();
        }

        [Route("doctor/reject")]
        [HttpPost]
        public async Task<IActionResult> Reject([FromQuery] Guid callTransactionId)
        {
            var doctorUsername = User.Identity.Name;
            var callTransactionService = _scope.Resolve<ICallTransactionService>();
            await callTransactionService.RejectCall(callTransactionId, doctorUsername);
            return Ok();
        }

        [Route("twilio/begin_call")]
        [HttpPost]
        public async Task<IActionResult> BeginCall([FromQuery] Guid callTransactionId)
        {
            var callTransactionService = _scope.Resolve<ICallTransactionService>();
            await callTransactionService.BeginCall(callTransactionId);
            return Ok();
        }

        [Route("doctor/stop_call")]
        public async Task<IActionResult> StopCall([FromQuery] Guid callTransactionId)
        {
            var callTransactionService = _scope.Resolve<ICallTransactionService>();
            await callTransactionService.StopCall(callTransactionId);
            return Ok();
        }

        [Route("update-discharge-instruction")]
        [HttpPost]
        public async Task<IActionResult> UpdateDischargeInstructions([FromBody]DischargeInstructionDataModel model, [FromQuery] Guid callTransactionId)
        {
            var callTransactionService = _scope.Resolve<ICallTransactionService>();
            var dischargeInstructionJson = JsonConvert.SerializeObject(model, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            await callTransactionService.UpdateDischargeInstruction(callTransactionId, dischargeInstructionJson);
            return Ok();
        }

        // TODO: obsolete?
        [Route("discharge-instruction-detail")]
        [HttpGet]
        public IActionResult GEtDischargeInstructions([FromQuery] Guid callTransactionId)
        {
            var callTransactionService = _scope.Resolve<ICallTransactionService>();
            var dischargeInstruction = callTransactionService.GetDischargeInstruction(callTransactionId);
            if (!string.IsNullOrEmpty(dischargeInstruction))
            {
                var dischargeModel = JsonConvert.DeserializeObject<DischargeInstructionDataModel>(dischargeInstruction);
                var dischargeHtml = callTransactionService.FormatDischargeInstructionHtml(_hostingEnvironment.ContentRootPath,
                dischargeModel);
                return Ok(dischargeHtml);
            }
            

            return Ok(string.Empty);
        }

        // TODO: obsolete?
        [Route("by-patient/{patientId}")]
        [HttpGet]
        public IActionResult GetCallTransactionByPatient(int patientId)
        {
            var callTransactionService = _scope.Resolve<ICallTransactionService>();
            var callTransactions = callTransactionService.GetHistoryCallByPatient(patientId);
            return Ok(callTransactions);
        }
    }
}