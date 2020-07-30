using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.IO;
using System.IO.Compression;
using Microsoft.EntityFrameworkCore.Internal;

namespace Domain.Extensions
{
    public static class FileExtensions
    {
        public static List<string> ReadAsList(this IFormFile file)
        {
            var result = new List<string>();
            using(var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    result.Add(reader.ReadLine());
            }
            return result;
        }
    }
}
