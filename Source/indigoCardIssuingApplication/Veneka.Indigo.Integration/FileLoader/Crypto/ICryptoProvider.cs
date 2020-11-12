using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.FileLoader.Crypto
{
    public interface ICryptoProvider
    {
        Stream DecryptFile(Stream inputStream, string key, string passPhrase);
        Stream EncryptFile(Stream inputStream, string key);
    }
}
