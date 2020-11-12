using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.BackOffice.Application.Crypto
{
    public interface ICryptoManager
    {
        byte[] Encrypt(string value);
        string Decrypt(byte[] encodedvalue);
        
    }
}
