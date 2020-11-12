
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.BackOffice.Application.Objects;

namespace Veneka.Indigo.BackOffice.Application.DAL
{
    public class BackOfficeDB : ILocalStorage
    {
        private readonly string _filePath;
        private readonly string _fileName;
        private static readonly ILog log = LogManager.GetLogger(typeof(BackOfficeDB));

        public BackOfficeDB(string filePath, string fileName)
        {
            _filePath = filePath;
            _fileName = fileName;
            Init();
        }
        //private DataTable Tasks() => ExecuteWithConnection(Tasks, SelectTaskQuery);
        private void ExecuteWithConnection(Action<SQLiteConnection> action)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                action(connection);
            }
        }
        private SQLiteConnection CreateDatabase(string _filePath, string filename)
        {
            string dbpath = _filePath + "\\" + filename;
            if (!System.IO.File.Exists(_filePath))
            {
                if (!Directory.Exists(_filePath))
                {
                    Directory.CreateDirectory(_filePath);
                }
                if(!File.Exists(dbpath))
                {
                    log.Debug("Just entered to create Sync DB");
                    SQLiteConnection.CreateFile(dbpath);
                }
                 
            }
            return new SQLiteConnection("Data Source=" + dbpath + ";Version=3;"); 
        }
        private static DataTable Tasks(SQLiteConnection connection, string SelectTaskQuery)
        {
            var adapter = new SQLiteDataAdapter(SelectTaskQuery, connection);

            var datatable = new DataTable();

            adapter.Fill(datatable);

            return datatable;
        }



        public SQLiteConnection GetConnection() => CreateDatabase(_filePath, _fileName);

        public void Init() => ExecuteWithConnection(connection =>
        {

            using (var command = connection.CreateCommand())
                CreateSchemaIfNotExists(command);
        });
        public void Get(DataTable dataTable, string SelectTaskQuery, List<Parameters> _lst)
        {
            ExecuteWithConnection(connection =>
            {

                SQLiteCommand cmd = new SQLiteCommand(SelectTaskQuery, connection);

                {
                    cmd.CommandType = CommandType.Text;

                    foreach (Parameters p in _lst.Where(p => p.Direction == ParameterDirection.Input))
                    {
                        cmd.Parameters.Add(new SQLiteParameter(p.ParameterName, p.Value));
                    }

                    var adapter = new SQLiteDataAdapter(cmd);

                    var builder = new SQLiteCommandBuilder(adapter);

                    adapter.Fill(dataTable);

                }
            });
        }
        public void UpdateChanges(string SelectTaskQuery, List<Parameters> _lst)
        {
            ExecuteWithConnection(connection =>
            {
                SQLiteCommand cmd = new SQLiteCommand(SelectTaskQuery, connection);

                cmd.CommandType = CommandType.Text;
                foreach (Parameters p in _lst.Where(p => p.Direction == ParameterDirection.Input))
                {
                    cmd.Parameters.Add(p.ParameterName, p.DataType).Value = p.Value;
                }
                foreach (Parameters p in _lst.Where(p => p.Direction == ParameterDirection.Output))
                {
                    cmd.Parameters.Add(p.ParameterName, p.DataType).Direction = p.Direction;
                }
                cmd.ExecuteNonQuery();
                foreach (Parameters p in _lst.Where(p => p.Direction == ParameterDirection.Output))
                {
                    if (cmd.Parameters[p.ParameterName].Value != null)
                        p.Value = cmd.Parameters[p.ParameterName].Value.ToString();
                }
            });
        }

        private T ExecuteWithConnection<T>(Func<SQLiteConnection, T> action)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                return action(connection);
            }
        }
        private static void CreateSchemaIfNotExists(IDbCommand command)
        {
            const string query = "CREATE TABLE IF NOT EXISTS printbatch_requests (Id INTEGER PRIMARY KEY AUTOINCREMENT,request_id BIGINT  NOT NULL,print_batch_id BIGINT  NOT NULL, pan TEXT, printing_status TEXT,request_reference TEXT,request_statusId INTEGER)";

            command.CommandText = query;

            command.ExecuteNonQuery();
        }




    }

}
