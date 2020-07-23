using System;
using System.Collections.Generic;

namespace Access.Entities
{
    public partial class User
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual Ip Ip { get; set; }
        public virtual Request Request { get; set; }
    }
}
