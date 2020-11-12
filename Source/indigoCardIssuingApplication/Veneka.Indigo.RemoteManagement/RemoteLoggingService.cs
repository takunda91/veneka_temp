using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.RemoteManagement.DAL;

namespace Veneka.Indigo.RemoteManagement
{
    public class RemoteLoggingService
    {
        private readonly IRemoteLoggingDAL _remoteLoggingDAL;

        public RemoteLoggingService() : this(new RemoteLoggingDAL())
        { }

        public RemoteLoggingService(IRemoteLoggingDAL remoteLoggingDAL)
        {
            _remoteLoggingDAL = remoteLoggingDAL;
        }

        public void LogRequest(string token, string clientIP, int methodCalId, string request)
        {
            _remoteLoggingDAL.LogRequest(token, clientIP, methodCalId, request);
        }
    }
}
