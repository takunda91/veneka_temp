using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Integration.Remote;
using Veneka.Indigo.RemoteManagement.DAL;

namespace Veneka.Indigo.RemoteManagement
{
    public class RemoteCardUpdateService
    {
        private readonly IRemoteCardUpdateDAL _remoteCardUpdateDAL;

        public RemoteCardUpdateService() :this(new RemoteCardUpdateDAL())
        {   }

        public RemoteCardUpdateService(IRemoteCardUpdateDAL remoteComponentDAL)
        {
            _remoteCardUpdateDAL = remoteComponentDAL;
        }

        public RemoteCardUpdates GetPendingCardUpdates(int issuerId, string remoteComponentIP, long auditUserId, string auditWorkstation)
        {
            return _remoteCardUpdateDAL.GetPendingCardUpdates(issuerId, remoteComponentIP, auditUserId, auditWorkstation);
        }

        public void SetCardUpdates(string remoteComponentAddress, List<CardDetailResponse> cardDetails, long auditUserId, string auditWorkstation)
        {
            _remoteCardUpdateDAL.SetCardUpdates(remoteComponentAddress, cardDetails, auditUserId, auditWorkstation);
        }

        public List<RemoteCardUpdateSearchResult> SearchRemoteCardUpdates(string pan, int? remoteUpdateStatusesId, int? issuerId, int? branchId, int? productId, DateTime? dateFrom,
                                        DateTime? dateTo, int pageIndex, int rowsPerPage, int languageId, long auditUserId, string auditWorkstation)
        {
            return _remoteCardUpdateDAL.SearchRemoteCardUpdates(pan, remoteUpdateStatusesId, issuerId, branchId, productId, dateFrom, dateTo, pageIndex, rowsPerPage, languageId, auditUserId, auditWorkstation);
        }

        public RemoteCardUpdateDetailResult GetRemoteCardDetail(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _remoteCardUpdateDAL.GetRemoteCardDetail(cardId, languageId, auditUserId, auditWorkstation);
        }

        public RemoteCardUpdateDetailResult ChangeRemoteCardStatus(long cardId, int remoteUpdateStatusesId, string comment, int languageId, long auditUserId, string auditWorkstation)
        {
            return _remoteCardUpdateDAL.ChangeRemoteCardStatus(cardId, remoteUpdateStatusesId, comment, languageId, auditUserId, auditWorkstation);
        }

        public void ChangeRemoteCardsStatus(List<long> cardIds, int remoteUpdateStatusesId, string comment, int languageId, long auditUserId, string auditWorkstation)
        {
            _remoteCardUpdateDAL.ChangeRemoteCardsStatus(cardIds, remoteUpdateStatusesId, comment, languageId, auditUserId, auditWorkstation);
        }
    }
}
