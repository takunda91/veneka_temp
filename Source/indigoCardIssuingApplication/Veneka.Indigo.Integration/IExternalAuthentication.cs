using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Integration
{
  public interface IExternalAuthentication:ICommon
    {
        bool Login(Config.IConfig config, long? user_id,string branchcode ,string username, string password, int languageId, long auditUserId, string auditWorkStation, out string responseMessage);

        UserObject GetUserDetails(Config.IConfig config, long? user_id, string branchcode, string username, int languageId, long auditUserId, string auditWorkStation, out string responseMessage);                

    }
}
