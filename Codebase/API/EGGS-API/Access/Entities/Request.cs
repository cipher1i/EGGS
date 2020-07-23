using System;
using System.Collections.Generic;

namespace Access.Entities
{
    public partial class Request
    {
        public string Email { get; set; }
        public long Requests { get; set; }
        public bool? Status { get; set; }

        public virtual User EmailNavigation { get; set; }
    }
}
