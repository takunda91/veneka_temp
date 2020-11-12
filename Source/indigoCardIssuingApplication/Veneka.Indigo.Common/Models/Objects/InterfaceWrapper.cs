using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Models;

namespace Veneka.Indigo.Common.Objects
{
    public class InterfaceWrapper
    {
        public int ID { get; set; }
        public issuer_interface IssuerInterface { get; set; }
        public interface_type InterfaceType { get; set; }
        //public connection_parameters ConParams { get; set; }
    }
}
