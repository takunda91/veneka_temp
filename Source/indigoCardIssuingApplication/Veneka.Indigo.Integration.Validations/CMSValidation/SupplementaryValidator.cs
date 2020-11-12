using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Integration.Validations.CMSValidation
{
    public class SupplementaryValidator : ICardValidator
    {
        public bool Validate(CMSCard cmsCard)
        {
            if ((cmsCard.CardStatus == "0" || cmsCard.CardStatus == "1" || cmsCard.CardStatus == "2") &&
                 cmsCard.IsBaseCard.Value)
            {
                return true;
            }

            return false;
        }
    }
}
