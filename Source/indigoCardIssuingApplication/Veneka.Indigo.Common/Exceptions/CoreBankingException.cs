using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Common.Exceptions
{
    public class CoreBankingException : BaseIndigoException
    {
        /// <summary>
        /// Throw Exceptions in CoreBanking Module.
        /// </summary>
        /// <param name="exMessage"></param>
        /// <param name="systemResponseCode"></param>
        /// <param name="innerException"></param>
        public CoreBankingException(string exMessage, SystemResponseCode systemResponseCode, Exception innerException)
            : base(exMessage, systemResponseCode, innerException)
        {            
        }        
    }
}
