using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.UserManagement.objects;

namespace Veneka.Indigo.UserManagement.dal
{
    public interface IAuthConfigurationDAL
    {
        auth_configuration_result GetAuthConfiguration(int? authConfigurationId);
        List<auth_configuration_result> GetAuthConfigurationList(int? authConfigurationId, int? pageIndex, int? rowsPerPage);
        List<auth_configuration_connectionparams_result> GetAuthConfigParams(int? authConfigurationId);

        SystemResponseCode DeleteAuthConfiguration(int authConfigurationId, long auditUserId, string auditWorkstation);

        SystemResponseCode InsertAuthenticationConfiguration(AuthConfigResult authConfig, long auditUserId, string auditWorkstation, out int authentication_configuration_id);

        SystemResponseCode UpdateAuthenticationConfiguration(AuthConfigResult authConfig, long auditUserId, string auditWorkstation);
    }
}
