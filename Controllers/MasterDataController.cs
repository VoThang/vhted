using Autofac;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VHTED.Api.Service.Service;

namespace VHTED.Api.Controllers
{
    [Authorize]
    [Route("api/master-data")]
    [ApiController]
    public class MasterDataController : ControllerBase
    {
        private readonly ILifetimeScope _scope;

        public MasterDataController(ILifetimeScope scope)
        {
            _scope = scope;
        }

        [Route("discharge-instructions")]
        [HttpGet]
        public IActionResult GetDischargeInstructions()
        {
            var dischargeService = _scope.Resolve<IDischargeInstructionMasterDataService>();
            var dischargeInstructions = dischargeService.GetMasterDatas();

            return Ok(dischargeInstructions);
        }

        [Route("system")]
        [HttpGet]
        public IActionResult GetMasterDataForIosMobile()
        {
            var masterDataService = _scope.Resolve<IMasterDataService>();
            var masterDataOfSystem = masterDataService.GetMasterDataOfSystem();
            return Ok(masterDataOfSystem);
        }
    }
}