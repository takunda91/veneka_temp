using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Models;

namespace Veneka.Indigo.RemoteManagement.DAL
{
    public class RemoteLoggingDAL : IRemoteLoggingDAL
    {
        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;

        public void LogRequest(string token, string clientIP, int methodCalId, string request)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_remote_log_request(clientIP, token, request, methodCalId);                
            }
        }
    }
}
