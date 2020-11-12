using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.BackOffice.Application.Authentication
{
    public class AnonymousIdentity : BackOfficeAppIdentity
    {
        public AnonymousIdentity() : base(string.Empty, string.Empty, new string[] { },string.Empty)
        { }
    }
}
