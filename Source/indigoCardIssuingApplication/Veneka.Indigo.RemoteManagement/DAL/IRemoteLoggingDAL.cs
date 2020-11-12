namespace Veneka.Indigo.RemoteManagement.DAL
{
    public interface IRemoteLoggingDAL
    {
        void LogRequest(string token, string clientIP, int methodCalId, string request);
    }
}