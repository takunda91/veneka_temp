using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Models;
using Common.Logging;

namespace Veneka.Indigo.Common.Language
{
    public class ResponseTranslator : IResponseTranslator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ResponseTranslator));
        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;

        /// <summary>
        /// returns the response in the users selected language
        /// </summary>
        /// <param name="responseCode"></param>
        /// <param name="systemArea"></param>
        /// <param name="language"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public string TranslateResponseCode(SystemResponseCode responseCode, SystemArea systemArea, int language, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_lookup_response_message((int)responseCode, (int)systemArea, auditUserId, auditWorkstation);

                var langResult = result.First();

                switch (language)
                {
                    case 0:
                        return langResult.english_response;
                    case 1:
                        return langResult.french_response;
                    case 2:
                        return langResult.portuguese_response;
                    case 3:
                        return langResult.spanish_response;
                    default:
                        return langResult.english_response;
                }
            }
        }
    }
}
