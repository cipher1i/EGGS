using Access.Entities;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Repositories
{
    public class KeyRepository : IEGGSRepositoryKey, IDisposable
    {
        private readonly EGGSDBContext _dbContext;
        public KeyRepository(EGGSDBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public bool CreateKey(KeyModel k)
        {
            try
            {
                var key = new Key()
                {
                    Email = k.Email,
                    Key1 = k.Key
                };

                _dbContext.Key.Add(key);
                Save();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public Key ReadKey(string key)
        {
            return _dbContext.Key.Where(k => k.Key1 == key).SingleOrDefault();
        }

        public IEnumerable<Key> ReadKeys(string username)
        {
            return _dbContext.Key.Where(k => k.Email == username.ToLower());
        }

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
