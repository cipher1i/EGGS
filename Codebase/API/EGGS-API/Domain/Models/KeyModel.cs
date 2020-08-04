using Access.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Models
{
    public class KeyModel : IDisposable
    {
        private readonly IEGGSRepositoryKey _repoReaper;
        public KeyModel(IEGGSRepositoryKey repoReaper)
        {
            _repoReaper = repoReaper;
        }

        public long Id { get; set; }
        public string Email { get; set; }
        public string Key { get; set; }

        public bool Create()
        {
            if (string.IsNullOrEmpty(this.Email) || string.IsNullOrEmpty(this.Key))
                return false;

            var key = new KeyModel(_repoReaper)
            {
                Email = this.Email,
                Key = this.Key
            };

            return _repoReaper.CreateKey(key);
        }

        public bool Read(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            var k = _repoReaper.ReadKey(key);
            if (k == null)
                return false;

            this.Id = k.Id;
            this.Email = k.Email;
            this.Key = k.Key1;
            return true;
        }

        public IEnumerable<KeyModel> ReadAll(string username)
        {
            if (string.IsNullOrEmpty(username))
                return null;

            var keys = _repoReaper.ReadKeys(username);
            if (keys == null)
                return null;

            List<KeyModel> list = new List<KeyModel>();
            foreach (var key in keys)
            {
                list.Add(new KeyModel(_repoReaper)
                {
                    Id = key.Id,
                    Email = key.Email,
                    Key = key.Key1
                });
            }
            
            return list;
        }

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _repoReaper.Dispose();
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
