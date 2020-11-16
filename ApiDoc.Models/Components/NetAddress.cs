using System;
using System.Collections.Generic;
using System.Text;

namespace ApiDoc.Models.Components
{
    public class NetAddress
    {
        public string Address { get; set; }
        public int Port { get; set; } 
        public List<string> ServiceNames { get; set; }
    }
}
