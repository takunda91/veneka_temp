using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Common.Exceptions
{
    [Serializable]
    public class BaseIndigoException : Exception
    {
        /// <summary>
        /// If the login was unsuccessful, throw back message as to why.
        /// </summary>
        /// <param name="exMessage"></param>
        public BaseIndigoException(string exMessage, SystemResponseCode systemResponseCode, Exception innerException)
            : base(exMessage, innerException)
        {
            this.SystemCode = systemResponseCode;
        }

        public SystemResponseCode SystemCode { get; private set; }
    }
}
