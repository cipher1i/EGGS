using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.Models;
using Domain.Models.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace EGGS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IEGGSRepositoryUser _repoReaper;
        public AuthController(IEGGSRepositoryUser repoReaper)
        {
            _repoReaper = repoReaper;
        }

        [HttpPost]
        public IActionResult PostUser([FromForm] string username, [FromForm] string password)
        {
            UserModel user = new UserModel(_repoReaper);
            
            if (!user.Auth(username, password))
            {
                user.Email = username;
                user.Password = password;
                if (!user.Create())
                    return StatusCode(500, $"{username} and {password} on create");
            }

            if (!user.Read(username, password))
                return StatusCode(500, $"{username} and {password} on read");
            
            return Ok(user);
        }
    }
}
