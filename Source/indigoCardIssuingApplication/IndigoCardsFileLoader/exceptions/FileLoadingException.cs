using System;

namespace IndigoFileLoader.exceptions
{
    [Serializable]
    public class FileLoadingException : Exception
    {
        public FileLoadingException(string message)
            : base(message)
        {
        }
    }
}