using System;
using System.Collections.Generic;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.CardIssuanceService;

namespace indigoCardIssuingWeb.CCO
{
    public sealed class General
    {
        public static string MaskCardNumber(string cardNumber)
        {
            return UtilityClass.DisplayPartialPAN(cardNumber, '*');
        }
    }
}