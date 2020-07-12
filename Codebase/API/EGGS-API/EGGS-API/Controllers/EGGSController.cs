using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace EGGS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EGGSController : ControllerBase
    {
        
        [HttpGet]
        public IActionResult GetMe()
        {
            return Ok("Welcome to EGGS API!");
        }

        [HttpPost, DisableRequestSizeLimit]
        public IActionResult Upload()
        {
            try
            {
                var files = Request.Form.Files;
                var folder_name = Path.Combine("Resources", "Codebase");
                var path_to_save = Path.Combine(Directory.GetCurrentDirectory(), folder_name);

                if (files.Any(f => f.Length == 0))
                    return BadRequest();

                foreach (var file in files)
                {
                    var file_name = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
                    var full_path = Path.Combine(path_to_save, file_name.ToString());
                    var db_path = Path.Combine(folder_name, file_name.ToString());

                    using (var stream = new FileStream(full_path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                return Ok("All the files were successfully uploaded.");
            }
            catch(Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }
    }
}
