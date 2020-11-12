using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Linq;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Utilities;
using IndigoFileLoader.objects;
using IndigoFileLoader.utility;
//using Veneka.Indigo.PINManagement.objects;
using Veneka.Indigo.Common.Models;
using System.Data.Objects;
using Veneka.Indigo.CardManagement;
using Veneka.Indigo.Common.DataAccess;

namespace IndigoFileLoader.dal
{
    internal class FileLoaderDAL
    {
        private readonly DatabaseConnectionObject dbObject = DatabaseConnectionObject.Instance;

        /// <summary>
        /// Create file_load
        /// </summary>
        /// <param name="searchFileName"></param>
        /// <returns></returns>
        public int CreateFileLoad(DateTimeOffset startTime, int filesToProcess)
        {
            ObjectParameter file_load_id = new ObjectParameter("file_load_id", typeof(int));

            using (var context = new indigo_databaseEntities(dbObject.EFSQLConnectionString))
            {
                context.usp_file_load_create(startTime, null, -2, filesToProcess, -2, "SYSTEM", file_load_id);
            }

            return int.Parse(file_load_id.Value.ToString());
        }

        /// <summary>
        /// Update the end time of file_load
        /// </summary>
        /// <param name="fileLoadId"></param>
        public void UpdateFileLoad(int fileLoadId)
        {
            using (var context = new indigo_databaseEntities(dbObject.EFSQLConnectionString))
            {
                context.usp_file_load_update(fileLoadId, DateTimeOffset.Now, -2, "SYSTEM");
            }
        }

        /// <summary>
        /// Search for a list of file loader logs.
        /// </summary>
        /// <param name="fileStatus"></param>
        /// <param name="fileName"></param>
        /// <param name="issuerId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowsPerpage"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<GetFileLoderLog_Result> SearchFileLoadLog(int? fileLoadId, FileStatus? fileStatus, string fileName, int? issuerId,
                                                              DateTime? dateFrom, DateTime? dateTo, int languageId, int pageIndex, int rowsPerpage,
                                                              long auditUserId, string auditWorkstation)
        {
            List<GetFileLoderLog_Result> rtnValue = new List<GetFileLoderLog_Result>();

            using (var context = new indigo_databaseEntities(dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_fileloaderlog(fileLoadId, (int?)fileStatus, fileName, issuerId, dateFrom, dateTo, languageId, pageIndex,
                                                         rowsPerpage, auditUserId, auditWorkstation);


                return results.ToList();
            }
        }

        /// <summary>
        /// Checks the DB if the file has already been processed. Returns true if file has been processed.
        /// </summary>
        /// <param name="searchFileName"></param>
        /// <returns></returns>
        public bool CheckIfFileExists(string searchFileName)
        {
            using (var context = new indigo_databaseEntities(dbObject.EFSQLConnectionString))
            {
                ObjectResult<file_history> results = context.usp_find_file_info_by_filename(searchFileName);

                if (results.Count() > 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the supplied list of card numbers is already in the load_cards table.
        /// </summary>
        /// <param name="cardList"></param>
        /// <returns></returns>
        public List<string> ValidateCardsLoaded(List<string> cardList)
        {
            List<string> rtnList = new List<string>();

            using (SqlConnection con = dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
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

                    SqlDataReader reader;
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        rtnList.Add(reader[0].ToString());
                    }

                    command.Parameters.Clear();
                    return rtnList;
                }
            }
        }

        /// <summary>
        /// Fetch issuer product for the card.
        /// </summary>
        /// <param name="binCode"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        internal List<issuer_product> FetchIssuerProduct(string binCode, int branchId)
        {
            var results = new IssuerManagementDataAccess().usp_get_products_by_bincode(binCode, null, null);
            return results;
        }

        /// <summary>
        /// Fetch an issuer based on bin code and branch code.
        /// Should produce one eunique issuer.
        /// </summary>
        /// <param name="binCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        internal bool FetchIssuerByProductAndBinCode(string binCode, string branchCode, out issuer cardFileIssuer)
        {
            cardFileIssuer = new issuer();
            using (var context = new indigo_databaseEntities(dbObject.EFSQLConnectionString))
            {

                var results = context.usp_get_issuer_by_product_and_branchcode(binCode, branchCode);

                List<issuer> issuers = results.ToList();

                if (issuers.Count() > 1)
                {
                    //throw new Exception("BIN code and branch code linked to more than, one issuer. Check configuaration.");
                    return false;
                }
                else if (issuers.Count == 0)
                {
                    return false;
                }

                cardFileIssuer = issuers.First();
                return true;
            }
        }


        //public bool ValidateCardLoaded(string cardNumber)
        //{
        //    //check if the card has been loaded into the DB
        //    try
        //    {
        //        using (SqlConnection con = dbObject.SQLConnection)
        //        {
        //            string sql =
        //                "OPEN SYMMETRIC KEY Indigo_Symmetric_Key " +
        //                "DECRYPTION BY CERTIFICATE Indigo_Certificate; " +
        //                "SELECT card_number FROM load_card WHERE " +
        //                "DECRYPTBYKEY(card_number) =  '" + cardNumber + "' " +
        //                "CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;";

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

        public bool CheckPinMailerLoaded(string cardNumber, string encryptedPinBlock)
        {
            //check if the PIN MAILER has been loaded into the DB
            try
            {
                using (SqlConnection con = dbObject.SQLConnection)
                {
                    string sql = " SELECT * FROM pin_mailer WHERE card_number ='" + cardNumber +
                                 "' AND encrypted_pin = '" + encryptedPinBlock + "'";

                    var command = new SqlCommand(sql, con);
                    SqlDataReader dataReader = command.ExecuteReader();

                    if (dataReader.HasRows)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                LogFileWriter.WriteFileLoaderError(ToString(), ex);
                return false;
            }
        }

        public bool SaveFileInfo(file_history fileHistory)
        {
            ObjectParameter fileId = new ObjectParameter("file_id", typeof(long));

            using (var context = new indigo_databaseEntities(dbObject.EFSQLConnectionString))
            {
                context.usp_insert_file_history(fileHistory.file_load_id,
                                                fileHistory.issuer_id,
                                                fileHistory.name_of_file,
                                                fileHistory.file_created_date,
                                                fileHistory.file_size,
                                                DateTimeOffset.Now,
                                                fileHistory.file_status_id,
                                                fileHistory.file_directory,
                                                fileHistory.number_successful_records,
                                                fileHistory.number_failed_records,
                                                fileHistory.file_load_comments,
                                                fileHistory.file_type_id,
                                                fileId);

            }

            return true;
        }

        internal bool InsertCardsListToTempTable(Dictionary<string, CardFileRecord> cardList)
        {
            using (SqlConnection con = dbObject.SQLConnection)
            {

                DataTable dt_CardList = new DataTable();
                dt_CardList.Columns.Add("card_number", typeof(String));
                dt_CardList.Columns.Add("branch_id", typeof(int));
                dt_CardList.Columns.Add("card_sequence", typeof(String));
                dt_CardList.Columns.Add("product_id", typeof(int));
                dt_CardList.Columns.Add("already_loaded", typeof(bool));
                dt_CardList.Columns.Add("card_id", typeof(long));
                dt_CardList.Columns.Add("load_batch_type_id", typeof(int));
                DataRow workRow;

                foreach (var card in cardList)
                {
                    workRow = dt_CardList.NewRow();
                    workRow["card_number"] = card.Key;
                    workRow["branch_id"] = card.Value.BranchId;
                    workRow["card_sequence"] = card.Value.SequenceNumber;
                    workRow["product_id"] = card.Value.ProductId;
                    workRow["already_loaded"] = false;
                    workRow["card_id"] = null;
                    workRow["load_batch_type_id"] = null;
                    dt_CardList.Rows.Add(workRow);
                }
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                {
                    //Set the database table name
                    sqlBulkCopy.DestinationTableName = "dbo.[temp_load_cards_type]";

                    //[OPTIONAL]: Map the DataTable columns with that of the database table
                    sqlBulkCopy.ColumnMappings.Add("card_number", "card_number");
                    sqlBulkCopy.ColumnMappings.Add("branch_id", "branch_id");
                    sqlBulkCopy.ColumnMappings.Add("card_sequence", "card_sequence");
                    sqlBulkCopy.ColumnMappings.Add("product_id", "product_id");
                    sqlBulkCopy.ColumnMappings.Add("already_loaded", "already_loaded");
                    sqlBulkCopy.ColumnMappings.Add("card_id", "card_id");
                    sqlBulkCopy.ColumnMappings.Add("load_batch_type_id", "load_batch_type_id");

                    con.Open();
                    sqlBulkCopy.WriteToServer(dt_CardList);
                    con.Close();
                    return true;
                }

            }
        }
        /// <summary>
        /// This method will run usp_create_load_batch.
        /// Inserts load_batch, load_batch_status, load_cards and file loader.
        /// Status for batch and cards should be "LOADED"
        /// </summary>
        internal bool CreateLoadBatch(Dictionary<string, CardFileRecord> cardList, string loadBatchReference, int issuer_id, file_history fileHistory)
        {
            if (InsertCardsListToTempTable(cardList))
            {

                using (SqlConnection con = dbObject.SQLConnection)
                {
                    using (SqlCommand command = con.CreateCommand())
                    {


                        DateTimeOffset dateTimeNow = DateTimeOffset.Now;

                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "[usp_create_load_batch]";

                        //command.Parameters.AddWithValue("@card_list", dt_CardList);
                        command.Parameters.Add("@load_batch_reference", SqlDbType.VarChar).Value = loadBatchReference;
                        command.Parameters.Add("@batch_status_id", SqlDbType.Int).Value = (int)LoadBatchStatus.LOADED;
                        command.Parameters.Add("@user_id", SqlDbType.BigInt).Value = -2; //Using the SYSTEM account.
                        command.Parameters.Add("@load_card_status_id", SqlDbType.Int).Value = (int)LoadCardStatus.LOADED;
                        command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuer_id;
                        command.Parameters.Add("@file_load_id", SqlDbType.Int).Value = fileHistory.file_load_id;
                        command.Parameters.Add("@name_of_file", SqlDbType.VarChar).Value = fileHistory.name_of_file;
                        command.Parameters.Add("@file_created_date", SqlDbType.DateTimeOffset).Value = fileHistory.file_created_date;
                        command.Parameters.Add("@file_size", SqlDbType.Int).Value = fileHistory.file_size;
                        command.Parameters.Add("@load_date", SqlDbType.DateTimeOffset).Value = dateTimeNow;
                        command.Parameters.Add("@file_status_id", SqlDbType.VarChar).Value = fileHistory.file_status_id;
                        command.Parameters.Add("@file_directory", SqlDbType.VarChar).Value = fileHistory.file_directory;
                        command.Parameters.Add("@number_successful_records", SqlDbType.Int).Value = 1;//fileHistory.number_successful_records;
                        command.Parameters.Add("@number_failed_records ", SqlDbType.Int).Value = 1;//fileHistory.number_failed_records;
                        command.Parameters.Add("@file_load_comments", SqlDbType.VarChar).Value = String.IsNullOrWhiteSpace(fileHistory.file_load_comments) ?
                                                                                                    "No Comment." : fileHistory.file_load_comments;
                        command.Parameters.Add("@file_type_id", SqlDbType.VarChar).Value = fileHistory.file_type_id;
                        //command.Parameters.Add("@number_successful_records", SqlDbType.Int).Direction = ParameterDirection.Output;                        
                        command.ExecuteNonQuery();

                        return true;
                    }
                }
            }
            return false;
        }

        internal void InsertPinBatchRecord(string batchReference, string loadedDT, int mailerCount, int issuerID,
                                           string branchCode)
        {
            //writes a load batch record to the load_batch table

            Console.WriteLine("Writing batch to DB");

            var sqlStatement = new StringBuilder("");
            sqlStatement.Append("INSERT INTO pin_batch VALUES (");
            sqlStatement.Append("'" + batchReference + "', ");
            sqlStatement.Append("'" + loadedDT + "', ");
            sqlStatement.Append("'" + mailerCount + "', ");
            sqlStatement.Append("'" + issuerID + "', ");
            sqlStatement.Append("'', "); //manager comment
            sqlStatement.Append("'LOADED',");
            sqlStatement.Append("'', "); //operator comment
            sqlStatement.Append("'" + branchCode + "')");
            try
            {
                using (SqlConnection con = dbObject.SQLConnection)
                {
                    string sql = sqlStatement.ToString();
                    var command = new SqlCommand(sql, con);
                    command.ExecuteNonQuery();
                    Console.WriteLine("Batch written to DB OK");
                }
            }
            catch (Exception ex)
            {
                LogFileWriter.WriteFileLoaderError(ToString(), ex);
            }
        }

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
    }
}