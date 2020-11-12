using System;
using Veneka.Indigo.Common.Models;

namespace Veneka.Indigo.RemoteManagement.DAL
{
    public interface IRemoteTokenDAL
    {
        RemoteTokenIssuerResult GetRemoteToken(Guid remoteToken, long auditUserId, string auditWorkstation);
    }
}