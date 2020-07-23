using System;
using System.Collections.Generic;

namespace Access.Entities
{
    public partial class Ip
    {
        public string Email { get; set; }
        public string Ipv4 { get; set; }
        public string Ipv6 { get; set; }

        public virtual User EmailNavigation { get; set; }
    }
}
