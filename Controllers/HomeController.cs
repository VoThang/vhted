using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VHTED.Api.Service.Service;

namespace VHTED.Api.Controllers
{
    [Authorize]
    [Route("api/home")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILifetimeScope _scope;

        public HomeController(ILifetimeScope scope)
        {
            _scope = scope;
        }

        [Route("call/history/week")]
        [HttpGet]
        public IActionResult GetCallHistoriesInWeek()
        {
            // implement later
            return Ok(new
            {

            });
        }

        [Route("call/history/recent")]
        [HttpGet]
        public IActionResult GetCallHistoriesRecent([FromQuery] int limit)
        {
            var dashboardService = _scope.Resolve<IDashboardService>();
            var callRecent = dashboardService.GetCallTransactionRecent(limit);

            return Ok(callRecent);
        }

        [Route("call/history/cnt")]
        [HttpGet]
        public IActionResult GetCallHistoriesCnt()
        {
            var dashboardService = _scope.Resolve<IDashboardService>();
            var callHistoriesCnt = dashboardService.GetCallHistoriesCnt();

            return Ok(callHistoriesCnt);
        }

        [Route("call/history/amount")]
        [HttpGet]
        public IActionResult GetCallHistoriesAmount()
        {
            var dashboardService = _scope.Resolve<IDashboardService>();
            var callHistoriesAmount = dashboardService.GetCallHistoriesAmount();

            return Ok(callHistoriesAmount);
        }

        [Route("call/history/month")]
        [HttpGet]
        public IActionResult GetCallHistoriesByMonth()
        {
            var dashboardService = _scope.Resolve<IDashboardService>();
            var callHistoriesByMonth = dashboardService.GetCallHistoriesByMonth();

            return Ok(callHistoriesByMonth);
        }

        [Route("patient/cnt")]
        [HttpGet]
        public async Task<IActionResult> GetPatientSummary()
        {
            var patientService = _scope.Resolve<IPatientService>();
            var patients = await patientService.GetPatients();

            return Ok(new
            {
                cnt = patients.Count(),
                isIncrease = true
            });
        }

        [Route("doctor/cnt")]
        [HttpGet]
        public IActionResult GetDoctorCnt()
        {
            var doctorService = _scope.Resolve<IDoctorService>();
            var doctors = doctorService.GetDoctors();

            return Ok(new
            {
                cnt = doctors.Count(),
                isIncrease = false
            });
        }
    }
}