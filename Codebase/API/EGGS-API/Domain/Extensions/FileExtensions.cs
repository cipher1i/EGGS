using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Domain.Extensions
{
    public static class FileExtensions
    {
        public static List<string> ReadAsList(this IFormFile file)
        {
            List<string> result = new List<string>();
            using(StreamReader reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    result.Add(reader.ReadLine());
            }
            return result;
        }
    }
}
