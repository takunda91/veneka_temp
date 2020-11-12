using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Common.Utilities
{
    public sealed class StaticFields
    {
        #region security
        public const string INTERNAL_SECURITY_KEY = "7QywOYlerHsirr8xx9Z9Mbyw4xH3ntyZ8FpnPJYdypmXmvf8xO79Q6T55TidNAUA"; //"INDIGOVENEKA";
        public const string EXTERNAL_SECURITY_KEY = "F1A234CB58B929BA0237200DBC9E6DC4";
        public const string PIN_SECURITY_KEY = "CE3188FB9DDA8F628F42EE99C38F95A6";
        public static readonly string START_OF_SESSION_KEY = "000XXXHHH";
        public const string DB_SECURITY_KEY = "VENEKAINDIGO";// "Xcj2GfPEyxZG5H9KNAGXqSWT2NeDDjtbrYwCzZn1961NWi2MKYakVEepH8FpP5ue"; //"VENEKAINDIGO";
        public const bool USE_HASHING_FOR_ENCRYPTION = true;
        #endregion

        public const string DATETIME_FORMAT = "dd/MM/yyyy HH:mm:ss";
        public const string DATE_FORMAT = "dd/MM/yyyy";
    }
}
