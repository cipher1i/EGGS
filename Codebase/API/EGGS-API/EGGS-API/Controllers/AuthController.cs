﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.Models;
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
            try
            {
                using (UserModel user = new UserModel(_repoReaper))
                {
                    if (!user.Auth(username, password))
                    {
                        user.Email = username;
                        user.Password = password;
                        if (!user.Create())
                            throw new Exception();
                    }

                    user.Read(username, password);

                    return Ok(user.Email);
                }   
            }
            catch(Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }
    }
}
