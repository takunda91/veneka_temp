namespace Veneka.Indigo.Common.Database
{
    public class DBConnectInfo
    {
        private string _databaseServer = "Localhost";


        public string DatabaseServer
        {
            get { return _databaseServer; }
            set { _databaseServer = value; }
        }

        public string AuthenticationType { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}