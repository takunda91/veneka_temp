using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.Cryptography;

namespace Veneka.Indigo.Integration.FileLoader.Crypto
{
    public enum FileEncryptionType { PGP = 1 };

    public class FileDecryptionFactory
    {
        public static ICryptoProvider CreateCryptoProvider(FileEncryptionType fileEncryptionType)
        {
            switch (fileEncryptionType)
            {
                case FileEncryptionType.PGP: return new PGP();
                default: return null;
            }
        }
    }
}
