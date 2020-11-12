using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.Validations.CMSValidation
{
    public class ValidationFactory
    {
        public static ICardValidator Validator(int issuingScenario)
        {
            switch(issuingScenario)
            {
                case 2: return new RenewalValidator();
                case 3: return new ReplacementValidator();
                case 4: return new SupplementaryValidator();
                default: throw new ArgumentException("No validator for issuing scenario: " + issuingScenario);
            }
        }
    }
}
