using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Utilities;
using IndigoFileLoader.objects;
using IndigoFileLoader.utility;
using Veneka.Indigo.Common.Models;
using System.Data.Objects;

namespace IndigoFileLoader.dal
{
    internal class IssuerConfigDAL
    {
        private readonly DatabaseConnectionObject dbObject = DatabaseConnectionObject.Instance;

        /// <summary>
        /// Returns all active issuers.
        /// </summary>
        /// <returns></returns>
        internal List<issuer> GetIssuerConfiguration()
        {
            var rtnList = new List<issuer>();
            try
            {
                using (var context = new indigo_databaseEntities(dbObject.EFSQLConnectionString))
                {
                    ObjectResult<issuer> results = context.usp_find_issuer_by_status((int)IssuerStatus.ACTIVE);

                    foreach (issuer result in results)
                    {
                        rtnList.Add(result);
                    }
                }
            }
            catch (Exception ex)
            {
                LogFileWriter.WriteFileLoaderComment("Unable to access the database, cannot run the file loader");
                LogFileWriter.WriteFileLoaderError(ToString(), ex);
            }
            return rtnList;
        }
    }
}