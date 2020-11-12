using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.IssuerManagement.Exceptions
{
    [Serializable]
    public class DuplicateIssuerException : Exception
    {
        public DuplicateIssuerException(string message)
            : base(message)
        {
        }
    }

}
