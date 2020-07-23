using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Helpers
{
    public class AuthHelper
    {
        private string _username;
        private string _password;
        public string Username { get { return _username; } set { _username = value ?? ""; } }
        public string Password { get { return _password; } set { _password = value ?? ""; } }
        public AuthHelper(string username, string password)
        {
            Username = username;
            Password = password;
            ChangeLock();
        }
        private string Encrypt(string str)
        {
            string result = "";
            foreach (char c in str)
            {
                Stack<long> stack = new Stack<long>();
                long num = Convert.ToInt64(c) * 19;
                while (num > 0)
                {
                    stack.Push(num % 10);

                    num /= 10;
                }

                while (stack.Count > 0)
                {
                    long digit = stack.Pop();
                    if (digit > 0)
                    {
                        for (int i = 0; i < digit; i++)
                        {
                            result += "*";
                        }
                        result += " ";
                    }
                    else
                        result += "o ";
                }

                result += str[^1] == c ? "" : ".";
            }

            return result + "#";
        }
        private string Decrypt(string str)
        {
            string decrypted = "";
            foreach (string tuple in str.Split('.'))
            {
                int count = 0;
                Queue<int> q = new Queue<int>();
                foreach (char c in tuple)
                {
                    switch (c)
                    {
                        case ' ':
                            q.Enqueue(count);
                            count = 0;
                            break;
                        case '*':
                            count++;
                            break;
                        case 'o':
                            count = 0;
                            break;
                    }
                }
                string number = "";
                while (q.Count > 0)
                {
                    number += q.Dequeue();
                }

                long.TryParse(number, out long n);
                decrypted += Convert.ToChar(n/19) != '#' ? Convert.ToChar(n/19).ToString() : "";
            }

            return decrypted;
        }
        public bool ChangeLock()
        {
            if (Username.Length > 0 && Password.Length > 0)
            {
                //unlocked
                if (!Username.EndsWith('#') && !Password.EndsWith('#'))
                {
                    Username = Encrypt(Username);
                    Password = Encrypt(Password);
                    return true;
                }

                //locked
                Username = Decrypt(Username);
                Password = Decrypt(Password);
            }

            return false;
        }
        ~AuthHelper() { }
    }
}
