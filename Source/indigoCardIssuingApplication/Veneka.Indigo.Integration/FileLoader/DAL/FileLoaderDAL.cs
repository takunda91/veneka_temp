using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.FileLoader.Objects;
using Common.Logging;
using Veneka.Indigo.Integration.DAL;

namespace Veneka.Indigo.Integration.FileLoader.DAL
{
    public sealed class FileLoaderDAL:IFileLoaderDAL
    {
        private readonly string _connectionString;

        public FileLoaderDAL(string connectionString)
        {
            this._connectionString = connectionString;
        }

        /// <summary>
        /// Create file_load
        /// </summary>
        /// <param name="startTime">The DateTime that the processing of the files started</param>
        /// <param name="filesToProcess">How many files will be processed</param>
        /// <returns>File Load ID</returns>
        public int CreateFileLoad(DateTimeOffset startTime, int filesToProcess, long auditUserId, string auditWorkstation)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_file_load_create]";

                    command.Parameters.Add("@file_load_start", SqlDbType.DateTimeOffset).Value = startTime;
                    command.Parameters.Add("@file_load_end", SqlDbType.DateTimeOffset).Value = null;
                    command.Parameters.Add("@user_id", SqlDbType.BigInt).Value = auditUserId; //Default to system user
                    command.Parameters.Add("@files_to_process", SqlDbType.Int).Value = filesToProcess;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId; //Default to system user
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;

                    command.Parameters.Add("@file_load_id", SqlDbType.Int).Direction = ParameterDirection.Output;

                    con.Open();
                    command.ExecuteNonQuery();

                    return (int)command.Parameters["@file_load_id"].Value;
                }
            }
        }

        /// <summary>
        /// Update the end time of file_load
        /// </summary>
        /// <param name="fileLoadId"></param>
        public void UpdateFileLoad(int fileLoadId, long auditUserId, string auditWorkstation)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_file_load_update]";

                    command.Parameters.Add("@file_load_id", SqlDbType.Int).Value = fileLoadId;
                    command.Parameters.Add("@file_load_end", SqlDbType.DateTimeOffset).Value = DateTimeOffset.Now;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId; //Default to system user
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;

                    con.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        ///// <summary>
        ///// Search for a list of file loader logs.
        ///// </summary>
        ///// <param name="fileStatus"></param>
        ///// <param name="fileName"></param>
        ///// <param name="issuerId"></param>
        ///// <param name="dateFrom"></param>
        ///// <param name="dateTo"></param>
        ///// <param name="pageIndex"></param>
        ///// <param name="rowsPerpage"></param>
        ///// <param name="auditUserId"></param>
        ///// <param name="auditWorkstation"></param>
        ///// <returns></returns>
        //public List<GetFileLoderLog_Result> SearchFileLoadLog(int? fileLoadId, int? fileStatusId, string fileName, int? issuerId,
        //                                                      DateTime? dateFrom, DateTime? dateTo, int languageId, int pageIndex, int rowsPerpage,
        //                                                      long auditUserId, string auditWorkstation)
        //{
        //    using (SqlConnection con = new SqlConnection(_connectionString))
        //    {
        //        using (SqlCommand command = con.CreateCommand())
        //        {
        //            command.CommandType = CommandType.StoredProcedure;
        //            command.CommandText = "[usp_GetFileLoderLog]";

        //            command.Parameters.Add("@file_load_id", SqlDbType.DateTime).Value = fileLoadId;
        //            command.Parameters.Add("@file_load_end", SqlDbType.DateTime).Value = DateTime.Now;
        //            command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = -2; //Default to system user
        //            command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = "SYSTEM";

        //            command.ExecuteNonQuery();
        //        }
        //    }

        //    //List<GetFileLoderLog_Result> rtnValue = new List<GetFileLoderLog_Result>();

        //    //using (var context = new indigo_databaseEntities(dbObject.EFSQLConnectionString))
        //    //{
        //    //    var results = context.usp_GetFileLoderLog(fileLoadId, (int?)fileStatus, fileName, issuerId, dateFrom, dateTo, languageId, pageIndex,
        //    //                                             rowsPerpage, auditUserId, auditWorkstation);


        //    //    return results.ToList();
        //    //}
        //}

        /// <summary>
        /// Checks the DB if the file has already been processed. Returns true if file has been processed.
        /// </summary>
        /// <param name="searchFileName"></param>
        /// <returns></returns>
        public bool CheckIfFileExists(string searchFileName)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_find_file_info_by_filename]";

                    command.Parameters.Add("@filename", SqlDbType.VarChar).Value = searchFileName;

                    con.Open();
                    var reader = command.ExecuteReader();

                    return reader.HasRows;
                }
            }
        }

        /// <summary>
        /// Fetchs the card centre branch for an issuer
        /// </summary>
        /// <param name="issuerId"></param>
        /// <returns></returns>
        public BranchLookup FetchCardCentreBranch(int issuerId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_branches_for_issuer]";

                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerId;
                    command.Parameters.Add("@card_centre_branch", SqlDbType.Bit).Value = 1;

                    con.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        return new BranchLookup(reader);
                    }
                }
            }

            return new BranchLookup(String.Empty, 0, false);
        }

        public BranchLookup FetchBranchByBranchCode(int issuerId, string branchCode)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_search_branch]";

                    command.Parameters.Add("@branch_name", SqlDbType.VarChar).Value = null;
                    command.Parameters.Add("@branch_code", SqlDbType.VarChar).Value = branchCode.Trim();
                    command.Parameters.Add("@branch_status_id", SqlDbType.Int).Value = 0;
                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerId;

                    con.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        return new BranchLookup(reader);
                    }
                }
            }

            return new BranchLookup(branchCode.Trim(), 0, false);
        }

        public List<CardsOrder> FetchOutstandingOrder(int productId, int numberOfCards, long auditUserId, string auditWorkstation)
        {
            List<CardsOrder> orders = new List<CardsOrder>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_outstanding_orders]";

                    command.Parameters.Add("@product_id", SqlDbType.Int).Value = productId;
                    command.Parameters.Add("@number_of_cards", SqlDbType.Int).Value = numberOfCards;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;

                    con.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        orders.Add(new CardsOrder(reader));
                    }
                }
            }

            return orders;
        }

        public List<CardOrderCard> FetchCardsForOrder(long distBatchId, long auditUserId, string auditWorkstation)
        {
            List<CardOrderCard> cards = new List<CardOrderCard>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_outstanding_order_cards]";

                    command.Parameters.Add("@dist_batch_id", SqlDbType.Int).Value = distBatchId;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;

                    con.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        cards.Add(new CardOrderCard(reader));
                    }
                }
            }

            return cards;
        }

        /// <summary>
        /// Checks if the supplied list of card numbers is already in the load_cards table.
        /// </summary>
        /// <param name="cardList"></param>
        /// <returns></returns>
        public List<string> ValidateCardsLoaded(List<string> cardList)
        {
            List<string> rtnList = new List<string>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    DataTable dt_DistBatchCards = new DataTable();
                    dt_DistBatchCards.Columns.Add("card_number", typeof(String));
                    DataRow workRow;

                    foreach (var card in cardList)
                    {
                        workRow = dt_DistBatchCards.NewRow();
                        workRow["card_number"] = card;
                        dt_DistBatchCards.Rows.Add(workRow);
                    }
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "usp_find_distinct_load_cards";
                    command.Parameters.AddWithValue("@card_list", dt_DistBatchCards);

                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        rtnList.Add(reader[0].ToString().Trim());
                    }

                    return rtnList;
                }
            }
        }

        /// <summary>
        /// Checks if the supplied list of card reference numbers have already been loaded and processed.
        /// </summary>
        /// <param name="cardRefList"></param>
        /// <returns></returns>
        public List<string> ValidateCardReferencesLoaded(List<string> cardRefList)
        {
            List<string> rtnList = new List<string>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    DataTable dt_key_value_array = new DataTable();
                    dt_key_value_array.Columns.Add("key", typeof(long));
                    dt_key_value_array.Columns.Add("value", typeof(String));
                    DataRow workRow;

                    int i = 0;
                    foreach (var cardRef in cardRefList)
                    {
                        workRow = dt_key_value_array.NewRow();
                        workRow["key"] = i++;
                        workRow["value"] = cardRef;
                        dt_key_value_array.Rows.Add(workRow);
                    }


                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "usp_find_distinct_load_requests";
                    command.Parameters.AddWithValue("@card_ref_list", dt_key_value_array);

                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        rtnList.Add(reader[0].ToString().Trim());
                    }

                    return rtnList;
                }
            }
        }

        /// <summary>
        /// Pass in card_id, searches DB for load_pending cards, returns a list of all found card_id's and product id.
        /// </summary>
        /// <param name="cardList"></param>
        /// <returns></returns>
        public List<Tuple<long, int>> ValidateCardsOrdered(List<Tuple<long, string, int>> cardList)
        {
            List<Tuple<long, int>> rtnList = new List<Tuple<long, int>>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    DataTable dt_key_value_array = new DataTable();
                    dt_key_value_array.Columns.Add("key", typeof(long));
                    dt_key_value_array.Columns.Add("value", typeof(String));
                    DataRow workRow;

                    foreach (var card in cardList)
                    {
                        workRow = dt_key_value_array.NewRow();
                        workRow["key"] = card.Item1;
                        workRow["value"] = card.Item2;
                        dt_key_value_array.Rows.Add(workRow);
                    }
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "usp_validate_cards_ordered";
                    command.Parameters.AddWithValue("@card_list", dt_key_value_array);

                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        rtnList.Add(Tuple.Create<long, int>(long.Parse(reader[0].ToString().Trim()),
                                                            int.Parse(reader[1].ToString())));
                    }

                    return rtnList;
                }
            }
        }

        /// <summary>
        /// Fetch the issuer by the issuer's code
        /// </summary>
        /// <param name="issuerCode"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Issuer FetchIssuerByCode(string issuerCode, long auditUserId, string auditWorkstation)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_issuer_by_code]";

                    command.Parameters.Add("@issuer_code", SqlDbType.Char).Value = issuerCode;
                    command.Parameters.Add("@audit_user_id", SqlDbType.VarChar).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;

                    con.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        return new Issuer(reader);
                    }
                }
            }

            return null;
        }


        ///// <summary>
        ///// Fetch issuer product for the card.
        ///// </summary>
        ///// <param name="binCode"></param>
        ///// <param name="issuerId"></param>
        ///// <returns></returns>
        //internal List<IssuerProduct> FetchIssuerProduct(string binCode, int issuerId)
        //{
        //    using (SqlConnection con = new SqlConnection(_connectionString))
        //    {
        //        using (SqlCommand command = con.CreateCommand())
        //        {
        //            command.CommandType = CommandType.StoredProcedure;
        //            command.CommandText = "[usp_get_products_by_bincode]";

        //            command.Parameters.Add("@bin_code", SqlDbType.Char).Value = binCode;
        //            command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerId;

        //            con.Open();
        //            var reader = command.ExecuteReader();

        //            List<IssuerProduct> results = new List<IssuerProduct>();

        //            while(reader.Read())
        //            {
        //                results.Add(new IssuerProduct(reader));
        //            }

        //            return results;
        //        }
        //    }
        //}

        /// <summary>
        /// Fetch an issuer based on bin code and branch code.
        /// Should produce one unique issuer.
        /// </summary>
        /// <param name="binCode">Will trim the string to 6 digits</param>
        /// <param name="branchCode">May be null</param>
        /// <returns></returns>
        internal Issuer FetchIssuerByProductAndBinCode(string binCode, string branchCode)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_issuer_by_product_and_branchcode]";

                    command.Parameters.Add("@bin_code", SqlDbType.Char).Value = binCode;
                    command.Parameters.Add("@branch_code", SqlDbType.VarChar).Value = branchCode;

                    con.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        return new Issuer(reader);
                    }
                }
            }

            return null;
        }

        //cardFileIssuer = new issuer();
        //using (var context = new indigo_databaseEntities(dbObject.EFSQLConnectionString))
        //{

        //    var results = context.usp_get_issuer_by_product_and_branchcode(binCode, branchCode);

        //    List<issuer> issuers = results.ToList();

        //    if (issuers.Count() > 1)
        //    {
        //        //throw new Exception("BIN code and branch code linked to more than, one issuer. Check configuaration.");
        //        return false;
        //    }
        //    else if (issuers.Count == 0)
        //    {
        //        return false;
        //    }

        //    cardFileIssuer = issuers.First();
        //    return true;
        //}

        //   cardFileIssuer = null;

        //   return false;
        //}        

        public bool SaveFileInfo(FileHistory fileHistory)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_insert_file_history]";

                    command.Parameters.Add("@file_load_id", SqlDbType.Int).Value = fileHistory.FileLoadId;
                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = fileHistory.IssuerId;
                    command.Parameters.Add("@name_of_file", SqlDbType.VarChar).Value = fileHistory.NameOfFile;
                    command.Parameters.Add("@file_created_date", SqlDbType.DateTimeOffset).Value = fileHistory.FileCreatedDate;
                    command.Parameters.Add("@file_size", SqlDbType.Int).Value = fileHistory.FileSize;
                    command.Parameters.Add("@load_date", SqlDbType.DateTimeOffset).Value = fileHistory.LoadDate;
                    command.Parameters.Add("@file_status_id", SqlDbType.Int).Value = fileHistory.FileStatusId;
                    command.Parameters.Add("@file_directory", SqlDbType.VarChar).Value = fileHistory.FileDirectory;
                    command.Parameters.Add("@number_successful_records", SqlDbType.Int).Value = fileHistory.NumberSuccessfulRecords ?? 0;
                    command.Parameters.Add("@number_failed_records", SqlDbType.Int).Value = fileHistory.NumberFailedRecords ?? 0;
                    command.Parameters.Add("@file_load_comments", SqlDbType.VarChar).Value = fileHistory.FileLoadComments;
                    command.Parameters.Add("@file_type_id", SqlDbType.Int).Value = fileHistory.FileTypeId;

                    SqlParameter outputFileIdParam = new SqlParameter("@file_id", SqlDbType.BigInt)
                    {
                        Direction = ParameterDirection.InputOutput,
                        Value = fileHistory.FileId
                    };
                    command.Parameters.Add(outputFileIdParam);

                    con.Open();
                    command.ExecuteNonQuery();

                    fileHistory.FileId = (long)command.Parameters["@file_id"].Value;
                }
            }

            return true;
        }
        internal bool InsertCardsListToTempTable(CardFile cardFile)
        {
           
                DataTable dt_CardList = new DataTable();
                dt_CardList.Columns.Add("card_number", typeof(String));
                dt_CardList.Columns.Add("card_reference", typeof(String));
                DataColumn branchcolumn;
                branchcolumn = new DataColumn("branch_id", typeof(int));
                branchcolumn.AllowDBNull = true;
                dt_CardList.Columns.Add(branchcolumn);

                dt_CardList.Columns.Add("card_sequence", typeof(int));

                DataColumn datecolumn;
                datecolumn = new DataColumn("expiry_date", typeof(DateTime));
                datecolumn.AllowDBNull = true;
                dt_CardList.Columns.Add(datecolumn);

                dt_CardList.Columns.Add("product_id", typeof(int));
                dt_CardList.Columns.Add("card_issue_method_id", typeof(int));

                DataColumn column;
                column = new DataColumn("sub_product_id", typeof(int));
                column.AllowDBNull = true;
                dt_CardList.Columns.Add(column);

                dt_CardList.Columns.Add("already_loaded", typeof(bool));

                DataColumn cardIdColumn;
                cardIdColumn = new DataColumn("card_id", typeof(long));
                cardIdColumn.AllowDBNull = true;
                dt_CardList.Columns.Add(cardIdColumn);

                dt_CardList.Columns.Add("load_batch_type_id", typeof(int));
                DataRow workRow;

                foreach (var card in cardFile.CardFileRecords)
                {
                    workRow = dt_CardList.NewRow();

                    workRow["card_number"] = !String.IsNullOrWhiteSpace(card.PsuedoPAN) ? card.PsuedoPAN : card.PAN;
                    workRow["card_reference"] = card.CardReference;

                    if (card.BranchId == null)
                        workRow["branch_id"] = DBNull.Value;
                    else
                        workRow["branch_id"] = card.BranchId;

                    workRow["card_sequence"] = string.IsNullOrEmpty(card.SequenceNumber) ? 0 : int.Parse(card.SequenceNumber);

                    if (card.ExpiryDate != null)
                        workRow["expiry_date"] = card.ExpiryDate;
                    else
                        workRow["expiry_date"] = DBNull.Value;

                    workRow["product_id"] = card.ProductId;
                    workRow["card_issue_method_id"] = card.CardIssueMethodId;

                    //if (card.SubProductCode != null)
                    //    workRow["sub_product_id"] = card.SubProductCode;
                    //else
                    //Not used anymore - Default to null
                    workRow["sub_product_id"] = DBNull.Value;

                    workRow["already_loaded"] = false;

                    if (card.CardId != null)
                        workRow["card_id"] = card.CardId;
                    else
                        workRow["card_id"] = DBNull.Value;

                    workRow["load_batch_type_id"] = card.ProductLoadTypeId.Value;

                    dt_CardList.Rows.Add(workRow);
                }
            int fileloader_status_id = -1;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();


                DataTable dt = new DataTable();
                DateTimeOffset executed_datetime = DateTimeOffset.Now;
                using (SqlDataAdapter cmd = new SqlDataAdapter("select [fileloader_status],executed_datetime from [dbo].[fileloader_status]", con))
                {

                    cmd.Fill(dt);
                  
                 
                    con.Close();
                    fileloader_status_id = int.Parse(dt.Rows[0]["fileloader_status"].ToString());
                    executed_datetime = DateTimeOffset.Parse(dt.Rows[0]["executed_datetime"].ToString());
                }
                if (fileloader_status_id == 1 && (DateTimeOffset.Now.DateTime-executed_datetime.DateTime).TotalMinutes<3)
                {
                    throw new Exception("another file loader in progress.");
                }
                else if (fileloader_status_id == 2 || (fileloader_status_id == 1 && (DateTimeOffset.Now.DateTime - executed_datetime.DateTime).TotalMinutes > 3)) //clean up process
                {
                    TemptableCleanup(con);
                }
                if (con.State == ConnectionState.Closed)
                    con.Open();
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {


                        // Must assign both transaction object and connection 


                        //Set the database table name
                        sqlBulkCopy.DestinationTableName = "dbo.[temp_load_cards_type]";

                        //[OPTIONAL]: Map the DataTable columns with that of the database table
                        sqlBulkCopy.ColumnMappings.Add("card_number", "card_number");
                        sqlBulkCopy.ColumnMappings.Add("branch_id", "branch_id");
                        sqlBulkCopy.ColumnMappings.Add("card_sequence", "card_sequence");
                        sqlBulkCopy.ColumnMappings.Add("card_issue_method_id", "card_issue_method_id");
                        sqlBulkCopy.ColumnMappings.Add("card_reference", "card_reference");

                    
                        sqlBulkCopy.ColumnMappings.Add("product_id", "product_id");
                        sqlBulkCopy.ColumnMappings.Add("sub_product_id", "sub_product_id");
                        sqlBulkCopy.ColumnMappings.Add("already_loaded", "already_loaded");
                        sqlBulkCopy.ColumnMappings.Add("card_id", "card_id");
                        sqlBulkCopy.ColumnMappings.Add("load_batch_type_id", "load_batch_type_id");

                       
                        sqlBulkCopy.WriteToServer(dt_CardList);
                        con.Close();
                            if (con.State == ConnectionState.Closed)
                                con.Open();
                            using (SqlCommand cmd1 = new SqlCommand("update [dbo].[fileloader_status] set [fileloader_status]=1,[executed_datetime] ='" + DateTimeOffset.Now+"'", con))
                            {
                                cmd1.CommandType = CommandType.Text;

                                cmd1.ExecuteNonQuery();
                                con.Close();
                            }
                    return true;
                    }
                
                


            }
        }
        private void TemptableCleanup(SqlConnection con)
        {

            if (con.State == ConnectionState.Closed)
                con.Open();
            using (SqlCommand cmd1 = new SqlCommand("delete from [temp_load_cards_type]", con))
            {
                cmd1.CommandType = CommandType.Text;

                cmd1.ExecuteNonQuery();
                con.Close();
            }
            if (con.State == ConnectionState.Closed)
                con.Open();
            using (SqlCommand cmd1 = new SqlCommand("update [dbo].[fileloader_status] set [fileloader_status]=0,[executed_datetime] ='" + DateTimeOffset.Now + "'", con))
            {
                cmd1.CommandType = CommandType.Text;

                cmd1.ExecuteNonQuery();
                con.Close();
            }

        }
        /// <summary>
        /// This method will run sp_create_load_batch.
        /// Inserts load_batch, load_batch_status, load_cards and file loader.
        /// Status for batch and cards should be "LOADED"
        /// </summary>
        internal bool CreateLoadBatch(CardFile cardFile, string loadBatchReference, FileHistory fileHistory, long auditUserId, string auditWorkstation, out long? loadBatchId)
        {
            loadBatchId = 0;
            try
            {
                if (InsertCardsListToTempTable(cardFile))
                {
                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand command = con.CreateCommand())
                        {

                            command.CommandTimeout = 240;

                            DateTimeOffset dateTimeNow = DateTimeOffset.Now;

                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = "[usp_create_load_batch]";

                            //command.Parameters.AddWithValue("@card_list", dt_CardList);
                            command.Parameters.Add("@load_batch_reference", SqlDbType.VarChar).Value = loadBatchReference;
                            command.Parameters.Add("@batch_status_id", SqlDbType.Int).Value = 0;
                            command.Parameters.Add("@user_id", SqlDbType.BigInt).Value = auditUserId; //Using the SYSTEM account.
                            command.Parameters.Add("@load_card_status_id", SqlDbType.Int).Value = 0;
                            command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = cardFile.CardFileRecords[0].IssuerId;
                            command.Parameters.Add("@file_load_id", SqlDbType.Int).Value = fileHistory.FileLoadId;
                            command.Parameters.Add("@name_of_file", SqlDbType.VarChar).Value = fileHistory.NameOfFile;
                            command.Parameters.Add("@file_created_date", SqlDbType.DateTimeOffset).Value = fileHistory.FileCreatedDate;
                            command.Parameters.Add("@file_size", SqlDbType.Int).Value = fileHistory.FileSize;
                            command.Parameters.Add("@load_date", SqlDbType.DateTimeOffset).Value = dateTimeNow;
                            command.Parameters.Add("@file_status_id", SqlDbType.VarChar).Value = fileHistory.FileStatusId;
                            command.Parameters.Add("@file_directory", SqlDbType.VarChar).Value = fileHistory.FileDirectory;
                            command.Parameters.Add("@number_successful_records", SqlDbType.Int).Value = cardFile.CardFileRecords.Count;//fileHistory.number_successful_records;
                            command.Parameters.Add("@number_failed_records ", SqlDbType.Int).Value = 0;//fileHistory.number_failed_records;
                            command.Parameters.Add("@file_load_comments", SqlDbType.VarChar).Value = String.IsNullOrWhiteSpace(fileHistory.FileLoadComments) ?
                                                                                                        "No Comment." : fileHistory.FileLoadComments;
                            command.Parameters.Add("@file_type_id", SqlDbType.VarChar).Value = fileHistory.FileTypeId;
                            command.Parameters.Add("@load_batch_type_id", SqlDbType.Int).Value = 1;
                            command.Parameters.Add("@order_batch_id", SqlDbType.BigInt).Value = cardFile.OrderBatchId;
                            //command.Parameters.Add("@number_successful_records", SqlDbType.Int).Direction = ParameterDirection.Output; 

                            command.Parameters.Add("@file_id", SqlDbType.BigInt).Direction = ParameterDirection.Output;
                            command.Parameters.Add("@load_batch_id", SqlDbType.BigInt).Direction = ParameterDirection.Output;
                            command.Parameters.Add("@load_batch_status_id", SqlDbType.BigInt).Direction = ParameterDirection.Output;

                            con.Open();
                            command.ExecuteNonQuery();

                            fileHistory.FileId = (long?)command.Parameters["@file_id"].Value;
                            loadBatchId = (long?)command.Parameters["@load_batch_id"].Value;
                            TemptableCleanup(con);
                            return true;
                        }
                    }
                }
            }
            catch(Exception ex)
            {

                throw ex;
            }
            return false;
        }

              
        

        //internal IssuerProduct FetchProductByCodeAndIssuer(int issuerId, string productCode)
        //{
        //    using (SqlConnection con = new SqlConnection(_connectionString))
        //    {
        //        using (SqlCommand command = con.CreateCommand())
        //        {
        //            command.CommandType = CommandType.StoredProcedure;
        //            command.CommandText = "[usp_get_products_by_productCode]";

        //            command.Parameters.Add("@product_code", SqlDbType.Char).Value = productCode;
        //            command.Parameters.Add("@issuer_id", SqlDbType.VarChar).Value = issuerId;

        //            con.Open();
        //            var reader = command.ExecuteReader();

        //            while (reader.Read())
        //            {
        //                return new IssuerProduct(reader);
        //            }
        //        }
        //    }

        //    return null;
        //}


        /// <summary>
        /// Bulk Card Requests
        /// </summary>
        /// <param name="cardRequestFile"></param>
        /// <param name="loadBatchReference"></param>
        /// <param name="fileHistory"></param>
        /// <returns></returns>
        public bool CreateBulkRequestLoadBatch(BulkRequestsFile cardRequestFile, string loadBatchReference, FileHistory fileHistory, long auditUserId, string auditWorkstation)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandTimeout = 60;

                    DataTable dt_CardRequestList = new DataTable();
                    dt_CardRequestList.Columns.Add("temp_customer_account_id", typeof(long));
                    dt_CardRequestList.Columns.Add("card_number", typeof(String));
                    dt_CardRequestList.Columns.Add("reference_number", typeof(String));
                    dt_CardRequestList.Columns.Add("branch_id", typeof(int));
                    dt_CardRequestList.Columns.Add("product_id", typeof(int));
                    dt_CardRequestList.Columns.Add("card_priority_id", typeof(int));
                    dt_CardRequestList.Columns.Add("customer_account_number", typeof(String));
                    dt_CardRequestList.Columns.Add("domicile_branch_id", typeof(int));
                    dt_CardRequestList.Columns.Add("account_type_id", typeof(int));
                    dt_CardRequestList.Columns.Add("card_issue_reason_id", typeof(int));
                    dt_CardRequestList.Columns.Add("customer_first_name", typeof(String));
                    dt_CardRequestList.Columns.Add("customer_middle_name", typeof(String));
                    dt_CardRequestList.Columns.Add("customer_last_name", typeof(String));
                    dt_CardRequestList.Columns.Add("name_on_card", typeof(String));
                    dt_CardRequestList.Columns.Add("customer_title_id", typeof(int));
                    dt_CardRequestList.Columns.Add("currency_id", typeof(int));
                    dt_CardRequestList.Columns.Add("resident_id", typeof(int));
                    dt_CardRequestList.Columns.Add("customer_type_id", typeof(int));
                    dt_CardRequestList.Columns.Add("cms_id", typeof(String));
                    dt_CardRequestList.Columns.Add("contract_number", typeof(String));
                    dt_CardRequestList.Columns.Add("idnumber", typeof(String));
                    dt_CardRequestList.Columns.Add("contact_number", typeof(String));
                    dt_CardRequestList.Columns.Add("customer_id", typeof(String));
                    dt_CardRequestList.Columns.Add("fee_waiver_YN", typeof(bool));
                    dt_CardRequestList.Columns.Add("fee_editable_YN", typeof(bool));
                    dt_CardRequestList.Columns.Add("fee_charged", typeof(decimal));
                    dt_CardRequestList.Columns.Add("fee_overridden_YN", typeof(bool));
                    dt_CardRequestList.Columns.Add("audit_user_id", typeof(long));
                    dt_CardRequestList.Columns.Add("audit_workstation", typeof(String));

                    DataColumn column;
                    column = new DataColumn("sub_product_id", typeof(String));
                    column.AllowDBNull = true;
                    dt_CardRequestList.Columns.Add(column);

                    dt_CardRequestList.Columns.Add("load_product_batch_type_id", typeof(int));

                    dt_CardRequestList.Columns.Add("already_loaded", typeof(bool));
                    DataRow workRow;

                    List<Tuple<long, long, byte[]>> productFieldsList = new List<Tuple<long, long, byte[]>>();
                    int tempCardAccountId = 0;

                    foreach (var card in cardRequestFile.CardRequestFileRecords)
                    {
                        workRow = dt_CardRequestList.NewRow();
                        workRow["temp_customer_account_id"] = tempCardAccountId;
                        workRow["card_number"] = card.CardNumber;
                        workRow["reference_number"] = card.RequestReferenceNumber;
                        workRow["branch_id"] = card.BranchId;
                        workRow["product_id"] = card.ProductId;
                        workRow["card_priority_id"] = card.CardPriorityId;
                        workRow["customer_account_number"] = card.CustomerAccountNumber;
                        workRow["domicile_branch_id"] = card.BranchId;
                        workRow["account_type_id"] = card.AccountTypeId;
                        workRow["card_issue_reason_id"] = card.CardIssueReasonId;
                        workRow["customer_first_name"] = card.CustomerFirstName;
                        workRow["customer_middle_name"] = card.CustomerMiddleName;
                        workRow["customer_last_name"] = card.CustomerLastName;
                        workRow["name_on_card"] = card.NameOnCard;
                        workRow["customer_title_id"] = card.CustomerTitleId;
                        workRow["currency_id"] = card.CurrencyId;
                        workRow["resident_id"] = card.ResidentId;
                        workRow["customer_type_id"] = card.CustomerTypeId;
                        workRow["cms_id"] = card.CmsId;
                        workRow["contract_number"] = card.ContractNumber;
                        workRow["idnumber"] = card.IdNumber;
                        workRow["contact_number"] = card.ContactNumber;
                        workRow["customer_id"] = card.CustomerId;
                        workRow["fee_waiver_YN"] = card.FeeWaiverYN;
                        workRow["fee_editable_YN"] = card.FeeEditableYN;
                        workRow["fee_charged"] = card.FeeCharged;
                        workRow["fee_overridden_YN"] = card.FeeOverriddenYN;
                        workRow["audit_user_id"] = auditUserId;
                        workRow["audit_workstation"] = auditWorkstation;

                        if (!String.IsNullOrWhiteSpace(card.SubProductCode))
                            workRow["sub_product_id"] = card.SubProductCode;
                        else
                            workRow["sub_product_id"] = DBNull.Value;

                        workRow["load_product_batch_type_id"] = card.ProductLoadTypeId;                        

                        workRow["already_loaded"] = false;
                        dt_CardRequestList.Rows.Add(workRow);

                        foreach(var field in card.PrintFields)
                        {
                            byte[] value;

                            if (field is ProductPrinting.PrintStringField)
                                value = System.Text.Encoding.UTF8.GetBytes(((ProductPrinting.PrintStringField)field).Value);
                            else if (field is ProductPrinting.PrintImageField)
                                value = ((ProductPrinting.PrintImageField)field).Value;
                            else
                                throw new ArgumentException("ProductField is of an unknown type.");

                            productFieldsList.Add(Tuple.Create<long, long, byte[]>(tempCardAccountId, field.ProductPrintFieldId, value));
                        }

                        tempCardAccountId++;
                    }

                    DateTimeOffset dateTimeNow = DateTimeOffset.Now;

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_create_load_batch_bulk_card_request]";

                    command.Parameters.AddWithValue("@card_list", dt_CardRequestList);
                    command.Parameters.AddWithValue("@product_fields", UtilityClass.CreateBiKeyBinaryValueTable2(productFieldsList));


                    command.Parameters.Add("@load_batch_reference", SqlDbType.VarChar).Value = loadBatchReference;
                    command.Parameters.Add("@batch_status_id", SqlDbType.Int).Value = 0;
                    command.Parameters.Add("@user_id", SqlDbType.BigInt).Value = auditUserId; //Using the SYSTEM account.
                    command.Parameters.Add("@load_card_status_id", SqlDbType.Int).Value = 0;
                    command.Parameters.Add("@load_batch_type_id", SqlDbType.Int).Value = 2;
                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = cardRequestFile.CardRequestFileRecords[0].IssuerId;
                    command.Parameters.Add("@file_load_id", SqlDbType.Int).Value = fileHistory.FileLoadId;
                    command.Parameters.Add("@name_of_file", SqlDbType.VarChar).Value = fileHistory.NameOfFile;
                    command.Parameters.Add("@file_created_date", SqlDbType.DateTimeOffset).Value = fileHistory.FileCreatedDate;
                    command.Parameters.Add("@file_size", SqlDbType.Int).Value = fileHistory.FileSize;
                    command.Parameters.Add("@load_date", SqlDbType.DateTimeOffset).Value = dateTimeNow;
                    command.Parameters.Add("@file_status_id", SqlDbType.VarChar).Value = fileHistory.FileStatusId;
                    command.Parameters.Add("@file_directory", SqlDbType.VarChar).Value = fileHistory.FileDirectory;
                    command.Parameters.Add("@number_successful_records", SqlDbType.Int).Value = cardRequestFile.CardRequestFileRecords.Count;//fileHistory.number_successful_records;
                    command.Parameters.Add("@number_failed_records ", SqlDbType.Int).Value = 0;//fileHistory.number_failed_records;
                    command.Parameters.Add("@file_load_comments", SqlDbType.VarChar).Value = String.IsNullOrWhiteSpace(fileHistory.FileLoadComments) ?
                                                                                                "No Comment." : fileHistory.FileLoadComments;
                    command.Parameters.Add("@file_type_id", SqlDbType.VarChar).Value = fileHistory.FileTypeId;
                    //command.Parameters.Add("@number_successful_records", SqlDbType.Int).Direction = ParameterDirection.Output;                        

                    con.Open();
                    command.ExecuteNonQuery();

                    return true;
                }
            }
        }

        #region PIN Batch Stuff
        //public bool CheckPinMailerLoaded(string cardNumber, string encryptedPinBlock)
        //{
        //    //check if the PIN MAILER has been loaded into the DB
        //    try
        //    {
        //        using (SqlConnection con = dbObject.SQLConnection)
        //        {
        //            string sql = " SELECT * FROM pin_mailer WHERE card_number ='" + cardNumber +
        //                         "' AND encrypted_pin = '" + encryptedPinBlock + "'";

        //            var command = new SqlCommand(sql, con);
        //            SqlDataReader dataReader = command.ExecuteReader();

        //            if (dataReader.HasRows)
        //                return true;
        //            else
        //                return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogFileWriter.WriteFileLoaderError(ToString(), ex);
        //        return false;
        //    }
        //}

        //internal void InsertPinBatchRecord(string batchReference, string loadedDT, int mailerCount, int issuerID,
        //                                   string branchCode)
        //{
        //    //writes a load batch record to the load_batch table

        //    Console.WriteLine("Writing batch to DB");

        //    var sqlStatement = new StringBuilder("");
        //    sqlStatement.Append("INSERT INTO pin_batch VALUES (");
        //    sqlStatement.Append("'" + batchReference + "', ");
        //    sqlStatement.Append("'" + loadedDT + "', ");
        //    sqlStatement.Append("'" + mailerCount + "', ");
        //    sqlStatement.Append("'" + issuerID + "', ");
        //    sqlStatement.Append("'', "); //manager comment
        //    sqlStatement.Append("'LOADED',");
        //    sqlStatement.Append("'', "); //operator comment
        //    sqlStatement.Append("'" + branchCode + "')");
        //    try
        //    {
        //        using (SqlConnection con = dbObject.SQLConnection)
        //        {
        //            string sql = sqlStatement.ToString();
        //            var command = new SqlCommand(sql, con);
        //            command.ExecuteNonQuery();
        //            Console.WriteLine("Batch written to DB OK");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogFileWriter.WriteFileLoaderError(ToString(), ex);
        //    }
        //}

        //internal void InsertPinRecords(string batchReference, List<PINMailer> pinMailers)
        //{
        //    var sqlStatement = new StringBuilder("");

        //    Console.WriteLine("Writing PINs to DB");
        //    foreach (PINMailer mailer in pinMailers)
        //    {
        //        sqlStatement.Append("INSERT INTO pin_mailer VALUES (");
        //        sqlStatement.Append("'', "); //pin ref
        //        sqlStatement.Append("'" + batchReference + "', "); //batch ref
        //        sqlStatement.Append("'" + mailer.Status + "', "); //status
        //        sqlStatement.Append("'" + mailer.CardNumber + "', "); //card number
        //        sqlStatement.Append("'', "); //offset/pvv
        //        sqlStatement.Append("'" + mailer.EncryptedPIN + "', ");
        //        sqlStatement.Append("'" + mailer.CustomerName + "', ");
        //        sqlStatement.Append("'" + mailer.CustomerAddress + "', ");
        //        sqlStatement.Append("'', "); //date printed
        //        sqlStatement.Append("'', "); //date printed
        //        sqlStatement.Append("'') "); //reprinted request
        //    }
        //    try
        //    {
        //        using (SqlConnection con = dbObject.SQLConnection)
        //        {
        //            var command = new SqlCommand(sqlStatement.ToString(), con);
        //            command.ExecuteNonQuery();
        //            Console.WriteLine("PINs to written to DB OK");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogFileWriter.WriteFileLoaderError(ToString(), ex);
        //    }
        //}
        #endregion

        internal IssuerProduct ValidateProduct(int issuerId, string productCode)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_products_by_productcode]";

                    command.Parameters.Add("@product_code", SqlDbType.VarChar).Value = productCode;
                    command.Parameters.Add("@issuer_id", SqlDbType.VarChar).Value = issuerId;
                    command.Parameters.Add("@only_active_records", SqlDbType.Bit).Value = true;

                    con.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        return new IssuerProduct(reader);
                    }
                }
            }

            return null;
        }
    }
}
