using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Integration.Validations.CMSValidation
{
    public interface ICardValidator
    {
        bool Validate(CMSCard cmsCard);
    }
}
