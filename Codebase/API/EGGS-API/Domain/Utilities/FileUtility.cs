using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Domain.Utilities
{
    public static class FileUtility
    {
        public static void SaveToFile(string path, string content)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                byte[] data = new UTF8Encoding(true).GetBytes(content);
                fs.Write(data, 0, data.Length);
            }
        }

        public static string MakeUniqueDirectory(string path, int index)
        {
            string uniquePath;
            do
            {
                uniquePath = Path.Combine(path, index.ToString());
                index++;
            } while (Directory.Exists(uniquePath));
            Directory.CreateDirectory(uniquePath);
            return uniquePath;
        }

        public static string MakeUniqueZip(string zipName, string sourceDirectoryPath, string zipDestinationPath)
        {
            string partialZipPath = Path.Combine(zipDestinationPath, zipName);
            string fullZipPath;
            string uniqueStringID;
            do
            {
                int ascii = 0;
                uniqueStringID = "";
                Random random = new Random();
                for (int i = 0; i < 10; i++)
                {
                    switch (random.Next(3))
                    {
                        case 0:
                            ascii = random.Next(48, 58);
                            break;
                        case 1:
                            ascii = random.Next(65, 91);
                            break;
                        case 2:
                            ascii = random.Next(97, 123);
                            break;
                        default:
                            break;
                    }

                    uniqueStringID += Convert.ToChar(ascii);
                }

                fullZipPath = partialZipPath + "-" + uniqueStringID + ".zip";
            } while (System.IO.File.Exists(Path.Combine(sourceDirectoryPath, fullZipPath)));

            ZipFile.CreateFromDirectory(sourceDirectoryPath, fullZipPath);

            return uniqueStringID;
        }
    }
}
