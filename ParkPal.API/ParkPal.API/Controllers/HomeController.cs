using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using ParkPal.API.Models;

namespace ParkPal.API.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet("IsAlive")]
        public IActionResult IsAlive()
        {
            // Very basic endpoint that we can call to know if the service is alive or not.
            _logger.LogInformation("The IsAlive endpoint was called.");
            return Ok(true);
        }
    }
}
