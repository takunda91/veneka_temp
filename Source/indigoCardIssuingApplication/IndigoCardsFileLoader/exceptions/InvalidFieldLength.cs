using System;

namespace IndigoFileLoader.exceptions
{
    [Serializable]
    public class InvalidFieldLength : Exception
    {
        public InvalidFieldLength(string message)
            : base(message)
        {
        }
    }
}