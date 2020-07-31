using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Metadata.Ecma335;

namespace Domain.Models
{
    public class UserModel
    {
        private readonly IEGGSRepositoryUser _repoReaper;
        public UserModel(IEGGSRepositoryUser repoReaper)
        {
            _repoReaper = repoReaper;
        }

        public long Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool Auth(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            return _repoReaper.AuthUser(username, password);
        }

        public bool Create()
        {
            if (string.IsNullOrEmpty(this.Email) || string.IsNullOrEmpty(this.Password))
                return false;

            var user = new UserModel(_repoReaper)
            {
                Email = this.Email,
                Password = this.Password,
                FirstName = this.FirstName,
                LastName = this.LastName
            };

            return _repoReaper.CreateUser(user);
        }

        public bool Read(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            var user = _repoReaper.ReadUser(username, password);
            if (user == null)
                return false;

            Id = user.Id;
            Email = user.Email;
            Password = user.Password;
            FirstName = user.FirstName;
            LastName = user.LastName;
            return true;
        }

        /*
        public bool Update(string username, string password)
        {
            var user = new User
            {
                Email = Email,
                Password = Password,
                FirstName = FirstName,
                LastName = LastName
            };

            return _repoReaper.UpdateUser(username, password, user);
        }

        public bool Delete(string username, string password)
        {
            return _repoReaper.DeleteUser(username, password);
        }
        */
    }
}
