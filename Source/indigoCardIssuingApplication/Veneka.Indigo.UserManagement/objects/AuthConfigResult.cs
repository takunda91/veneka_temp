using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Models;

namespace Veneka.Indigo.UserManagement.objects
{
    public sealed class AuthConfigResult
    {
            public auth_configuration_result AuthConfig { get; set; }
            public List<auth_configuration_connectionparams_result> AuthConfigConnectionParams { get; set; }

        
    }
}
