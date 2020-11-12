using Common.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.BackOffice.Application.DAL;
using Veneka.Indigo.BackOffice.Application.Objects;


namespace Veneka.Indigo.BackOffice.Application.Database
{
   public class BackOfficeDAL
    {
      
        ILocalStorage _connection;
        private static BackOfficeDAL _instance;
        private static readonly ILog log = LogManager.GetLogger(typeof(BackOfficeDB));

        public BackOfficeDAL()
        {
            _connection = new BackOfficeDB(@"C:\veneka\sqlite", "BackOffice.db");
        }

        public static BackOfficeDAL Instance
        {
            get
            {
                if (_instance == null)
                {
                    
                        if (_instance == null)
                        {
                            _instance = new BackOfficeDAL();
                        }
                    
                }
                return _instance;
            }
        }

        public void InsertRequests(long request_id, long print_batch_id, string printing_status,string request_reference,int? request_statusId)
        {

            try
            {


                string txtSQLQuery = "insert into  printbatch_requests (request_id,print_batch_id,printing_status,request_reference,request_statusId) values (@request_id,@print_batch_id,@printing_status,@request_reference,@request_statusId)";
                List<Parameters> _paramlist = new List<Parameters>();
                _paramlist.Add(new Parameters("@request_id", DbType.Int32, request_id.ToString(), ParameterDirection.Input));
                _paramlist.Add(new Parameters("@print_batch_id", DbType.Int32, print_batch_id.ToString(), ParameterDirection.Input));
                _paramlist.Add(new Parameters("@printing_status", DbType.String, printing_status, ParameterDirection.Input));
                _paramlist.Add(new Parameters("@request_reference", DbType.String, request_reference, ParameterDirection.Input));
                _paramlist.Add(new Parameters("@request_statusId", DbType.Int32, request_statusId.ToString(), ParameterDirection.Input));

                //_connection.ExecuteQueries(txtSQLQuery, _paramlist);
                _connection.UpdateChanges(txtSQLQuery, _paramlist);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                //_connection.Close();
            }
        }
        public void UpdateRequest(long request_id, long print_batch_id, string printing_status,string new_printing_status, string pan)
        {

            try
            {
                string txtSQLQuery = "update printbatch_requests set pan=@pan, printing_status=@new_printing_status where request_id=@request_id and print_batch_id=@print_batch_id ";
                //"and printing_status=@printing_status";
                List<Parameters> _paramlist = new List<Parameters>();
             
                _paramlist.Add(new Parameters("@request_id", DbType.Int32, request_id.ToString(), ParameterDirection.Input));
                _paramlist.Add(new Parameters("@pan", DbType.String, pan.ToString(), ParameterDirection.Input));
                _paramlist.Add(new Parameters("@print_batch_id", DbType.Int32, print_batch_id.ToString(), ParameterDirection.Input));
                _paramlist.Add(new Parameters("@new_printing_status", DbType.String, new_printing_status, ParameterDirection.Input));

                _paramlist.Add(new Parameters("@printing_status", DbType.String, printing_status, ParameterDirection.Input));
                _connection.UpdateChanges(txtSQLQuery, _paramlist);

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                //_connection.Close();
            }

        }
        public bool IsRequestInProgress(long request_id, long print_batch_id, string printing_status)
        {
            bool flag = false;
            try
            {
                string txtSQLQuery = "select count(*) as 'count' from printbatch_requests  where request_id=@request_id and print_batch_id=@print_batch_id and pan is null";
                List<Parameters> _paramlist = new List<Parameters>();
                _paramlist.Add(new Parameters("@request_id", DbType.Int32, request_id.ToString(), ParameterDirection.Input));
                _paramlist.Add(new Parameters("@print_batch_id", DbType.Int32, print_batch_id.ToString(), ParameterDirection.Input));
                DataTable dt = new DataTable();
                _connection.Get(dt,txtSQLQuery, _paramlist);
                if(dt!=null )
                {
                    if (int.Parse(dt.Rows[0]["count"].ToString()) > 0)
                    {
                        flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
              
            }
            return flag;
        }


        public DataTable GetRequests(long? print_batch_id)
        {
            bool flag = false;
            DataTable dt = new DataTable();
            try
            {
                string txtSQLQuery = "select * from printbatch_requests  where  print_batch_id=@print_batch_id ";
                List<Parameters> _paramlist = new List<Parameters>();
                _paramlist.Add(new Parameters("@print_batch_id", DbType.Int32, print_batch_id.ToString(), ParameterDirection.Input));
                
                _connection.Get(dt, txtSQLQuery, _paramlist);
                
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {

            }
            return dt;
        }
    }
}
