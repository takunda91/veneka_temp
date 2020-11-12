using Common.Logging;
using IndigoCardIssuanceService.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Veneka.Indigo.CardManagement;
using Veneka.Indigo.CardManagement.dal;
using Veneka.Indigo.Renewal;
using Veneka.Indigo.Renewal.Entities;
using Veneka.Indigo.Renewal.Incoming;
using Veneka.Indigo.Common.Language;
using Veneka.Indigo.COMS.Core;
using Veneka.Indigo.COMS.DataSource;
using Veneka.Indigo.Integration;
using Veneka.Indigo.IssuerManagement;
using Veneka.Indigo.DocumentManagement;
using Veneka.Indigo.Common;
using IndigoCardIssuanceService.Models;
using System.Configuration;
using System.IO;

namespace IndigoCardIssuanceService.bll
{
    public class DocumentManagementController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RenewalController));
        private readonly CardMangementService _cardManService = new CardMangementService(new LocalDataSource());
        private readonly BranchService _branchService = new BranchService();
        private readonly IDocumentManagementOperations _documentManagement = new DocumentManagementOperations();

        public DocumentManagementController() : this(new LocalDataSource(), new CardManagementDAL(), null, null, null)
        {

        }

        public DocumentManagementController(IDataSource dataSource, Veneka.Indigo.CardManagement.dal.ICardManagementDAL cardManagementDAL, IIntegrationController integration, IComsCore comsCore, IResponseTranslator translator)
        {

        }

        internal Response<int> DocumentTypeSave(DocumentType entity, int languageId, long userId, string auditWorkstation)
        {
            try
            {
                int newId = 0;
                if (entity.Id == 0)
                {
                    newId = _documentManagement.DocumentTypeCreate(entity, userId, auditWorkstation);
                }
                else
                {
                    newId = _documentManagement.DocumentTypeUpdate(entity, userId, auditWorkstation);
                }
                return new Response<int>(newId, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<int>(0, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<SystemResponseCode> DocumentTypeDelete(int id, int languageId, long userId, string auditWorkstation)
        {
            try
            {

                var result = _documentManagement.DocumentTypeDelete(id, userId, auditWorkstation);

                return new Response<SystemResponseCode>(result, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<SystemResponseCode>(SystemResponseCode.GENERAL_FAILURE, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<DocumentType> DocumentTypeGet(int id, int languageId, long userId, string auditWorkstation)
        {
            try
            {

                var result = _documentManagement.DocumentTypeRetrieve(id, languageId, userId, auditWorkstation);

                return new Response<DocumentType>(result, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<DocumentType>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<DocumentType>> DocumentTypeAll(bool isActive, int languageId, long userId, string auditWorkstation)
        {
            try
            {

                var result = _documentManagement.DocumentTypeRetrieveAll(isActive);

                return new Response<List<DocumentType>>(result, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<DocumentType>>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<int> ProductDocumentSave(ProductDocument entity, int languageId, long userId, string auditWorkstation)
        {
            try
            {
                int newId = 0;
                if (entity.Id == 0)
                {
                    newId = _documentManagement.ProductDocumentCreate(entity, userId, auditWorkstation);
                }
                else
                {
                    newId = _documentManagement.ProductDocumentUpdate(entity, userId, auditWorkstation);
                }
                return new Response<int>(newId, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<int>(0, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<SystemResponseCode> ProductDocumentDelete(int id, int languageId, long userId, string auditWorkstation)
        {
            try
            {

                var result = _documentManagement.ProductDocumentDelete(id, userId, auditWorkstation);

                return new Response<SystemResponseCode>(result, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<SystemResponseCode>(SystemResponseCode.GENERAL_FAILURE, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<ProductDocumentListModel> ProductDocumentGet(int id, int languageId, long userId, string auditWorkstation)
        {
            try
            {

                var result = _documentManagement.ProductDocumentRetrieve(id, languageId, userId, auditWorkstation);

                return new Response<ProductDocumentListModel>(result, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<ProductDocumentListModel>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<ProductDocumentListModel>> ProductDocumentAll(int productId, bool isActive, int languageId, long userId, string auditWorkstation)
        {
            try
            {

                var result = _documentManagement.ProductDocumentRetrieveAll(productId, isActive);

                return new Response<List<ProductDocumentListModel>>(result, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<ProductDocumentListModel>>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<long> CardDocumentSave(CardDocument entity, int languageId, long userId, string auditWorkstation)
        {
            try
            {
                long newId = 0;
                if (entity.Id == 0)
                {
                    newId = _documentManagement.CardDocumentCreate(entity, userId, auditWorkstation);
                }
                else
                {
                    newId = _documentManagement.CardDocumentUpdate(entity, userId, auditWorkstation);
                }
                return new Response<long>(newId, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<long>(0, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<SystemResponseCode> CardDocumentDelete(long id, int languageId, long userId, string auditWorkstation)
        {
            try
            {

                var result = _documentManagement.CardDocumentDelete(id, userId, auditWorkstation);

                return new Response<SystemResponseCode>(result, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<SystemResponseCode>(SystemResponseCode.GENERAL_FAILURE, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<CardDocument> CardDocumentGet(long id, int languageId, long userId, string auditWorkstation)
        {
            try
            {

                var result = _documentManagement.CardDocumentRetrieve(id, languageId, userId, auditWorkstation);

                return new Response<CardDocument>(result, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<CardDocument>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<CardDocument>> CardDocumentAll(long cardId, bool isActive, int languageId, long userId, string auditWorkstation)
        {
            try
            {

                var result = _documentManagement.CardDocumentRetrieveAll(cardId);

                return new Response<List<CardDocument>>(result, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<CardDocument>>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal List<LocalFileModel> SaveLocalFiles(string accountNumber, string customerId, List<LocalFileModel> documents)
        {
            try
            {
                log.Info("Save files");
                var localFolder = ConfigurationManager.AppSettings["LocalDocumentsLocation"];
                List<LocalFileModel> savedFiles = new List<LocalFileModel>();
                if (Directory.Exists(localFolder))
                {
                    throw new Exception("Directory not accessible");
                }

                foreach (var item in documents)
                {
                    string generatedFile = string.Format("{0}_{1}.{2}", DateTime.Now.ToString("yyyyMMddhhmmss"), Guid.NewGuid().ToString().Replace("-", ""), Path.GetExtension(item.FileName));
                    string newFileName = Path.Combine(localFolder, generatedFile);
                    File.WriteAllBytes(newFileName, item.Content);
                    savedFiles.Add(new LocalFileModel() { FileName = newFileName, AccountNumber = accountNumber, CustomerId = customerId });
                }
                return savedFiles;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}