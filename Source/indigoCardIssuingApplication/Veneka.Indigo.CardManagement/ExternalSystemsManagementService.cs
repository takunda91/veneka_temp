using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.CardManagement.dal;
using Veneka.Indigo.CardManagement.objects;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Language;
using Veneka.Indigo.Common.Models;

namespace Veneka.Indigo.CardManagement
{
   public class ExternalSystemsManagementService
    {

       private readonly ExternalSystemsManagement cardDal = new ExternalSystemsManagement();
        private readonly ResponseTranslator _translator = new ResponseTranslator();

        public bool CreateExternalSystems(ExternalSystemFieldResult externalsystems, long auditUserId, string auditWorkstation, int languageId, out int? result, out string responseMessage)
       {
           var response = cardDal.CreateExternalSystems(externalsystems, auditUserId, auditWorkstation, out result);
           responseMessage = _translator.TranslateResponseCode(response, 0, languageId, auditUserId, auditWorkstation);

           if (response == SystemResponseCode.SUCCESS)
           {
               return true;
           }

           return false;
       }

        public bool UpdateExternalSystem(ExternalSystemFieldResult externalsystems, long auditUserId, string auditWorkstation, int languageId, out string responseMessage)
        {
            var response = cardDal.UpdateExternalSystem(externalsystems, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, 0, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public ExternalSystemFieldResult GetExternalSystems(int? externalsytemid, int rowindex, int rowsperpage, int languageId, long auditUserId, string auditWorkstation)
        {
            return cardDal.GetExternalSystems(externalsytemid, rowindex, rowsperpage, languageId, auditUserId, auditWorkstation);
        }

        public bool DeleteExternalSystems(int? external_system_id, long auditUserId, string auditWorkstation, int languageId, out string responseMessage)
        {
            var response = cardDal.DeleteExternalSystems(external_system_id, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, 0, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }


        public bool CreateExternalSystemFields(ExternalSystemFieldsResult externalsystemfields, long auditUserId, string auditWorkstation, int languageId, out int? result, out string responseMessage)
        {
            var response = cardDal.CreateExternalSystemFields(externalsystemfields, auditUserId, auditWorkstation, out result);
            responseMessage = _translator.TranslateResponseCode(response, 0, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public bool UpdateExternalSystemFields(ExternalSystemFieldsResult externalsystemfields, long auditUserId, string auditWorkstation, int languageId, out string responseMessage)
        {
            var response = cardDal.UpdateExternalSystemFields(externalsystemfields, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, 0, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public List<ExternalSystemFieldsResult> GetExternalSystemsFields(int? externalsytemfieldid, int rowindex, int rowsperpage, long auditUserId, string auditWorkstation)
        {
            return cardDal.GetExternalSystemsFields(externalsytemfieldid, rowindex, rowsperpage, auditUserId, auditWorkstation);
        }

        public bool DeleteExternalSystemField(int? external_system_field_id, long auditUserId, string auditWorkstation, int languageId, out string responseMessage)
        {
            var response = cardDal.DeleteExternalSystemField(external_system_field_id, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, 0, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public List<LangLookup> LangLookupExternalSystems(int languageId, long auditUserId, string auditWorkstation)
        {
            return cardDal.LangLookupExternalSystems( languageId, auditUserId, auditWorkstation);
        }

    }
}
