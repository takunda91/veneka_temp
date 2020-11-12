using Common.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.UserManagement.objects;

namespace Veneka.Indigo.UserManagement.dal
{
  internal  class AuthConfigurationDAL :IAuthConfigurationDAL
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UserDataManagement));
        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;


        public auth_configuration_result GetAuthConfiguration(int? authConfigurationId)
        {
            auth_configuration_result rtnValue = new auth_configuration_result();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<auth_configuration_result> results = context.usp_get_auth_configuration(authConfigurationId);

                foreach (auth_configuration_result result in results)
                {
                    rtnValue= result;
                    break;
                }
            }

            return rtnValue;
        }

        public List<auth_configuration_result> GetAuthConfigurationList(int? authConfigurationId, int? pageIndex, int? rowsPerPage)
        {
            List<auth_configuration_result> rtnValue = new List<auth_configuration_result>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<auth_configuration_result> results = context.usp_get_auth_configuration_list(pageIndex,rowsPerPage);

                foreach (auth_configuration_result result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

        public List<auth_configuration_connectionparams_result> GetAuthConfigParams(int? authConfigurationId)
        {
            List<auth_configuration_connectionparams_result> rtnValue = new List<auth_configuration_connectionparams_result>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<auth_configuration_connectionparams_result> results = context.usp_get_auth_configuration_connectionparams(authConfigurationId);

                foreach (auth_configuration_connectionparams_result result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

        public SystemResponseCode DeleteAuthConfiguration(int authConfigurationId, long auditUserId, string auditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_delete_authentication_configuration(authConfigurationId, auditUserId, auditWorkstation, ResultCode);
            }

            return (SystemResponseCode)int.Parse(ResultCode.Value.ToString());

        }

        public SystemResponseCode InsertAuthenticationConfiguration(AuthConfigResult authConfig, long auditUserId, string auditWorkstation,out int authentication_configuration_id)
        {
            authentication_configuration_id = 0;
            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    DataTable dt_auth_configparams = new DataTable();
                    dt_auth_configparams.Columns.Add("authentication_configuration_id", typeof(int));
                    dt_auth_configparams.Columns.Add("auth_type_id", typeof(int));
                    dt_auth_configparams.Columns.Add("connection_parameter_id", typeof(int));
                  
                    dt_auth_configparams.Columns.Add("interface_guid", typeof(string));
                    dt_auth_configparams.Columns.Add("external_interface_id", typeof(string));


                    DataRow workRow;

                    foreach (var item in authConfig.AuthConfigConnectionParams)
                    {
                        workRow = dt_auth_configparams.NewRow();
                        workRow["authentication_configuration_id"] = DbNullIfNull( item.authentication_configuration_id);
                        workRow["auth_type_id"] = DbNullIfNull(item.auth_type_id);
                        workRow["connection_parameter_id"] = DbNullIfNull(item.connection_parameter_id);
                        workRow["interface_guid"] = DbNullIfNull(item.interface_guid);

                        workRow["external_interface_id"] = DbNullIfNull(item.external_interface_id);

                        dt_auth_configparams.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_insert_auth_configuration]";

                    command.Parameters.Add("@authentication_configuration", SqlDbType.VarChar).Value = authConfig.AuthConfig.authentication_configuration;
                    
                    command.Parameters.AddWithValue("@auth_configuration_interface", dt_auth_configparams);
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;
                  
                    command.Parameters.Add("@authentication_configuration_id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    int resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                    return (SystemResponseCode)resultCode;
                }
            }
        }

        public SystemResponseCode UpdateAuthenticationConfiguration(AuthConfigResult authConfig, long auditUserId, string auditWorkstation)
        {
            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    DataTable dt_auth_configparams = new DataTable();
                    dt_auth_configparams.Columns.Add("authentication_configuration_id", typeof(int));
                    dt_auth_configparams.Columns.Add("auth_type_id", typeof(int));
                    dt_auth_configparams.Columns.Add("connection_parameter_id", typeof(int));
                  
                    dt_auth_configparams.Columns.Add("interface_guid", typeof(string));
                    dt_auth_configparams.Columns.Add("external_interface_id", typeof(string));

                    DataRow workRow;

                    foreach (var item in authConfig.AuthConfigConnectionParams)
                    {
                        workRow = dt_auth_configparams.NewRow();
                        workRow["authentication_configuration_id"] = item.authentication_configuration_id;
                        workRow["auth_type_id"] = DbNullIfNull(item.auth_type_id);
                        workRow["connection_parameter_id"] = DbNullIfNull( item.connection_parameter_id);
                        workRow["interface_guid"] = DbNullIfNull( item.interface_guid);

                        workRow["external_interface_id"] = DbNullIfNull( item.external_interface_id);

                        dt_auth_configparams.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_auth_configuration]";
                    command.Parameters.Add("@authentication_configuration_id", SqlDbType.Int).Value = authConfig.AuthConfig.authentication_configuration_id;

                    command.Parameters.Add("@authentication_configuration", SqlDbType.VarChar).Value = authConfig.AuthConfig.authentication_configuration;

                    command.Parameters.AddWithValue("@auth_configuration_interface", dt_auth_configparams);
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;

                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    int resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                    return (SystemResponseCode)resultCode;
                }
            }
        }
        public object DbNullIfNull( object obj)
        {
            return obj != null ? obj : DBNull.Value;
        }

    }
}
