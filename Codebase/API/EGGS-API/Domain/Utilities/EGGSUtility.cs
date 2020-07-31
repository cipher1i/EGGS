using Domain.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Utilities
{
    public static class EGGSUtility
    {
        public static string EncryptContent(List<string> content, string publicKey)
        {
            string contents = "";
            foreach (var line in content)
                contents += line + '\n';

            Hashtable hashTable = new Hashtable(contents.Length);

            long i = 0;
            foreach (char c in contents)
            {
                long a = Convert.ToInt32(c);

                bool asIs = (c == '\n' || c == '\t' || c == ' ' || a < 32 || a > 126);

                long hashResult = FofASCII(a);
                long hashIndex = FofIndex(i);
                string resultFinal = EncodeResult(hashResult);
                string indexFinal = EncodeResult(hashIndex);

                long bktIndex = hashResult % contents.Length;
                while (hashTable.ContainsKey(bktIndex) && hashTable.Count < contents.Length)
                    bktIndex = (bktIndex == (contents.Length - 1)) ? ((bktIndex + 1) % contents.Length) : (bktIndex + 1);

                if (hashTable.Count >= contents.Length)
                    break;

                hashTable[bktIndex] = indexFinal + GenerateSymbol() + resultFinal;

                i++;
            }

            string encryptedContent = "Skrambled EGG\n" + publicKey + '\n';

            foreach (DictionaryEntry ht in hashTable)
                encryptedContent += ht.Value + "-";

            return encryptedContent;
        }

        public static string DecryptContent(List<string> content)
        {
            content.RemoveAt(0);
            content.RemoveAt(0);
            string contents = "";
            foreach (var line in content)
                contents += line + '\n';

            string decryptedContent = "";
            SortedDictionary<long, long> dehashedValues = new SortedDictionary<long, long>();

            string[] parsedContent = contents.Split('-');
            foreach (string encryptedPair in parsedContent)
            {
                string decryptedPair = "";
                foreach (char eC in encryptedPair)
                {
                    switch (eC)
                    {
                        case '!':
                        case '@':
                        case '#':
                        case '$':
                        case '%':
                        case '^':
                        case '&':
                        case '*':
                        case '_':
                        case '+':
                        case '=':
                        case '`':
                        case '~':
                        case '|':
                        case ';':
                        case ':':
                        case '?':
                        case ',':
                        case '.':
                            decryptedPair += eC.ToString();
                            continue;
                        default:
                            break;
                    }

                    switch (char.ToLower(eC))
                    {
                        case 'q':
                            decryptedPair += "1";
                            break;
                        case 'w':
                            decryptedPair += "2";
                            break;
                        case 'e':
                            decryptedPair += "3";
                            break;
                        case 'r':
                            decryptedPair += "4";
                            break;
                        case 't':
                            decryptedPair += "5";
                            break;
                        case 'y':
                            decryptedPair += "6";
                            break;
                        case 'u':
                            decryptedPair += "7";
                            break;
                        case 'i':
                            decryptedPair += "8";
                            break;
                        case 'o':
                            decryptedPair += "9";
                            break;
                        case 'p':
                            decryptedPair += "0";
                            break;
                        default:
                            break;
                    }


                }
                var parsedPair = decryptedPair.Split('!', '@', '#', '$', '%', '^', '&', '*', '_', '+', '=', '`', '~', '|', ';', ':', '?', ',', '.');

                long index = 0;
                long value = 0;
                int i = 0;
                foreach (string hashedValue in parsedPair)
                {
                    if (long.TryParse(hashedValue, out long v))
                    {
                        if (i > 0)
                        {
                            value = FofASCII(v, true);
                            dehashedValues.Add(index, value);
                            break;
                        }

                        index = FofIndex(v, true);
                    }

                    i++;
                }
            }

            foreach (var pair in dehashedValues)
            {
                decryptedContent += Convert.ToChar(pair.Value);
            }

            return decryptedContent;
        }

        private static string EncodeResult(long y)
        {
            Stack<long> stack = new Stack<long>();
            while (y > 0)
            {
                stack.Push(y % 10);
                y /= 10;
            }
            string result = "";
            while (stack.Count > 0)
            {
                switch (stack.Pop())
                {
                    case 0:
                        result += 'p';
                        break;
                    case 1:
                        result += 'q';
                        break;
                    case 2:
                        result += 'w';
                        break;
                    case 3:
                        result += 'e';
                        break;
                    case 4:
                        result += 'r';
                        break;
                    case 5:
                        result += 't';
                        break;
                    case 6:
                        result += 'y';
                        break;
                    case 7:
                        result += 'u';
                        break;
                    case 8:
                        result += 'i';
                        break;
                    case 9:
                        result += 'o';
                        break;
                }
            }

            return result;
        }

        private static char GenerateSymbol()
        {
            Random random = new Random();
            switch (random.Next(20))
            {
                case 0:
                    return '!';
                case 1:
                    return '@';
                case 2:
                    return '#';
                case 3:
                    return '$';
                case 4:
                    return '%';
                case 5:
                    return '^';
                case 6:
                    return '&';
                case 7:
                    return '*';
                case 8:
                    return '_';
                case 9:
                    return '+';
                case 10:
                    return '=';
                case 11:
                    return '`';
                case 12:
                    return '~';
                case 13:
                    return '|';
                case 14:
                    return ';';
                case 15:
                    return ':';
                case 16:
                    return '?';
                case 17:
                    return ',';
                default:
                    return '.';
            }
        }

        private static long FofASCII(long a, bool inverse = false)
        {
            //f(a) = 2a^2 + 32
            if (inverse)
                return Convert.ToInt64(Math.Sqrt((a - 32) / 2));

            return (2 * (a * a)) + 32;
        }

        private static long FofIndex(long i, bool inverse = false)
        {
            //f(i) = 2(i+919) - 717
            if (inverse)
                return ((i - 717) / 2) - 919;

            return (2 * (i + 919)) - 717;
        }
    }
}
