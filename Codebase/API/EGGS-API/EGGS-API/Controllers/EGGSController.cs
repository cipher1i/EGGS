using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Extensions;
using Domain.Models;
using Domain.Models.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Net.Http.Headers;

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
        public FileStream GetDownload([FromQuery] string userKey)
        {
            //TODO : Get the response body from upload on the client end.
            var file = Path.Combine(Directory.GetCurrentDirectory() + "/../../../Data", "EGG-"+ userKey +".zip");
            return new FileStream(file, FileMode.Open, FileAccess.Read);
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
                var files = Request.Form.Files;
                if (files.Any(f => f.Length == 0))
                    return BadRequest();
                
                var path = Path.Combine(Directory.GetCurrentDirectory() + "/../../..", "Resources");  //path to save files to after new directory created
                int dirIndex = 0;
                string savePath = "";
                do
                {
                    savePath = Path.Combine(path, dirIndex.ToString());
                    dirIndex++;
                } while (Directory.Exists(savePath));
                Directory.CreateDirectory(savePath);

                foreach (var file in files)
                {
                    var contents = file.ReadAsList();
                    string encryptedContent = "";
                    foreach(var line in contents)
                        encryptedContent += line + '\n';

                    encryptedContent = EGGSUtility.Encrypt(encryptedContent);
                    var filePath = Path.Combine(savePath, file.FileName);
                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        byte[] data = new UTF8Encoding(true).GetBytes(encryptedContent);
                        fs.Write(data, 0, data.Length);
                    }
                }

                string randUser = "";
                do
                {
                    int ascii = 0;
                    randUser = "";
                    Random random = new Random();
                    for (int i = 0; i < 10; i++)
                    {
                        /* guaranteed between 0 and 2 so no default necessary */
                        switch (random.Next(3))    //3 randoms to choose from per tuple
                        {
                            case 0:
                                ascii = random.Next(48, 58);    //10 decimals possible
                                break;
                            case 1:
                                ascii = random.Next(65, 91);    //26 uppercase possible
                                break;
                            case 2:
                                ascii = random.Next(97, 123);   //26 lowercase possible
                                break;
                            default:    //keep ascii same
                                break;
                        }

                        randUser += Convert.ToChar(ascii);
                    }
                } while (System.IO.File.Exists(Path.Combine(savePath, savePath + "/../../Data/EGG-" + randUser + ".zip")));
 
                ZipFile.CreateFromDirectory(savePath, savePath + "/../../Data/EGG-"+randUser+".zip");
                Directory.Delete(savePath, true);
                return Ok(randUser);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e}");
            }
        }

        [HttpDelete]
        public IActionResult DeleteData([FromQuery] string userKey)
        {
            var file = Path.Combine(Directory.GetCurrentDirectory() + "/../../../Data", "EGG-" + userKey + ".zip");
            System.IO.File.Delete(file);
            return Ok();
        }
    }
}
