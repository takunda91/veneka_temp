using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.Integration.Cryptography;
using System.IO;

namespace IndigoAppTesting.Veneka.Indigo.Integration
{
    [TestClass]
    public class Crypto
    {
        [Ignore]
        [TestMethod]
        public void PGPDecryptTest()
        {
            PGP pgp = new PGP();
            var input = File.OpenRead(@"c:\veneka\pgp\B0021002_484_178.OUT.gpg");
            var key = File.ReadAllText(@"c:\veneka\pgp\PrivateKey.txt");

            var file = pgp.DecryptFile(input, key, "v3n3ka!");

            using (var fileStream = File.Create(@"c:\veneka\pgp\decrypted"))
            {
                //file.Seek(0, SeekOrigin.Begin);
                file.CopyTo(fileStream);

            }
        }

        [Ignore]
        [TestMethod]
        public void PGPEncryptTest()
        {
            PGP pgp = new PGP();

            using (var fileStream = File.Create(@"C:\veneka\pgp\Sample_enc.pgp"))
            {
                using (var strm = pgp.EncryptFile(File.OpenRead(@"C:\veneka\pgp\Sample.txt"), File.ReadAllText(@"C:\veneka\pgp\bk\PublicKey.txt")))
                {
                    strm.CopyTo(fileStream);
                }
            }
        }
    }
}
