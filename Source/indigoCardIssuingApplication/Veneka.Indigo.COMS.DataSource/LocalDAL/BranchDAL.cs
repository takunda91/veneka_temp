using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.FileLoader.Objects;

namespace Veneka.Indigo.COMS.DataSource.LocalDAL
{
    public class BranchDAL : IBranchDAL
    {
        private string connectionString;

        public BranchDAL()
        {
            connectionString = DatabaseConnectionObject.Instance.SQLConnectionString; ;
        }

        public BranchLookup GetBranchesForIssuer(int issuerId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_branches_for_issuer]";

                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerId;

                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        return new BranchLookup(reader);
                    }
                }

                return new BranchLookup(string.Empty, 0, false);
            }
        }

        public BranchLookup GetBranch(string branchCode, string branchName, int issuerId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_branch]";

                    command.Parameters.Add("@branchName", SqlDbType.VarChar).Value = branchName;
                    command.Parameters.Add("@branchCode", SqlDbType.VarChar).Value = branchCode;
                    command.Parameters.Add("@issuerID", SqlDbType.Int).Value = issuerId;

                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        return new BranchLookup(reader);
                    }
                }

                return new BranchLookup(branchCode.Trim(), 0, false);
            }
        }

        public List<BranchLookup> GetCardCentreList(int issuerId)
        {
            List<BranchLookup> cardCentres = new List<BranchLookup>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_branch]";

                    command.Parameters.Add("@branchName", SqlDbType.VarChar).Value = String.Empty;
                    command.Parameters.Add("@branchCode", SqlDbType.VarChar).Value = String.Empty;
                    command.Parameters.Add("@issuerID", SqlDbType.Int).Value = issuerId;
                    command.Parameters.Add("@is_card_centre", SqlDbType.Bit).Value = 1;

                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        cardCentres.Add(new BranchLookup(reader));
                    }
                }                
            }

            return cardCentres;
        }

        public List<BranchLookup> GetBranchesForIssuerByIssuerCode(string issuerCode)
        {
            List<BranchLookup> issuerBranches = new List<BranchLookup>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_branches_for_issuer_code]";

                    command.Parameters.Add("@issuerCode", SqlDbType.VarChar).Value = issuerCode;

                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        issuerBranches.Add(new BranchLookup(reader));
                    }
                }
            }

            return issuerBranches;
        }
    }
}
