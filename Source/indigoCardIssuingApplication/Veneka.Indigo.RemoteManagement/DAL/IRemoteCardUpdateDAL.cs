using System;
using System.Collections.Generic;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Integration.Remote;

namespace Veneka.Indigo.RemoteManagement.DAL
{
    public interface IRemoteCardUpdateDAL
    {
        RemoteCardUpdates GetPendingCardUpdates(int issuerId, string remoteComponentIP, long auditUserId, string auditWorkstation);
        void SetCardUpdates(string remoteComponentAddress, List<CardDetailResponse> cardDetails, long auditUserId, string auditWorkstation);

        List<RemoteCardUpdateSearchResult> SearchRemoteCardUpdates(string pan, int? remoteUpdateStatusesId, int? issuerId, int? branchId, int? productId, DateTime? dateFrom,
                                        DateTime? dateTo, int pageIndex, int rowsPerPage, int languageId, long auditUserId, string auditWorkstation);

        RemoteCardUpdateDetailResult GetRemoteCardDetail(long cardId, int languageId, long auditUserId, string auditWorkstation);

        RemoteCardUpdateDetailResult ChangeRemoteCardStatus(long cardId, int remoteUpdateStatusesId, string comment, int languageId, long auditUserId, string auditWorkstation);

        void ChangeRemoteCardsStatus(List<long> cardIds, int remoteUpdateStatusesId, string comment, int languageId, long auditUserId, string auditWorkstation);
    }
}