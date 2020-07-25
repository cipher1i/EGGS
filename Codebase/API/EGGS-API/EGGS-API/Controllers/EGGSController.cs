using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Extensions;
using Domain.Models.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace EGGS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EGGSController : ControllerBase
    {
        private MemoryStream memStream = new MemoryStream();

        [HttpGet]
        public IActionResult GetHome()
        {
            return Ok("Welcome to EGGS API!");
        }

        [HttpGet("download"), DisableRequestSizeLimit]
        public async Task<FileStream> GetDownload()
        {
            var file = Path.Combine(Directory.GetCurrentDirectory() + "/../../../Data", "EGGS.zip");
            
            return new FileStream(file, FileMode.Open, FileAccess.Read);
        }

        [HttpPost("upload"), DisableRequestSizeLimit]
        public IActionResult PostUpload()
        {
            /*
                *DECRYPTION
                fileContent = EGGSUtility.Decrypt(fileContent);
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(fileContent);
                    fs.Write(info, 0, info.Length);
                }
            */
            try
            {
                var files = Request.Form.Files;
                if (files.Any(f => f.Length == 0))
                    return BadRequest();

                var savePath = Path.Combine(Directory.GetCurrentDirectory() + "/../../..", "Resources");  //path to save files to after new directory created
                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);

                Queue<string> names = new Queue<string>();
                string contents = "";
                foreach (var file in files)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
                    names.Enqueue(fileName.ToString());
                    var filePath = Path.Combine(savePath, fileName.ToString());
                    
                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fs);
                    }
                }

                string fileContent = "";
                using (MemoryStream ms = new MemoryStream())
                {
                    foreach (var filePath in Directory.GetFiles(savePath))
                    {
                        contents = System.IO.File.ReadAllText(filePath);

                        fileContent = EGGSUtility.Encrypt(filePath, contents);
                        using (FileStream fs = new FileStream(filePath, FileMode.Create))
                        {
                            byte[] info = new UTF8Encoding(true).GetBytes(fileContent);
                            fs.Write(info, 0, info.Length);
                        }

                        using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                        {
                            if(names.Count > 0)
                            {
                                var f = archive.CreateEntry(names.Dequeue());
                                using (var sw = new StreamWriter(f.Open()))
                                {
                                    sw.Write(fileContent);
                                }
                            }
                        }
                    }

                    ZipFile.CreateFromDirectory(savePath, savePath+"/../Data/EGGS.zip");
                    memStream = ms;
                    return Ok();
                    //return File(ms.ToArray(), "application/zip", "Directory.zip");
                    //return new FileStream(Path.Combine(savePath, "Directory0.zip"), FileMode.Open, FileAccess.Read);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e}");
            }
        }
    }
}
