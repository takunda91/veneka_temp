using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Common.Exceptions
{
    public class AccountValidationException : BaseIndigoException
    {
        /// <summary>
        /// If the login was unsuccessful, throw back message as to why.
        /// </summary>
        /// <param name="exMessage"></param>
        public AccountValidationException(string exMessage, SystemResponseCode systemResponseCode, Exception innerException)
            : base(exMessage, systemResponseCode, innerException)
        {            
        }        
    }
}
