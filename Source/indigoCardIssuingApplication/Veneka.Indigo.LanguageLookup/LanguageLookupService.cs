using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Models;

namespace Veneka.Indigo.LanguageLookup
{
    public sealed class LanguageLookupService
    {
        private static volatile LanguageLookupService instance;
        private static object syncRoot = new Object();

        private static readonly LanguageLookupDAL _langLookupDal = new LanguageLookupDAL();

        private LanguageLookupService() { }

        public static LanguageLookupService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new LanguageLookupService();
                    }
                }

                return instance;
            }
        }

        public List<language> GetLanguages(long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "Languages";

            // Check the cache
            List<language> items = CacheLayer.Get<List<language>>(cacheKey);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLanguages(auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey);
                }
            }

            return items;
        }

        /// <summary>
        /// Fetch user statuses from cache or db, based on language
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<LangLookup> LangLookupUserStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "UserStatuses";

            // Check the cache
            List<LangLookup> userStatuses = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (userStatuses == null)
            {
                // Go and retrieve the data from the DB
                userStatuses = _langLookupDal.GetLangLookupUserStatuses(languageId, auditUserId, auditWorkstation);

                if (userStatuses != null && userStatuses.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(userStatuses, cacheKey + languageId);
                }
            }

            return userStatuses;
        }

        /// <summary>
        /// Fetch user roles based from cache or db based on language
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<LangLookup> LangLookupUserRoles(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "UserRoles";

            // Check the cache
            List<LangLookup> userRoles = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (userRoles == null)
            {
                // Go and retrieve the data from the DB
                userRoles = _langLookupDal.GetLangLookupUserRoles(languageId, auditUserId, auditWorkstation);

                if (userRoles != null && userRoles.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(userRoles, cacheKey + languageId);
                }
            }

            return userRoles;
        }

        public List<LangLookup> LangLookupAuditActions(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "AuditActions";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupAuditActions(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }

        public List<LangLookup> LangLookupBranchCardStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "BranchCardStatuses";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupBranchCardStatuses(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }

        public List<LangLookup> LangLookupBranchStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "BranchStatuses";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupBranchStatuses(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }

        public List<LangLookup> LangLookupCardIssueReasons(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "CardIssueReasons";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupCardIssueReasons(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }

        public List<LangLookup> LangLookupCustomerAccountTypes(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "CustomerAccountTypes";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupCustomerAccountTypes(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }

        public List<LangLookup> LangLookupDistBatchStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "DistBatchStatuses";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupDistBatchStatuses(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }

        public List<LangLookup> LangLookupDistCardStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "DistCardStatuses";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupDistCardStatuses(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }

        public List<LangLookup> LangLookupFileStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "FileStatuses";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupFileStatuses(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }

        public List<LangLookup> LangLookupInterfaceTypes(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "InterfaceTypes";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupInterfaceTypes(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }

        public List<LangLookup> LangLookupIssuerStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "IssuerStatuses";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupIssuerStatuses(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }

        public List<LangLookup> LangLookupLoadBatchStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "LoadBatchStatuses";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupLoadBatchStatuses(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }

        public List<LangLookup> LangLookupLoadCardStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "LoadCardStatuses";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupLoadCardStatuses(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }

        public List<LangLookup> LangLookupCustomerResidency(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "CustomerResidency";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupCustomerResidency(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }

        public List<LangLookup> LangLookupCustomerTitle(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "CustomerTitle";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupCustomerTitle(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }

        public List<LangLookup> LangLookupCustomerType(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "CustomerType";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupCustomerType(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }

        public List<LangLookupChar> LangLookupGenderType(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "GenderType";

            // Check the cache
            List<LangLookupChar> items = CacheLayer.Get<List<LangLookupChar>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupGenderType(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }

        public List<LangLookup> LangLookupConnectionParameterType(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "ConnectionParameterType";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupConnectionParameterType(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }
        public List<LangLookup> LangLookupCardIssueMethod(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "CardIssueMethod";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupCardIssueMethod(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }
        public List<LangLookup> LangLookupPinBatchStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "PinBatchStatus";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupPinBatchStatus(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }

        public List<LangLookup> LangLookupPinReissueStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "PinReissueStatus";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupPinReissueStatus(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }

        public List<LangLookup> LangLookupExportBatchStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "ExportBatchStatuses";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupExportBatchStatuses(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }
        
        public List<LangLookup> LangLookupRemoteUpdateStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "RemoteUpdateStatuses";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupRemoteUpdateStatuses(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }


        public List<LangLookup> LangLookupProductLoadTypes(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "ProductLoadTypes";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupProductLoadTypes(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }

       

        public List<LangLookup> LangLookupHybridRequestStatues(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "HybridRequestStatuses";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupHybridRequestStatues(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }
        public List<LangLookup> LangLookupPrintBatchStatues(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "PrintBatchStatuses";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupPrintBatchStatues(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }
        public List<LangLookup> LangLookupBranchTypes(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "BranchTypes";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetBranchTypes(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }

        public List<LangLookup> LangLookupFileEncryptionTypes(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "FileEncryptionTypes";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupFileEncryptionTypes(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }

        public List<LangLookup> LangLookupThreedBatchStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            const string cacheKey = "ThreedBatches";

            // Check the cache
            List<LangLookup> items = CacheLayer.Get<List<LangLookup>>(cacheKey + languageId);

            if (items == null)
            {
                // Go and retrieve the data from the DB
                items = _langLookupDal.GetLangLookupThreedBatches(languageId, auditUserId, auditWorkstation);

                if (items != null && items.Count > 0)
                {
                    // Then add it to the cache so we 
                    // can retrieve it from there next time
                    CacheLayer.Add(items, cacheKey + languageId);
                }
            }

            return items;
        }
    }
}
