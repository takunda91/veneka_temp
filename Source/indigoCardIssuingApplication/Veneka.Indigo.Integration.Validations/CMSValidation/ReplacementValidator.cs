using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Integration.Validations.CMSValidation
{
    public class ReplacementValidator : ICardValidator
    {
        private readonly ILog _cmsLog = LogManager.GetLogger(General.CMS_LOGGER);

        public bool Validate(CMSCard cmsCard)
        {
            _cmsLog.Debug(d => d("Validate Card Status 2 = " + cmsCard.CardStatus ));
            if (cmsCard.CardStatus.Trim() == "2")
                //cmsCard.OtherFields[Helper.STATUS2_NAME] == "2" && //04 AUG 2016 - Rex says not to check this
                //CheckStopCause(cmsCard.StopCause))
            {
                return true;
            }

            return false;
        }


        private bool CheckStopCause(string stopCause)
        {
            switch (stopCause.Trim())
            {
                case "1": return true;
                case "2": return true;
                case "A": return true;
                case "B": return true;
                case "D": return true;
                case "F": return true;
                default: return false;
            }
        }
    }
}
