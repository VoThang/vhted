using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VHTED.Api.Infrastructure.Common;
using VHTED.Api.Model;
using VHTED.Api.Models.IosMobile;
using VHTED.Api.Service.Service;

namespace VHTED.Api.Controllers
{
    [Authorize]
    [Route("api/device")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly ILifetimeScope _scope;

        public DeviceController(ILifetimeScope scope)
        {
            _scope = scope;
        }

        [Route("regis_device_token")]
        [HttpPost]
        public async Task<IActionResult> RegisDeviceToken([FromBody] SendUserDeviceTokenModel model)
        {
            var currentPatientId = await GetCurrentPatientId();
            var deviceService = _scope.Resolve<IDeviceService>();
            await deviceService.RegisDeviceToken(new DeviceInfoModel()
            {
                OwnerType = OwnerType.Patient,
                OnwerId = currentPatientId,
                DeviceToken = model.DeviceToken
            });

            return Ok();
        }

        private async Task<int> GetCurrentPatientId()
        {
            var username = User.Identity.Name;
            var patientService = _scope.Resolve<IPatientService>();
            return await patientService.GetPatientId(username);
        }
    }
}