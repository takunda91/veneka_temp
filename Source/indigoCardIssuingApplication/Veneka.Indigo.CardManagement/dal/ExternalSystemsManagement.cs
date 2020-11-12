using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Veneka.Indigo.CardManagement.objects;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Models;

namespace Veneka.Indigo.CardManagement.dal
{
   internal class ExternalSystemsManagement
    {
        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;

        #region "EXTERNAL SYSTEMS"

        /// <summary>
        /// CREATEING EXTERNAL SYSTEMS IN THE DATABASE
        /// </summary>
        /// <param name="ldapSetting"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal SystemResponseCode CreateExternalSystems(ExternalSystemFieldResult externalsystems, long auditUserId, string auditWorkstation, out int? externalsystemid)
        {
            int resultCode;

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {


                    DataTable dt_externalsystemfields = new DataTable();
                    dt_externalsystemfields.Columns.Add("external_system_field_id", typeof(int));
                    dt_externalsystemfields.Columns.Add("field_name", typeof(string));
                    dt_externalsystemfields.Columns.Add("field_value", typeof(string));
                    DataRow Row;

                    foreach (var item in externalsystems.ExternalSystemFields)
                    {
                        Row = dt_externalsystemfields.NewRow();
                        Row["external_system_field_id"] = item.external_system_field_id;
                        Row["field_name"] = item.field_name;
                        Row["field_value"] = null;
                        dt_externalsystemfields.Rows.Add(Row);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_create_external_systems]";

                    command.Parameters.Add("@external_system_type_id", SqlDbType.Int).Value = externalsystems.ExternalSystem.external_system_type_id;
                    command.Parameters.Add("@system_name", SqlDbType.VarChar).Value = externalsystems.ExternalSystem.system_name;
                    command.Parameters.AddWithValue("@external_system_fields", dt_externalsystemfields);

                    command.Parameters.Add("@audit_user_id", SqlDbType.Int).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;


                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    command.Parameters.Add("@external_system_id", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    externalsystemid = int.Parse(command.Parameters["@external_system_id"].Value.ToString());
                    resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());
                }
                return (SystemResponseCode)resultCode;
            }
        }

        internal ExternalSystemFieldResult GetExternalSystems(int? externalsytemid,int rowindex,int rowsperpage,int languageId,long auditUserId, string auditWorkstation)
        {
            ExternalSystemFieldResult rtnValue = new ExternalSystemFieldResult();
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_external_systems(externalsytemid, languageId,rowindex, rowsperpage, auditUserId, auditWorkstation);

                rtnValue.ExternalSystems = results.ToList();
                if (externalsytemid != null) 
                rtnValue.ExternalSystemFields = GetExternalSystemsFields(null,1,2000,auditUserId,auditWorkstation).Where(i=>i.external_system_id==externalsytemid).ToList();

            }

            return rtnValue;
        }


        internal SystemResponseCode UpdateExternalSystem(ExternalSystemFieldResult externalsystem, long auditUserId, string auditWorkstation)
        {
            int resultCode;

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {


                    DataTable dt_externalsystemfields = new DataTable();
                    dt_externalsystemfields.Columns.Add("external_system_field_id", typeof(int));
                    dt_externalsystemfields.Columns.Add("field_name", typeof(string));
                    dt_externalsystemfields.Columns.Add("field_value", typeof(string));
                    DataRow Row;

                    foreach (var item in externalsystem.ExternalSystemFields)
                    {
                        Row = dt_externalsystemfields.NewRow();
                        Row["external_system_field_id"] = item.external_system_field_id;
                        Row["field_name"] = item.field_name;
                        Row["field_value"] = null;
                        dt_externalsystemfields.Rows.Add(Row);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_external_systems]";

                    command.Parameters.Add("@external_system_id", SqlDbType.Int).Value = externalsystem.ExternalSystem.external_system_id;
                    command.Parameters.Add("@external_system_type_id", SqlDbType.Int).Value = externalsystem.ExternalSystem.external_system_type_id;
                    command.Parameters.Add("@system_name", SqlDbType.VarChar).Value = externalsystem.ExternalSystem.system_name;
                    command.Parameters.AddWithValue("@external_system_fields", dt_externalsystemfields);

                    command.Parameters.Add("@audit_user_id", SqlDbType.Int).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;


                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());
                }
                return (SystemResponseCode)resultCode;
            }
        }

        internal SystemResponseCode DeleteExternalSystems(int? externalsystemid, long auditUserId, string auditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_delete_external_systems(externalsystemid,
                                       auditUserId, auditWorkstation, ResultCode);
            }
            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }
        #endregion

        #region "EXTERNAL SYSTEM FIELDS"

        internal SystemResponseCode CreateExternalSystemFields(ExternalSystemFieldsResult externalsystems, long auditUserId, string auditWorkstation, out int? externalsystemfieldid)
        {
            ObjectParameter ExternalSystemFieldId = new ObjectParameter("external_system_field_id", typeof(int));
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_create_external_system_fields(externalsystems.external_system_id,
                                                        externalsystems.field_name,
                                                        auditUserId,
                                                        auditWorkstation,
                                                    ExternalSystemFieldId,
                                                    ResultCode);
            }

            externalsystemfieldid = int.Parse(ExternalSystemFieldId.Value.ToString());
            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }


        internal List<ExternalSystemFieldsResult> GetExternalSystemsFields(int? externalsytemfieldid,int rowindex,int rowsperpage, long auditUserId, string auditWorkstation)
        {
            List<ExternalSystemFieldsResult> rtnValue = new List<ExternalSystemFieldsResult>();
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_external_system_fields(externalsytemfieldid, rowindex, rowsperpage, auditUserId, auditWorkstation);

                rtnValue = results.ToList();
            }

            return rtnValue;
        }
        internal List<LangLookup> LangLookupExternalSystems(int? language_id,  long auditUserId, string auditWorkstation)
        {
            List<LangLookup> rtnValue = new List<LangLookup>();
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_external_system_types(language_id, auditUserId, auditWorkstation);

                rtnValue = results.ToList();
            }

            return rtnValue;
        }


        internal SystemResponseCode UpdateExternalSystemFields(ExternalSystemFieldsResult externalsystem, long auditUserId, string auditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_update_external_system_fields(externalsystem.external_system_field_id, externalsystem.external_system_id, externalsystem.field_name,
                                       auditUserId, auditWorkstation,
                                       ResultCode);
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }

        internal SystemResponseCode DeleteExternalSystemField(int? externalsystemfieldid, long auditUserId, string auditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_delete_external_system_fields(externalsystemfieldid,
                                       auditUserId, auditWorkstation, ResultCode);
            }
            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }
        #endregion

    }
}
