using Domain.Interfaces;
using System;
using Access.Entities;
using Domain.Models;
using System.Linq;

namespace Domain.Repositories
{
    public class UserRepository : IEGGSRepositoryUser, IDisposable
    {
        private readonly EGGSDBContext _dbContext;
        public UserRepository(EGGSDBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public bool AuthUser(string username, string password)
        {
            return _dbContext.User.Where(u => u.Email == username.ToLower() && u.Password == password).SingleOrDefault() != null;
        }

        public bool CreateUser(UserModel u)
        {
            var user = new User
            {
                Email = u.Email,
                Password = u.Password,
                FirstName = u.FirstName,
                LastName = u.LastName
            };

            _dbContext.User.Add(user);
            Save();
            return true;
        }

        public User ReadUser(string username, string password)
        {
            return _dbContext.User.Where(u => u.Email == username.ToLower() && u.Password == password).SingleOrDefault();
        }

        /*
        public bool UpdateUser(string username, string password, User user)
        {
            User temp = _dbContext.User.Where(u => u.Email == username.ToLower() && u.Password == password).SingleOrDefault();
            temp = user;
            Save();
            return true;
        }

        public bool DeleteUser(string username, string password)
        {
            if (username.Length < 1 || password.Length < 1)
                return false;

            User user = new User
            {
                Email = username.ToLower()
            };

            _dbContext.Remove(user);
            Save();
            return true;
        }
        */

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }

                _disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
