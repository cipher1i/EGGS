using System;
using System.IO;
using System.Linq;
using Domain.Extensions;
using Domain.Interfaces;
using Domain.Models;
using Domain.Models.Utilities;
using Domain.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EGGS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EGGSController : ControllerBase
    {
        private readonly IEGGSRepositoryKey _repoReaper;
        public EGGSController(IEGGSRepositoryKey repoReaper)
        {
            _repoReaper = repoReaper;
        }

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

        [HttpPost("upload"), DisableRequestSizeLimit]
        public IActionResult PostUpload([FromQuery] string username)
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
                string pathToZip;

                /* GENERATE RANDOM UNIQUE KEY TO IDENTITY USER AND MAKE ZIP */
                KeyModel keyModel = new KeyModel(_repoReaper);
                string key;
                do
                {
                    key = Utility.GenerateUniqueString();
                } while (keyModel.Read(key));

                foreach (var file in files)
                {
                    /* READ FILE CONTENTS */
                    var contents = file.ReadAsList();

                    /* ENCRYPT FILE CONTENTS WITH PUBLIC KEY */
                    string convertedContent = EGGSUtility.EncryptContent(contents, key.Substring(10));

                    /* SAVE ENCRYPTION TO NEW FILE */
                    var filePath = Path.Combine(savePath, file.FileName);
                    FileUtility.SaveToFile(filePath, convertedContent);
                }

                /* USE PRIVATE KEY TO MAKE UNIQUE ZIP */
                pathToZip = Path.Combine(savePath, "../../Data");
                string ID = FileUtility.MakeUniqueZip("EGG", savePath, pathToZip);

                /* DISPOSE OF TEMP RESOURCES THAT WERE CREATED TO MAKE ZIP */
                Directory.Delete(savePath, true);

                /* ADD THE SUCCESSFUL KEY TO DB */
                keyModel.Email = username.TrimStart('\"').TrimEnd('\"');
                keyModel.Key = key;
                if (!keyModel.Create())
                    return BadRequest("Unable to generate key");

                /* RETURN PRIVATE KEY WITH ZIP ID */
                return StatusCode(201, key.Substring(0,10)+"+"+ID);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [HttpPost("translate"), DisableRequestSizeLimit]
        public IActionResult PostUpload([FromQuery] string username, [FromQuery] string privateKey)
        {
            try
            {
                username = username.TrimStart('\"').TrimEnd('\"');
                privateKey = privateKey.TrimStart('\"').TrimEnd('\"');

                /* VALIDATE FILES */
                var files = Request.Form.Files;
                if (files.Any(f => f.Length == 0))
                    return BadRequest();

                /* CREATE TEMP SAVE PATH */
                var path = Path.Combine($"{Directory.GetCurrentDirectory()}/../../..", "Resources");
                string savePath = FileUtility.MakeUniqueDirectory(path, 0);

                KeyModel keyModel = new KeyModel(_repoReaper);
                foreach (var file in files)
                {
                    /* READ FILE CONTENTS */
                    var contents = file.ReadAsList();

                    /* DECRYPT FILE CONTENTS */
                    string convertedContent = "";
                    if (contents.Count > 1 && contents[0] == "Skrambled EGG")
                    {
                        string key = privateKey + contents[1];
                        bool successfulRead = keyModel.Read(key);
                        if (!successfulRead || keyModel.Email != username)
                            return BadRequest();

                        convertedContent = EGGSUtility.DecryptContent(contents);
                    }
                    else
                        return BadRequest();

                    /* SAVE DECRYPTION TO NEW FILE */
                    var filePath = Path.Combine(savePath, file.FileName);
                    FileUtility.SaveToFile(filePath, convertedContent);
                }

                /* USE KEY TO MAKE UNIQUE ZIP */
                string pathToZip = Path.Combine(savePath, "../../Data");
                string ID = FileUtility.MakeUniqueZip("EGG", savePath, pathToZip);

                /* DISPOSE OF TEMP RESOURCES THAT WERE ADDED TO CREATE ZIP */
                Directory.Delete(savePath, true);

                /* RETURN PRIVATE KEY WITH ZIP ID */
                privateKey += "-Decrypted";
                return StatusCode(201, privateKey+"+"+ID);
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
