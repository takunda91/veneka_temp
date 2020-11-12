using System;

namespace IndigoFileLoader.exceptions
{
    [Serializable]
    public class UnExpectedNumberOfRecords : Exception
    {
        public UnExpectedNumberOfRecords(string message)
            : base(message)
        {
        }
    }
}