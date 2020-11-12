using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Fips;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;
using System.IO;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Crypto.Utilities;

namespace Veneka.Indigo.BackOffice.Application.Crypto
{
    public class WindowsCrypto : ICryptoManager
    {
        // https://proandroiddev.com/security-best-practices-symmetric-encryption-with-aes-in-java-7616beaaade9

        private readonly int _ivByteSize = 12; // NIST recommends 12 bytes not 16;
        private readonly int _macBitSize = 128; // Must be >= 128 bits
        
        private readonly byte[] _key = null; // encrypted key   
        private readonly byte[] _aData; // associated data
        private SecureRandom _fipsRandom; // fips compiant random number generator        
        FipsAes.Key _fipsKey;
        public WindowsCrypto(string strkey,string strData)
        {
            //ProtectedMemory.Protect(key, MemoryProtectionScope.SameProcess);
            //ProtectedMemory.Protect(aData, MemoryProtectionScope.SameProcess);
            byte[] key, Data;
            key = Org.BouncyCastle.Utilities.Encoders.Base64.Decode(strkey);
            Data = Org.BouncyCastle.Utilities.Encoders.Base64.Decode(strData);
            _key = key;
            _aData = Data;

            // get the crypto service started
            CryptoStatus.IsReady();
            CryptoServicesRegistrar.SetApprovedOnlyMode(true); // make sure we only use FIPS complaint stuff

            // setup the fips complaint RNG - NIST SP 800-90A
            string personalizationString = DateTime.UtcNow.ToString();
            byte[] additionalInput = new byte[24];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] persByte = new byte[12];
                rng.GetNonZeroBytes(persByte);
                rng.GetNonZeroBytes(additionalInput);

                personalizationString += Encoding.UTF8.GetString(persByte);
            }

            // setup the fips complaint RNG - NIST SP 800-90A
            SecureRandom entropySource = new SecureRandom();
            _fipsRandom = CryptoServicesRegistrar.CreateService(FipsDrbg.Sha512_256HMac)
                                    .FromEntropySource(entropySource, true)
                                    .SetPersonalizationString(Encoding.UTF8.GetBytes(personalizationString))
                                    .Build(entropySource.GenerateSeed((256 / (2 * 8))), true, additionalInput);

            _fipsKey = new FipsAes.Key(key);
            // generate a 128 bit AES key
            //_fipsKey = CryptoServicesRegistrar.CreateGenerator(FipsAes.KeyGen128, _fipsRandom).GenerateKey();
        }
        
       
        public string Decrypt(byte[] encodedVal)
        {
            byte[] iv = new byte[_ivByteSize];

            Buffer.BlockCopy(encodedVal, 0, iv, 0, iv.Length);

            IAeadCipherService provider = CryptoServicesRegistrar.CreateService(_fipsKey);
            IAeadCipherBuilder<IParameters<FipsAlgorithm>> aeadDecryptorBldr = provider.CreateAeadDecryptorBuilder(FipsAes.Gcm.WithIV(iv).WithMacSize(_macBitSize));

            MemoryOutputStream bOut = new MemoryOutputStream();

            // build the encryptor
            IAeadCipher encryptor = aeadDecryptorBldr.BuildAeadCipher(AeadUsage.AAD_FIRST, bOut);

            // write out the associated data
            encryptor.AadStream.Write(_aData, 0, _aData.Length);

            // write out the plain text
            using (Stream encOut = encryptor.Stream)
            {
                encOut.Write(encodedVal, iv.Length, encodedVal.Length - iv.Length);
            }

            byte[] output = bOut.ToArray(); // decrypted value
            string value = Encoding.UTF8.GetString(output);




            return value;
        }

        public byte[] Encrypt(string value)
        {

            // plain text data
            //byte[] data = Hex.Decode("I really hope that this works!!!");
            byte[] data = Encoding.UTF8.GetBytes(value);

            IAeadCipherService provider = CryptoServicesRegistrar.CreateService(_fipsKey);

            byte[] iv = new byte[_ivByteSize];
            _fipsRandom.NextBytes(iv);

            // create a builder for the AEAD cipher with a tag size of 128 bits.
            IAeadCipherBuilder<IParameters<FipsAlgorithm>> aeadEncryptorBldr = provider.CreateAeadEncryptorBuilder(FipsAes.Gcm.WithIV(iv).WithMacSize(_macBitSize));

            MemoryOutputStream bOut = new MemoryOutputStream();

            // build the encryptor
            IAeadCipher encryptor = aeadEncryptorBldr.BuildAeadCipher(AeadUsage.AAD_FIRST, bOut);

            // write out the associated data
            encryptor.AadStream.Write(_aData, 0, _aData.Length);

            // write out the plain text
            using (Stream encOut = encryptor.Stream)
            {
                encOut.Write(data, 0, data.Length);
            }

            var encryptedBytes = bOut.ToArray(); // encrypted output with in-line tag

            byte[] output = new byte[iv.Length + encryptedBytes.Length];
            Buffer.BlockCopy(iv, 0, output, 0, iv.Length);
            Buffer.BlockCopy(encryptedBytes, 0, output, iv.Length, encryptedBytes.Length);

            byte[] encMac = encryptor.GetMac().Collect(); // tag value by itself

            return output;
        }

        public byte[] DoAesGCM()
        {
            return null;
        }

        public byte[] UndoAesGCM(byte[] encodedVal)
        {
            return null;
        }
    }
}
