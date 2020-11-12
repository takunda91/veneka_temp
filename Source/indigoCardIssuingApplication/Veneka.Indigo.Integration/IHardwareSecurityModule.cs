using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.Objects;
using System.ComponentModel.Composition;
using Common.Logging;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common.Models.IssuerManagement;

namespace Veneka.Indigo.Integration
{
    /// <summary>
    /// This interfaces provides a contract between Indigo and the Hardware Security Module integration layer.
    /// </summary>
    [InheritedExport(typeof(IHardwareSecurityModule))]
    public interface IHardwareSecurityModule : ICommon
    {
        bool GeneratePVV(ref List<CardObject> cardObjects, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage);

        bool GenerateCVV(ref List<CardObject> cardObjects, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage);

        bool GenerateCardEncryptionData(ref List<CardObject> cardObjects, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage);

        bool PrintPins(ref List<CardObject> cardObjects, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage);

        bool GenerateRandomKey(int issuerId, string tmk, string deviceId, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage, out TerminalSessionKey randomKeys);

        //bool ReadTrackData(int issuerId, string keyUnderLMK, TerminalCardData cardData, string deviceId, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage, out Track2 track2);

        bool DecryptFields(ZoneMasterKey zmk, TerminalSessionKey tpk, Product product, TerminalCardData termCardData, string operatorCode, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage, out DecryptedFields decryptedFields);

        bool DecryptFieldsWithMessaging(ZoneMasterKey zmk, TerminalSessionKey tpk, Product product, TerminalCardData termCardData, string operatorCode, CustomerDetailsResult customer, Messaging Message, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage, out DecryptedFields decryptedFields);


        bool GenerateIBMPVV(int issuerId, Product product, DecryptedFields decryptedFields, string deviceId, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage, out string pvv);

        bool GenerateVisaPVV(int issuerId, Product product, DecryptedFields decryptedFields, string deviceId, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage, out string pvv);

        bool PinFromPinBlock(int issuerId, Product product, DecryptedFields decryptedFields, string deviceId, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage, out string pin);
    }    
}
