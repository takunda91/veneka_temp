using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Integration.TMB.MD5Hash
{
    public sealed class MD5Hash
    {
        public static string EncryptPassword(string password)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] passwordBytes = System.Text.Encoding.ASCII.GetBytes(password);

            byte[] hash = md5.ComputeHash(passwordBytes);

            // step 2, convert byte array to hex string

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)

            {
               sb.Append(hash[i].ToString("x2"));

            }

            return sb.ToString();
        }

    }
}
