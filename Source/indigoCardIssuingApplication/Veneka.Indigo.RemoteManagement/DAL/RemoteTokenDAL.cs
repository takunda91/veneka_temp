using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Models;

namespace Veneka.Indigo.RemoteManagement.DAL
{
    public class RemoteTokenDAL : IRemoteTokenDAL
    {
        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;

        public RemoteTokenIssuerResult GetRemoteToken(Guid remoteToken, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_remote_get_token(remoteToken, auditUserId, auditWorkstation);
                return result.FirstOrDefault();
            }
        }
    }
}
