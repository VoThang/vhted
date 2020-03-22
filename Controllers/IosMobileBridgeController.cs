using Autofac;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VHTED.Api.Models.IosMobile;
using VHTED.Api.Service.Service;

namespace VHTED.Api.Controllers
{
    // this api will be removed in the future (mobile --> flutter)
    [Route("api/ios_mobile")]
    [ApiController]
    public class IosMobileBridgeController : ControllerBase
    {
        private readonly ILifetimeScope _scope;

        public IosMobileBridgeController(ILifetimeScope scope)
        {
            _scope = scope;
        }

        [Route("doctor/get_username")]
        [HttpPost]
        public IActionResult GetDoctorUsernameInfo(GetAccountInfoOfDoctorRequest request)
        {
            var doctorService = _scope.Resolve<IDoctorService>();
            var doctor = doctorService.GetDoctor(request.DoctorId);

            return Ok(new
            {
                UserId = doctor.UserId,
                Username = doctor.Username
            });
        }

        [Route("patient/get_twilio_credential")]
        [HttpPost]
        public IActionResult GetTwilioCredential(GetTwilioCredentialRequest request)
        {
            var callTransactionService = _scope.Resolve<ICallTransactionService>();
            var callTransaction = callTransactionService.GetByTransactionId(request.CallTransactionId);

            if (callTransaction != null)
            {
                return Ok(new GetTwilioCredentialResponse
                {
                    RoomId = callTransaction.RoomId,
                    PatientToken = callTransaction.PatientToken
                });
            }

            return Ok(new GetTwilioCredentialResponse());
        }

        [Route("patient/stop_call")]
        [HttpPost]
        public IActionResult StopCall(GetTwilioCredentialRequest request)
        {
            var callTransactionService = _scope.Resolve<ICallTransactionService>();
            callTransactionService.StopCall(request.CallTransactionId);
            return Ok();
        }
    }
}