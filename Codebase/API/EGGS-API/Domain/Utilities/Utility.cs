
using System;

namespace Domain.Utilities
{
    public static class Utility
    {
        public static string GenerateUniqueString(int n = 20)
        {
            int ascii = 0;
            string uniqueStringID = "";
            Random random = new Random();
            for (int i = 0; i < n; i++)
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
            return uniqueStringID;
        }
    }
}
