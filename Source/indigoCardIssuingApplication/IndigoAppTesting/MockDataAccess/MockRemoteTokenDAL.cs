using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.RemoteManagement.DAL;

namespace IndigoAppTesting.MockDataAccess
{
    public class MockRemoteTokenDAL : IRemoteTokenDAL
    {
        public static Guid RemoteTokenValid { get { return new Guid("BA7A2F42-8476-464B-8F50-897776CE70B0"); } }
        public static Guid RemoteTokenExpired { get { return new Guid("95978F9B-3853-4512-9D40-675987BF34E0"); } }
        public static Guid RemoteTokenInactiveIssuer { get { return new Guid("670979EB-E8D3-4B34-964E-8ACB037CAA94"); } }

        public RemoteTokenIssuerResult GetRemoteToken(Guid remoteToken, long auditUserId, string auditWorkstation)
        {
            if(remoteToken.Equals(RemoteTokenValid))
            {
                return new RemoteTokenIssuerResult
                {
                    issuer_code = "001",
                    issuer_id = 0,
                    issuer_name = "FirstBank",
                    issuer_status_id = 0,
                    remote_token_expiry = DateTime.Now.AddMonths(1)
                };
            }
            else if(remoteToken.Equals(RemoteTokenExpired))
            {
                return new RemoteTokenIssuerResult
                {
                    issuer_code = "001",
                    issuer_id = 0,
                    issuer_name = "FirstBank",
                    issuer_status_id = 0,
                    remote_token_expiry = DateTime.Now.AddDays(-1)
                };
            }
            else if (remoteToken.Equals(RemoteTokenInactiveIssuer))
            {
                return new RemoteTokenIssuerResult
                {
                    issuer_code = "001",
                    issuer_id = 0,
                    issuer_name = "FirstBank",
                    issuer_status_id = 1,
                    remote_token_expiry = DateTime.Now.AddMonths(1)
                };
            }

            return null;
        }
    }
}
