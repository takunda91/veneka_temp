using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.CardManagement.objects;

namespace Veneka.Indigo.CardManagement.dal
{
    public interface ICardLimitDataAccess
    {
        bool CreateLimit(long cardId, decimal limit);
       
        bool UpdateLimit(long cardId, decimal limit);

        bool UpdateContractNumber(long cardId, string contract_number);

        bool ApproveLimit(long cardId,decimal limit, long userId);

        bool ApproveLimitManager(long cardId,  long userId);

        CardLimitModel GetLimit(long cardId);

        bool SetCreditStatus(long cardId, int creditStatusId);

        bool SetCreditContractNumber(long cardId, string creditContractNumber, long auditUserId);
    }
}
