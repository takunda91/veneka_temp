using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.BackOffice.Application.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Org.BouncyCastle.Utilities.Encoders;

namespace Veneka.Indigo.BackOffice.Application.Crypto.Tests
{
    [TestClass()]
    public class WindowsCryptoTests
    {
        [TestMethod()]
        public void DoAesGCMTest()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            string nonSecretPayload = "?\u001aF?,?4?71Ax???8y&\\.L?????\u001e???I'i\a\u0011??\u001b?H\u0013??`??\u0006??mW?\u007f?V?|sgS?i???H??!??C\u0018fu???R#??y??|\u0012(?Y";
            string encryptThisStr = "CakeCat";
            string strkey = "xh ?? L + f ?#&v?:$??";

            byte[] key = new byte[16]; //128 bit key
            byte[] aData = new byte[90]; // aData

            rng.GetBytes(key);

            rng.GetBytes(aData);
            //string strkey1 = Base64.ToBase64String(key);

            //string straData = Base64.ToBase64String(aData);

            string strkey1 = "Aq6jCfgrp/CIUYUeD70M/w==";

            string straData = "l6G2luEj7DVmYM8jAtIMojwCgqjdbEeYm8kcZT / OS9ibHVM6UPuw1Ac5oLHDMZwQFiG8clin0kvtpxEdPQrhq8vL2jLsAEL2318hG0cKceyvIxcn8yKBrJ9m";
            WindowsCrypto winCrypt = new WindowsCrypto(strkey1.Replace(" ", ""), straData);



            //WindowsCrypto winCrypt = new WindowsCrypto(key, aData);

            ////byte[] _key = winCrypt.GenerateKey();

            //string strkey1 = System.Text.ASCIIEncoding.ASCII.GetString(key);

            //  byte[] _encrypt = winCrypt.Encrypt("9908090980912345");
            //string  strencrypt= Base64.ToBase64String(_encrypt);
            //string strencrypt = System.Text.ASCIIEncoding.ASCII.GetString(_encrypt);

            byte[] _encrypt = Base64.Decode("J/2/aPEBx04Li1jnS4xVqKBTJEzWb8IXSjaLFkIJwrNKr9nTZScGRAi9AMI=");

            string s = winCrypt.Decrypt(_encrypt);


        }
    }
}