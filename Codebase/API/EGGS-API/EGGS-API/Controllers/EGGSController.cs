using System;
using System.IO;
using System.Linq;
using Domain.Extensions;
using Domain.Models.Utilities;
using Domain.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace EGGS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EGGSController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetHome()
        {
            return Ok("Welcome to EGGS API!");
        }

        [HttpGet("download"), DisableRequestSizeLimit]
        public IActionResult GetDownload([FromQuery] string userKey)
        {
            try
            {
                string zipLocation = $"{Directory.GetCurrentDirectory()}/../../../Data";
                string zipName = $"EGG-{userKey}.zip";

                var file = Path.Combine(zipLocation, zipName);
                return Ok(new FileStream(file, FileMode.Open, FileAccess.Read));
            }
            catch(UnauthorizedAccessException e)
            {
                return StatusCode(409, $"Conflict: {e.Message}");
            }
            catch(IOException e)
            {
                return StatusCode(404, $"Not found: {e.Message}");
            }
            catch(Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        /*
            DECRYPTION
            fileContent = EGGSUtility.Decrypt(fileContent);
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(fileContent);
                fs.Write(info, 0, info.Length);
            }
        */
        [HttpPost("upload"), DisableRequestSizeLimit]
        public IActionResult PostUpload()
        {
            try
            {
                /* VALIDATE FILES */
                var files = Request.Form.Files;
                if (files.Any(f => f.Length == 0))
                    return BadRequest();

                /* CREATE TEMP SAVE PATH */
                var path = Path.Combine($"{Directory.GetCurrentDirectory()}/../../..", "Resources");
                string savePath = FileUtility.MakeUniqueDirectory(path, 0);
                bool decrypted = false;
                foreach (var file in files)
                {
                    /* READ FILE CONTENTS */
                    var contents = file.ReadAsList();

                    /* ENCRYPT FILE CONTENTS */
                    string convertedContent;
                    if (contents[0] == "Skrambled EGG")
                    {
                        convertedContent = EGGSUtility.DecryptContent(contents);
                        decrypted = true;
                    }
                    else
                        convertedContent = EGGSUtility.EncryptContent(contents);

                    /* SAVE ENCRYPTION TO NEW FILE */
                    var filePath = Path.Combine(savePath, file.FileName);
                    FileUtility.SaveToFile(filePath, convertedContent);
                }

                /* GENERATE RANDOM UNIQUE STRING TO IDENTITY ZIP PER USER */
                string pathToZip = Path.Combine(savePath, "../../Data");
                string ID = FileUtility.MakeUniqueZip("EGG", savePath, pathToZip);

                /* DISPOSE OF TEMP RESOURCES THAT WERE ADDED TO CREATE ZIP */
                Directory.Delete(savePath, true);

                if (decrypted)
                    ID += "-Decrypted";

                return StatusCode(201, ID);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [HttpDelete]
        public IActionResult DeleteData([FromQuery] string userKey)
        {
            try
            {
                string zipLocation = $"{Directory.GetCurrentDirectory()}/../../../Data";
                string zipName = $"EGG-{userKey}.zip";

                var file = Path.Combine(zipLocation, zipName);
                System.IO.File.Delete(file);
                return NoContent();
            }
            catch(UnauthorizedAccessException e)
            {
                return StatusCode(409, $"Conflict: {e.Message}");
            }
            catch(IOException e)
            {
                return StatusCode(404, $"Not found: {e.Message}");
            }
            catch(Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }
    }
}
