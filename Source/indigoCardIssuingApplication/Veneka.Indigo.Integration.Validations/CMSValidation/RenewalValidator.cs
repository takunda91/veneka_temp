using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Integration.Validations.CMSValidation
{
    public class RenewalValidator : ICardValidator
    {
        private readonly ILog _cmsLog = LogManager.GetLogger(General.CMS_LOGGER);

        public bool Validate(CMSCard cmsCard)
        {
            _cmsLog.Debug(d => d("Validate Card Status 0 or 1 = " + cmsCard.CardStatus));

            if ((cmsCard.CardStatus.Trim() == "0" || cmsCard.CardStatus.Trim() == "1") &&
                //(cmsCard.OtherFields[Helper.STATUS2_NAME] == "0" || cmsCard.OtherFields[Helper.STATUS2_NAME] == "1" || cmsCard.OtherFields[Helper.STATUS2_NAME] == "2") &&
                //cmsCard.StopCause.Trim().Equals("0") &&
                IsCardAboutToExpire(cmsCard.ExpiryDate.Value))
            {
                return true;
            }

            return false;
        }

        private bool IsCardAboutToExpire(DateTime expiryDate)
        {            
            TimeSpan difference = expiryDate - DateTime.Now;

            _cmsLog.Debug(d => d("Expiry: " + expiryDate.ToShortDateString() + " vs " + difference.ToString()));

            //TODO: Confirm if we should use days or months
            //Can renew 60 days before expiry and any time after.
            if (Math.Ceiling(difference.TotalDays) <= 60)
                return true;

            return false;
        }
    }
}
