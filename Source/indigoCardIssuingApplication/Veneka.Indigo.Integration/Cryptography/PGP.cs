using System;
using System.IO;
using System.Security.Cryptography;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Text;

namespace Veneka.Indigo.Integration.Cryptography
{
    /// <summary>
    /// PGP implementation that uses Bouncy Castle.
    /// </summary>
    public class PGP : FileLoader.Crypto.ICryptoProvider
    {
        /// <summary>
        /// Decrypt PGP file (ICryptoProvider).
        /// </summary>
        /// <param name="inputStream">Stream of encrypted text.</param>
        /// <param name="key">Private key used to decrypt the file (UTF-8).</param>
        /// <param name="passPhrase">Password/Passphrase which was used to encrypt the private key.</param>
        /// <returns>Unencrypted text stream.</returns>
        public Stream DecryptFile(Stream inputStream, string key, string passPhrase)
        {
            using (var keyStream = new MemoryStream(Encoding.UTF8.GetBytes(key)))
                return this.DecryptPgpData(inputStream, keyStream, passPhrase);
        }

        /// <summary>
        /// Decrypt stream using PGP.
        /// </summary>
        /// <param name="inputStream">Stream of encrypted text.</param>
        /// <param name="privateKeyStream">Stream of Private key used to decrypt the file.</param>
        /// <param name="passPhrase">Password/Passphrase which was used to encrypt the private key.</param>
        /// <returns>Unencrypted text stream.</returns>
        public Stream DecryptPgpData(Stream inputStream, Stream privateKeyStream, string passPhrase)
        {
            PgpObjectFactory pgpFactory = new PgpObjectFactory(PgpUtilities.GetDecoderStream(inputStream));
            // find secret key
            PgpSecretKeyRingBundle pgpKeyRing = new PgpSecretKeyRingBundle(PgpUtilities.GetDecoderStream(privateKeyStream));

            PgpObject pgp = null;
            if (pgpFactory != null)
            {
                pgp = pgpFactory.NextPgpObject();
            }

            // the first object might be a PGP marker packet.
            PgpEncryptedDataList encryptedData = null;
            if (pgp is PgpEncryptedDataList)
            {
                encryptedData = (PgpEncryptedDataList)pgp;
            }
            else
            {
                encryptedData = (PgpEncryptedDataList)pgpFactory.NextPgpObject();
            }

            // decrypt
            PgpPrivateKey privateKey = null;
            PgpPublicKeyEncryptedData pubKeyData = null;
            foreach (PgpPublicKeyEncryptedData pubKeyDataItem in encryptedData.GetEncryptedDataObjects())
            {
                privateKey = FindSecretKey(pgpKeyRing, pubKeyDataItem.KeyId, passPhrase.ToCharArray());

                if (privateKey != null)
                {
                    pubKeyData = pubKeyDataItem;
                    break;
                }
            }

            if (privateKey == null)
            {
                throw new ArgumentException("Secret key for message not found.");
            }

            PgpObjectFactory plainFact = null;
            using (Stream clear = pubKeyData.GetDataStream(privateKey))
            {
                plainFact = new PgpObjectFactory(clear);
            }

            PgpObject message = plainFact.NextPgpObject();

            if (message is PgpCompressedData)
            {
                PgpCompressedData compressedData = (PgpCompressedData)message;
                PgpObjectFactory pgpCompressedFactory = null;

                using (Stream compDataIn = compressedData.GetDataStream())
                {
                    pgpCompressedFactory = new PgpObjectFactory(compDataIn);
                }

                message = pgpCompressedFactory.NextPgpObject();
                
                if (message is PgpOnePassSignatureList)
                {
                    message = pgpCompressedFactory.NextPgpObject();
                }

                return ((PgpLiteralData)message).GetInputStream();
            }
            else if (message is PgpLiteralData)
            {
                return ((PgpLiteralData)message).GetInputStream();
            }
            else if (message is PgpOnePassSignatureList)
            {
                throw new PgpException("Encrypted message contains a signed message - not literal data.");
            }
            else
            {
                throw new PgpException("Message is not a simple encrypted file - type unknown.");
            }
        }


        public Stream EncryptFile(Stream inputStream, string publicKey)
        {
            using (var keyStream = new MemoryStream(Encoding.UTF8.GetBytes(publicKey)))
                return EncryptPgpStream(inputStream, keyStream, true, true);
        }

        public byte[] Encrypt(byte[] inputData, PgpPublicKey passPhrase, bool withIntegrityCheck, bool armor)
        {
            byte[] processedData = Compress(inputData, PgpLiteralData.Console, CompressionAlgorithmTag.Uncompressed);

            MemoryStream bOut = new MemoryStream();
            Stream output = bOut;

            if (armor)
                output = new ArmoredOutputStream(output);

            PgpEncryptedDataGenerator encGen = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Cast5, withIntegrityCheck, new SecureRandom());
            encGen.AddMethod(passPhrase);

            Stream encOut = encGen.Open(output, processedData.Length);

            encOut.Write(processedData, 0, processedData.Length);
            encOut.Close();

            if (armor)
                output.Close();

            return bOut.ToArray();
        }


        #region Private Methods        
        private PgpPrivateKey FindSecretKey(PgpSecretKeyRingBundle pgpSec, long keyId, char[] pass)
        {
            PgpSecretKey pgpSecKey = pgpSec.GetSecretKey(keyId);
            if (pgpSecKey == null)
            {
                return null;
            }

            return pgpSecKey.ExtractPrivateKey(pass);
        }

        private PgpPublicKey ReadPublicKey(Stream inputStream)
        {
            inputStream = PgpUtilities.GetDecoderStream(inputStream);
            PgpPublicKeyRingBundle pgpPub = new PgpPublicKeyRingBundle(inputStream);

            foreach (PgpPublicKeyRing keyRing in pgpPub.GetKeyRings())
            {
                foreach (PgpPublicKey key in keyRing.GetPublicKeys())
                {
                    if (key.IsEncryptionKey)
                    {
                        return key;
                    }
                }
            }

            throw new ArgumentException("Can't find encryption key in key ring.");
        }

        private Stream EncryptPgpStream(Stream inputStream, Stream publicKeyStream, bool armor, bool withIntegrityCheck)
        {
            PgpPublicKey pubKey = ReadPublicKey(publicKeyStream);

            byte[] processedData;
            using (MemoryStream mStream = new MemoryStream())
            {
                inputStream.Position = 0;
                inputStream.CopyTo(mStream);
                mStream.Position = 0;
                processedData = mStream.ToArray();
            }

            return new MemoryStream(Encrypt(processedData, pubKey, withIntegrityCheck, armor));
        }

        private byte[] Compress(byte[] clearData, string fileName, CompressionAlgorithmTag algorithm)
        {
            using (MemoryStream bOut = new MemoryStream())
            {
                PgpCompressedDataGenerator comData = new PgpCompressedDataGenerator(algorithm);
                using (Stream cos = comData.Open(bOut)) // open it with the final destination
                {
                    PgpLiteralDataGenerator lData = new PgpLiteralDataGenerator();

                    // we want to Generate compressed data. This might be a user option later,
                    // in which case we would pass in bOut.
                    Stream pOut = lData.Open(
                        cos,                    // the compressed output stream
                        PgpLiteralData.Binary,
                        fileName,               // "filename" to store
                        clearData.Length,       // length of clear data
                        DateTime.UtcNow         // current time
                    );

                    pOut.Write(clearData, 0, clearData.Length);
                    pOut.Close();

                    comData.Close();

                    return bOut.ToArray();
                }
            }
        }
        #endregion

        #region Not Used
        //// Note: I was able to extract the private key into xml format .Net expecs with this
        //public string GetPrivateKeyXml(string inputData)
        //{
        //    Stream inputStream = IoHelper.GetStream(inputData);
        //    PgpObjectFactory pgpFactory = new PgpObjectFactory(PgpUtilities.GetDecoderStream(inputStream));
        //    PgpObject pgp = null;
        //    if (pgpFactory != null)
        //    {
        //        pgp = pgpFactory.NextPgpObject();
        //    }

        //    PgpEncryptedDataList encryptedData = null;
        //    if (pgp is PgpEncryptedDataList)
        //    {
        //        encryptedData = (PgpEncryptedDataList)pgp;
        //    }
        //    else
        //    {
        //        encryptedData = (PgpEncryptedDataList)pgpFactory.NextPgpObject();
        //    }

        //    Stream privateKeyStream = File.OpenRead(PrivateKeyOnlyPath);

        //    // find secret key
        //    PgpSecretKeyRingBundle pgpKeyRing = new PgpSecretKeyRingBundle(PgpUtilities.GetDecoderStream(privateKeyStream));
        //    PgpPrivateKey privateKey = null;

        //    foreach (PgpPublicKeyEncryptedData pked in encryptedData.GetEncryptedDataObjects())
        //    {
        //        privateKey = FindSecretKey(pgpKeyRing, pked.KeyId, Password.ToCharArray());
        //        if (privateKey != null)
        //        {
        //            //pubKeyData = pked;
        //            break;
        //        }
        //    }

        //    // get xml:
        //    RsaPrivateCrtKeyParameters rpckp = ((RsaPrivateCrtKeyParameters)privateKey.Key);
        //    RSAParameters dotNetParams = DotNetUtilities.ToRSAParameters(rpckp);
        //    RSA rsa = RSA.Create();
        //    rsa.ImportParameters(dotNetParams);
        //    string xmlPrivate = rsa.ToXmlString(true);

        //    return xmlPrivate;
        //}
        #endregion
    }
}
