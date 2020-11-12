using System;
using Veneka.Indigo.COMS.Core.Integration;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Integration.External;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.COMS.Core
{
    public class ComPrepaidSystem : IComPrepaidSystem
    {
        public readonly IDataSource _dataSource;

        public ComPrepaidSystem(IDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public ComsResponse<PrepaidCreditResponse> CreditPrepaidAccount(decimal amount, string destinationAccountNumber, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            PrepaidCreditResponse creditResponse;
            string responseMessage;

            try
            {
                var resp = IntegrationController.Instance
                            .PrepaidAccountSystem(interfaceInfo, _dataSource)
                            .CreditPrepaidAccount(externalFields, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, amount, destinationAccountNumber, out creditResponse, out responseMessage);

                if (resp)
                {
                    return ComsResponse<PrepaidCreditResponse>.Success(responseMessage, creditResponse);
                }

            }
            catch (Exception ex)
            {
                return ComsResponse<PrepaidCreditResponse>.Exception(ex, null);
            }

            return ComsResponse<PrepaidCreditResponse>.Failed(responseMessage, null);
        }

        public ComsResponse<PrepaidAccountDetail> GetPrepaidAccountDetail(string cardNumber, int mbr, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            PrepaidAccountDetail prepaidAccount;
            string responseMessage;

            try
            {
                var resp = IntegrationController.Instance
                            .PrepaidAccountSystem(interfaceInfo, _dataSource)
                            .GetPrepaidAccountDetail(externalFields, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, cardNumber, mbr, out prepaidAccount, out responseMessage);

                if (resp)
                {
                    return ComsResponse<PrepaidAccountDetail>.Success(responseMessage, prepaidAccount);
                }

            }
            catch (Exception ex)
            {
                return ComsResponse<PrepaidAccountDetail>.Exception(ex, null);
            }

            return ComsResponse<PrepaidAccountDetail>.Failed(responseMessage, null);
        }
    }
}