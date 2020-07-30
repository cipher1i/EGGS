using System;
using Access.Entities;
using Domain.Models;

namespace Domain.Interfaces
{
    public interface IEGGSRepositoryUser : IDisposable
    {
        public bool AuthUser(string username, string password);
        public bool CreateUser(UserModel u);
        public User ReadUser(string username, string password);

        /*
        public bool UpdateUser(string username, string password, User u);
        public bool DeleteUser(string username, string password);
        */
        public void Save();
    }
}
