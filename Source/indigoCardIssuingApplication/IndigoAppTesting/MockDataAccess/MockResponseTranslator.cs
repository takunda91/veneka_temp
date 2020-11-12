using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Language;

namespace IndigoAppTesting.MockDataAccess
{
    public class MockResponseTranslator : IResponseTranslator
    {
        public string TranslateResponseCode(SystemResponseCode responseCode, SystemArea systemArea, int language, long auditUserId, string auditWorkstation)
        {
            if(responseCode == SystemResponseCode.SUCCESS)
            {
                return "";
            }
            else
            {
                return "Somethinf bad happened";
            }
        }
    }
}
