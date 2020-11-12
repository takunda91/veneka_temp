using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Veneka.Indigo.MachineKeyGeneration
{
    class Program
    {
        static void Main(string[] args)
        {
            int len = 128;

            while (len != 0)
            {
                Console.WriteLine("Key Length (0 to exit):");
                len = int.Parse(Console.ReadLine());

                byte[] buff = new byte[len / 2];
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(buff);
                StringBuilder sb = new StringBuilder(len);
                for (int i = 0; i < buff.Length; i++)
                    sb.Append(string.Format("{0:X2}", buff[i]));
                Console.WriteLine(sb);
            }
        }
    }
}
