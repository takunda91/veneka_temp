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
using Veneka.Indigo.Integration.Remote;

namespace Veneka.Indigo.RemoteManagement.DAL
{
    public class RemoteCardUpdateDAL : IRemoteCardUpdateDAL
    {
        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;

        public RemoteCardUpdates GetPendingCardUpdates(int issuerId, string remoteComponentIP, long auditUserId, string auditWorkstation)
        {
            var remoteUpdates = new RemoteCardUpdates();

            using (SqlConnection connection = _dbObject.SQLConnection)
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_remote_get_pending]";
                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerId;
                    command.Parameters.Add("@remote_component_ip", SqlDbType.VarChar).Value = remoteComponentIP;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            remoteUpdates.Cards.Add(new CardDetail(reader));
                        }

                        reader.NextResult();

                        while (reader.Read())
                        {
                            remoteUpdates.ProductSettings.Add(new Settings(reader));
                        }

                        reader.NextResult();

                        while(reader.Read())
                        {
                            //remote setting for CMS not Remote CMS!!
                            var productId = (int)reader["product_id"];
                            var externalSytemTypeId = (int)reader["external_system_type_id"];

                            Settings productSetting;

                            switch (externalSytemTypeId)
                            {
                                case 1: productSetting = remoteUpdates.ProductSettings.Where(w => w.ProductId == productId && w.IntegrationTypeId == 2).FirstOrDefault(); break;
                                case 2: productSetting = remoteUpdates.ProductSettings.Where(w => w.ProductId == productId && w.IntegrationTypeId == 9).FirstOrDefault(); break;
                                default:
                                    productSetting = null;
                                    break;
                            }

                            if (productSetting != null)
                                productSetting.ExternalFields.Field.Add(reader["field_name"].ToString(), reader["field_value"].ToString());
                        }
                    }
                }
            }


            remoteUpdates.MakeSuccess();

            return remoteUpdates;
        }

        public void SetCardUpdates(string remoteComponentAddress, List<CardDetailResponse> cardDetails, long auditUserId, string auditWorkstation)
        {
            using (SqlConnection connection = _dbObject.SQLConnection)
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    DataTable dt_remoteCards = new DataTable();
                    dt_remoteCards.Columns.Add("card_id", typeof(long));
                    dt_remoteCards.Columns.Add("successful", typeof(bool));
                    dt_remoteCards.Columns.Add("comment", typeof(String));
                    dt_remoteCards.Columns.Add("time_update", typeof(DateTime));
                    DataRow workRow;

                    foreach (var card in cardDetails)
                    {
                        workRow = dt_remoteCards.NewRow();
                        workRow["card_id"] = card.CardId;
                        workRow["successful"] = card.UpdateSuccessful;
                        workRow["comment"] = card.Detail;
                        workRow["time_update"] = card.TimeUpdated;
                        dt_remoteCards.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_remote_set_card_updates]";
                    command.Parameters.Add("@remote_component_address", SqlDbType.VarChar).Value = remoteComponentAddress;
                    command.Parameters.AddWithValue("@card_updates", dt_remoteCards);
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;

                    if(connection.State == ConnectionState.Closed)
                        connection.Open();
                    var reader = command.ExecuteReader();
                }
            }
        }
        
        public List<RemoteCardUpdateSearchResult> SearchRemoteCardUpdates(string pan, int? remoteUpdateStatusesId, int? issuerId, int? branchId, int? productId, DateTime? dateFrom, 
                                                DateTime? dateTo, int pageIndex, int rowsPerPage, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_remote_card_update_search(pan, remoteUpdateStatusesId, issuerId, branchId, productId, dateFrom, dateTo, pageIndex, rowsPerPage, languageId, auditUserId, auditWorkstation);
                return result.ToList();
            }
        }

        public RemoteCardUpdateDetailResult GetRemoteCardDetail(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_remote_card_update_get_detail(cardId, languageId, auditUserId, auditWorkstation);
                return result.FirstOrDefault();
            }
        }

        public RemoteCardUpdateDetailResult ChangeRemoteCardStatus(long cardId, int remoteUpdateStatusesId, string comment, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_remote_card_update_change_status(cardId, remoteUpdateStatusesId, comment, languageId, auditUserId, auditWorkstation);
                return result.FirstOrDefault();
            }
        }

        public void ChangeRemoteCardsStatus(List<long> cardId, int remoteUpdateStatusesId, string comment, int languageId, long auditUserId, string auditWorkstation)
        {
            using (SqlConnection connection = _dbObject.SQLConnection)
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    DataTable dt_remoteCards = new DataTable();
                    dt_remoteCards.Columns.Add("card_id", typeof(long));
                    dt_remoteCards.Columns.Add("branch_card_statuses_id", typeof(int));
                    DataRow workRow;

                    foreach (var card in cardId)
                    {
                        workRow = dt_remoteCards.NewRow();
                        workRow["card_id"] = card;
                        workRow["branch_card_statuses_id"] = 0;
 
                        dt_remoteCards.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_remote_card_update_change_statuses]";                    
                    command.Parameters.AddWithValue("@card_id_list", dt_remoteCards);
                    command.Parameters.Add("@remote_card_update_statuses_id", SqlDbType.Int).Value = remoteUpdateStatusesId;
                    command.Parameters.Add("@comment", SqlDbType.VarChar).Value = comment;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    var reader = command.ExecuteNonQuery();
                }
            }
        }
    }
}
