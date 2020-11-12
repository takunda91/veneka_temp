using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.Old_App_Code.service;
using indigoCardIssuingWeb.security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace indigoCardIssuingWeb.service
{
    public class DocumentManagementService : BaseService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DocumentManagementService));

        internal int DocumentTypeSave(DocumentType entity)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);
            if (entity.Id == 0)
            {
                var response = m_indigoApp.DocumentTypeCreate(entity, encryptedSessionKey);
                return response.Value;
            }
            else
            {
                var response = m_indigoApp.DocumentTypeUpdate(entity, encryptedSessionKey);

                return response.Value;
            }
        }

        internal SystemResponseCode DocumentTypeDelete(int id)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.DocumentTypeDelete(id, encryptedSessionKey);
            return response.Value;
        }

        internal DocumentType DocumentTypeGet(int id)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);
            var response = m_indigoApp.DocumentTypeGet(id, encryptedSessionKey);
            return response.Value;
        }

        internal List<DocumentType> DocumentTypeAll(bool isActive)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.DocumentTypeGetAll(isActive, encryptedSessionKey);
            return response.Value.ToList();
        }

        internal int ProductDocumentSave(ProductDocument entity)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);
            if (entity.Id == 0)
            {
                var response = m_indigoApp.ProductDocumentCreate(entity, encryptedSessionKey);
                return response.Value;
            }
            else
            {
                var response = m_indigoApp.ProductDocumentUpdate(entity, encryptedSessionKey);

                return response.Value;
            }
        }

        internal SystemResponseCode ProductDocumentDelete(int id)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.ProductDocumentDelete(id, encryptedSessionKey);
            return response.Value;
        }

        internal ProductDocumentListModel ProductDocumentGet(int id)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);
            var response = m_indigoApp.ProductDocumentGet(id, encryptedSessionKey);
            return response.Value;
        }

        internal List<ProductDocumentListModel> ProductDocumentAll(int productId, bool isActive)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.ProductDocumentGetAll(productId, isActive, encryptedSessionKey);
            return response.Value.ToList();
        }

        internal ProductDocumentStructure GetProductDocumentStructure(int productId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetProductDocuments(productId, encryptedSessionKey);
            return response.Value;
        }

        internal List<CardDocument> UploadLocalFiles(string accountNumber, string customerId, List<LocalFileModel> localFiles)
        {
            try
            {
                log.Info("Save files");
                var localFolder = ConfigurationManager.AppSettings["LocalDocumentsLocation"];
                List<CardDocument> savedFiles = new List<CardDocument>();
                if (!Directory.Exists(localFolder))
                {
                    throw new Exception("Directory not accessible");
                }

                foreach (var item in localFiles)
                {
                    string generatedFile = string.Format("{0}_{1}{2}", DateTime.Now.ToString("yyyyMMddhhmmss"), Guid.NewGuid().ToString().Replace("-", ""), Path.GetExtension(item.FileName));
                    string newFileName = Path.Combine(localFolder, generatedFile);
                    File.WriteAllBytes(newFileName, item.Content);
                    savedFiles.Add(new CardDocument() { Location = newFileName, Id = 0 });
                }
                return savedFiles;
            }
            catch (Exception)
            {
                throw;
            }
        }

        internal List<long> CardDocumentSaveBulk(List<CardDocument> entity)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CardDocumentCreateBulk(entity.ToArray(), encryptedSessionKey);
            return response.Value.ToList();
        }

        internal long CardDocumentSave(CardDocument entity)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);
            if (entity.Id == 0)
            {
                var response = m_indigoApp.CardDocumentCreate(entity, encryptedSessionKey);
                return response.Value;
            }
            else
            {
                var response = m_indigoApp.CardDocumentUpdate(entity, encryptedSessionKey);

                return response.Value;
            }
        }

        internal SystemResponseCode CardDocumentDelete(int id)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CardDocumentDelete(id, encryptedSessionKey);
            return response.Value;
        }

        internal CardDocument CardDocumentGet(int id)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);
            var response = m_indigoApp.CardDocumentGet(id, encryptedSessionKey);
            return response.Value;
        }

        internal List<CardDocument> CardDocumentAll(long cardId, bool isActive)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CardDocumentGetAll(cardId, isActive, encryptedSessionKey);
            return response.Value.ToList();
        }

        internal List<CardDocument> CardDocumentsRemote(string accountNumber)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetRemoteDocuments(accountNumber, encryptedSessionKey);
            return response.Value.ToList();
        }

        internal byte[] DownloadRemoteDocument(string documentKey)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.DownloadRemoteDocument(documentKey, encryptedSessionKey);
            return response.Value;
        }
    }
}