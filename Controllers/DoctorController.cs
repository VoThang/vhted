using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VHTED.Api.Service.Service;

namespace VHTED.Api.Controllers
{
    [Authorize]
    [Route("api/doctor")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly ILifetimeScope _scope;

        public DoctorController(ILifetimeScope scope)
        {
            _scope = scope;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetDoctors()
        {
            var doctorService = _scope.Resolve<IDoctorService>();
            var doctors = doctorService.GetDoctors();
            return Ok(doctors);
        }

        [Route("{id}")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetDoctor(int id)
        {
            var doctorService = _scope.Resolve<IDoctorService>();
            var doctor = doctorService.GetDoctor(id);
            return Ok(doctor);
        }
    }
}