using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common.Database;

namespace Veneka.Indigo.LanguageLookup
{
    internal class LanguageLookupDAL
    {
        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;

        internal List<language> GetLanguages(long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_languages(0, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }

        /// <summary>
        /// Fetch user statuses based on language
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal List<LangLookup> GetLangLookupUserStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_user_status(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }

        /// <summary>
        /// Fetch user roles based on language
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal List<LangLookup> GetLangLookupUserRoles(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_user_roles(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }

        internal List<LangLookup> GetLangLookupAuditActions(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_audit_actions(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }

        internal List<LangLookup> GetLangLookupBranchCardStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_branch_card_statuses(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }

        internal List<LangLookup> GetLangLookupBranchStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_branch_statuses(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }

        internal List<LangLookup> GetLangLookupCardIssueReasons(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_card_issue_reasons(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }

        internal List<LangLookup> GetLangLookupCustomerAccountTypes(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_customer_account_types(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }

        internal List<LangLookup> GetLangLookupDistBatchStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_dist_batch_statuses(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }

        internal List<LangLookup> GetLangLookupDistCardStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_dist_card_statuses(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }

        internal List<LangLookup> GetLangLookupFileStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_file_statuses(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }

        internal List<LangLookup> GetLangLookupInterfaceTypes(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_interface_types(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }

        internal List<LangLookup> GetLangLookupIssuerStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_issuer_statuses(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }

        internal List<LangLookup> GetLangLookupLoadBatchStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_load_batch_statuses(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }

        internal List<LangLookup> GetLangLookupLoadCardStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_load_card_statuses(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }

        internal List<LangLookup> GetLangLookupCustomerResidency(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_customer_residency(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }

        internal List<LangLookup> GetLangLookupCustomerTitle(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_customer_title(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }

        internal List<LangLookup> GetLangLookupCustomerType(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_customer_type(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }

        internal List<LangLookupChar> GetLangLookupGenderType(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_gender(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }

        internal List<LangLookup> GetLangLookupConnectionParameterType(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_connection_parameter_type(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }
        internal List<LangLookup> GetLangLookupCardIssueMethod(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_card_issue_method(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }
        internal List<LangLookup> GetLangLookupPinBatchStatus(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_pin_batch_statuses(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }
        internal List<LangLookup> GetLangLookupPinReissueStatus(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_pin_reissue_statuses(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }
        internal List<LangLookup> GetLangLookupExportBatchStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_export_batch_statuses(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }
        
        internal List<LangLookup> GetLangLookupRemoteUpdateStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_remote_update_statuses(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }

        internal List<LangLookup> GetLangLookupProductLoadTypes(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_product_load_type(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }
        internal List<LangLookup> GetBranchTypes(int languageId, long auditUserId, string auditWorkstation)
        {

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_branch_types(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }

        }
        internal List<LangLookup> GetLangLookupPrintBatchStatues(int languageId, long auditUserId, string auditWorkstation)
        {

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_print_batch_statues(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }

        }
        internal List<LangLookup> GetLangLookupHybridRequestStatues(int languageId, long auditUserId, string auditWorkstation)
        {

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_hybrid_request_statues(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }

        }
        internal List<LangLookup> GetLangLookupFileEncryptionTypes(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_file_encryption_type(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }

        internal List<LangLookup> GetLangLookupThreedBatches(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_lang_lookup_Threed_batch_statues(languageId, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }
    }
}
