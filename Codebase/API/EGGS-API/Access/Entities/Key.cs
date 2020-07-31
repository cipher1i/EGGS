using System;
using System.Collections.Generic;

namespace Access.Entities
{
    public partial class Key
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Key1 { get; set; }

        public virtual User EmailNavigation { get; set; }
    }
}
