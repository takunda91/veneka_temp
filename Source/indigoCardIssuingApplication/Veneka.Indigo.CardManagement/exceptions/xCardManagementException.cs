using System;

namespace Veneka.Indigo.CardManagement.exceptions
{
    [Serializable]
    public class xCardManagementException : Exception
    {
        public xCardManagementException(string message)
            : base(message)
        {
        }
    }
}