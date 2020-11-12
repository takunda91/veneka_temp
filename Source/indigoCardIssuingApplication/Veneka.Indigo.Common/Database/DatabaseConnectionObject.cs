using System;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
using Veneka.Indigo.Common;
using Common.Logging;

namespace Veneka.Indigo.Common.Database
{
    public sealed class DatabaseConnectionObject
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DatabaseConnectionObject));

        private static DatabaseConnectionObject _instance;
        private static int _numberOfConnections;
        private static readonly object _lockOject = new object();
        private static string ConnetionString;
        private static string ReportsConnetionString;
        private static string IssuerManConnectionString;
        private static string _SQLConnectionString;

        private DatabaseConnectionObject()
        {
        }

        public static DatabaseConnectionObject Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockOject)
                    {
                        if (_instance == null)
                        {
                            _instance = new DatabaseConnectionObject();
                        }
                    }
                }
                return _instance;
            }
        }


        public SqlConnection SQLConnection
        {
            get
            {
                var connection = new SqlConnection(SQLConnectionString);

                _numberOfConnections++;
                connection.Open();

                return connection;
            }
        }

        /// <summary>
        /// Returns SQL Server Connection String.
        /// </summary>
        public string SQLConnectionString
        {
            get
            {

                #region old code
                //                return @"Data Source=10.2.108.41,2013;Initial Catalog=indigo_database;
//                        Integrated Security=SSPI;User ID=ECOBANKGROUP\indigoicpp;
//                        Password=56Gy6]7x5m6!";

                //for local dev server
               // return "Data Source=localhost;Initial Catalog=indigo_database;Integrated Security=true";

                //for VENEKA TEST
               // return @"Data Source=192.168.10.111;Initial Catalog=indigo_database;User Id=mpho.majenge@veneka.com;Password=Password=dub68ACE!;Integrated Security=false";

                //for live file loader application
               // return @"Data Source=10.2.108.41,2013;Initial Catalog=indigo_database;User Id=ECOBANKGROUP\indigoicpp;Password=Password=56Gy6]7x5m6!;Integrated Security=false";

                ////for live application server
                //return @"Data Source=10.2.108.41,2013;Initial Catalog=indigo_database;Integrated Security=True";
                #endregion

                try
                {
                    if (String.IsNullOrWhiteSpace(_SQLConnectionString))
                    {
                        string connectionString = "";
                        //@"C:\veneka\indigo_group\config\Database.config"

                        using (var reader = TextReader.Synchronized(new StreamReader(Path.Combine(SystemConfiguration.Instance.GetBaseConfigDir(), @"config\Database.config"))))
                        {
                            connectionString = reader.ReadLine();
                        }

                        _SQLConnectionString = connectionString;
                    }

                    return _SQLConnectionString;                   
                }
                catch (IOException ex)
                {
                    log.Fatal(ex);
                    throw;
                    //throw new Exception("File Database.config with a connection string not fouund. Please insert the file with correct values");
                }                
            }            
        }
        
        /// <summary>
        /// Returns Entity Framework Connection String.
        /// </summary>
        public string EFReportsSQLConnectionString 
        {
            get
            {
                if (String.IsNullOrEmpty(ReportsConnetionString))
                {
                    try
                    {
                        //@"C:\veneka\indigo_group\config\EF.config"
                        string aaa = SystemConfiguration.Instance.GetBaseConfigDir();
                        string abc = Path.Combine(SystemConfiguration.Instance.GetBaseConfigDir(), "config\\EF.config");
                        using (var reader = TextReader.Synchronized(new StreamReader(Path.Combine(SystemConfiguration.Instance.GetBaseConfigDir(), "config\\EF.config"))))
                        {
                            string line = reader.ReadLine().Replace(".Indigo.",".Reports.");
                            ReportsConnetionString = line.Replace("::CONNSTRING::", SQLConnectionString);
                        }
                    }
                    catch (IOException ex)
                    {
                        log.Fatal(ex);
                        throw;
                        //throw new Exception("File EF.config with a connection string not fouund. Please insert the file with correct values");
                    }                   
                }

                return ReportsConnetionString;
            }
        }

        public string EFIssuerManSQLConnectionString
        {
            get
            {
                if (String.IsNullOrEmpty(IssuerManConnectionString))
                {
                    try
                    {
                        string ef = "metadata=res://*/Models.IssuerManagement.csdl|res://*/Models.IssuerManagement.ssdl|res://*/Models.IssuerManagement.msl;provider=System.Data.SqlClient;provider connection string='::CONNSTRING:: MultipleActiveResultSets=True;App=EntityFramework'";

                        IssuerManConnectionString = ef.Replace("::CONNSTRING::", SQLConnectionString);                        
                    }
                    catch (IOException ex)
                    {
                        log.Fatal(ex);
                        throw;
                        //throw new Exception("File EF.config with a connection string not fouund. Please insert the file with correct values");
                    }
                }

                return IssuerManConnectionString;
            }
        }

         /// <summary>
        /// Returns Entity Framework Connection String.
        /// </summary>
        public string EFSQLConnectionString
        {
            get
            {
                if (String.IsNullOrEmpty(ConnetionString))
                {
                    try
                    {
                        //@"C:\veneka\indigo_group\config\EF.config"
                        string aaa = SystemConfiguration.Instance.GetBaseConfigDir();
                        string abc = Path.Combine(SystemConfiguration.Instance.GetBaseConfigDir(), "config\\EF.config");
                        using (var reader = TextReader.Synchronized(new StreamReader(Path.Combine(SystemConfiguration.Instance.GetBaseConfigDir(), "config\\EF.config"))))
                        {
                            string line = reader.ReadLine();
                            ConnetionString = line.Replace("::CONNSTRING::", SQLConnectionString);
                        }
                    }
                    catch (IOException ex)
                    {
                        log.Fatal(ex);
                        throw;
                        //throw new Exception("File EF.config with a connection string not fouund. Please insert the file with correct values");
                    }                   
                }

                return ConnetionString;
            }
        }

        public void DestroyConnection(SqlConnection conn, SqlCommand commandObject)
        {
            if (conn != null)
            {
                try
                {
                    conn.Close();
                    conn = null;
                    _numberOfConnections--;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    //IndigoCardIssuanceCommon.utilities.
                }
            }


            if (commandObject != null)
            {
                try
                {
                    commandObject = null;
                }
                catch (Exception ex)
                {
                    log.Fatal(ex);
                }
            }
        }
    }
}