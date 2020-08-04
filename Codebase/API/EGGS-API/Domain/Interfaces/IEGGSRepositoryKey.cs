using Access.Entities;
using Domain.Models;
using System;
using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IEGGSRepositoryKey : IDisposable
    {
        public bool CreateKey(KeyModel k);
        public Key ReadKey(string key);
        public IEnumerable<Key> ReadKeys(string username);

        /*
        public bool UpdateKey();
        public bool DeleteKey();
        */
        public void Save();
    }
}
