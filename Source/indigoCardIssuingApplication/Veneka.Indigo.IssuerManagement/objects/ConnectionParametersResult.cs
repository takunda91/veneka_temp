using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Models;

namespace Veneka.Indigo.IssuerManagement.objects
{
     public class ConnectionParametersResult
    {
        public ConnectionParamsResult ConnectionParams { get; set; }
        public List<ConnectionParamAdditionalDataResult> additionaldata { get; set; }

    }
}
