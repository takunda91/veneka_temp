using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Veneka.Indigo.Security
{
    public sealed class EncryptionManager
    {
        public static string EncryptString(string stringToEncrypt, bool useHasshing, string encryptionKey)
        {
            byte[] key = null;
            byte[] IV = new byte[16];

            using (System.Security.Cryptography.SHA256CryptoServiceProvider shaProvider
                           = new System.Security.Cryptography.SHA256CryptoServiceProvider())
            {
                key = shaProvider.ComputeHash(UTF8Encoding.UTF8.GetBytes(encryptionKey));
               // byte[] byteIV = shaProvider.ComputeHash(Encoding.ASCII.GetBytes(encryptionKey));//this produces a 256 bit hash so we need to split and corrupt the IV

                //byteIV.CopyTo(IV, 16);//take half the IV bytes
                Array.Copy(key, IV, 16);
            }


            byte[] result = null;

            using (System.Security.Cryptography.AesCryptoServiceProvider aesProvider =
               new System.Security.Cryptography.AesCryptoServiceProvider())
            {

                aesProvider.Key = key;
                aesProvider.IV = IV;
                aesProvider.Mode = System.Security.Cryptography.CipherMode.CBC;
                aesProvider.Padding = System.Security.Cryptography.PaddingMode.PKCS7;

                System.Security.Cryptography.ICryptoTransform cryptoTransformer =
                    aesProvider.CreateEncryptor();


                byte[] byteClear = UTF8Encoding.UTF8.GetBytes(stringToEncrypt);

                result = cryptoTransformer.TransformFinalBlock(byteClear,
                    0, byteClear.Length);


            }

            return Convert.ToBase64String(result);
        }

        public static string DecryptString(string stringToDecrypt, bool useHashing, string decryptionKey)
        {
            byte[] key = null;
            byte[] IV = new byte[16];

            using (System.Security.Cryptography.SHA256CryptoServiceProvider shaProvider
                           = new System.Security.Cryptography.SHA256CryptoServiceProvider())
            {
                key = shaProvider.ComputeHash(UTF8Encoding.UTF8.GetBytes(decryptionKey));
                //byte[] byteIV = shaProvider.ComputeHash(Encoding.ASCII.GetBytes(decryptionKey));//this produces a 256 bit hash so we need to split and corrupt the IV

                Array.Copy(key, IV, 16);
            }


            byte[] result = null;

            using (System.Security.Cryptography.AesCryptoServiceProvider aesProvider =
               new System.Security.Cryptography.AesCryptoServiceProvider())
            {

                aesProvider.Key = key;
                aesProvider.IV = IV;
                aesProvider.Mode = System.Security.Cryptography.CipherMode.CBC;
                aesProvider.Padding = System.Security.Cryptography.PaddingMode.PKCS7;

                System.Security.Cryptography.ICryptoTransform cryptoTransformer =
                    aesProvider.CreateDecryptor();
                stringToDecrypt= stringToDecrypt.Replace('-','+');
                if (!string.IsNullOrEmpty(stringToDecrypt))
                {

                    byte[] byteClear = Convert.FromBase64String(stringToDecrypt);

                    result = cryptoTransformer.TransformFinalBlock(byteClear,
                        0, byteClear.Length);

                }// end if (!string.IsNullOrEmpty(stringToDecrypt))

            }

            return null == result ? string.Empty
                        : UTF8Encoding.UTF8.GetString(result);
        }

        /// <summary>
        /// More advanced Encryption method, should rather use this for encryption and decryption.
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <param name="encryptionKey"></param>
        /// <returns></returns>
        public static byte[] EncryptData(byte[] toEncrypt, string encryptionKey)
        {
            byte[] finalResult = null;
            if (toEncrypt == null || toEncrypt.Length == 0) throw new ArgumentNullException("toEncrypt is null or empty.");
            if (string.IsNullOrEmpty(encryptionKey)) throw new ArgumentNullException("encryptionKey is null or empty.");
            
            using (var provider = new AesCryptoServiceProvider())
            {
                using (HashAlgorithm hasher = new SHA256CryptoServiceProvider())
                {
                    provider.Key = hasher.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));
                }

                provider.Mode = CipherMode.CBC;
                provider.Padding = PaddingMode.PKCS7;
                using (var encryptor = provider.CreateEncryptor(provider.Key, provider.IV))
                {
                    var result = encryptor.TransformFinalBlock(toEncrypt, 0, toEncrypt.Length);

                    finalResult = new byte[provider.IV.Length + result.Length];

                    Buffer.BlockCopy(provider.IV, 0, finalResult, 0, provider.IV.Length);
                    Buffer.BlockCopy(result, 0, finalResult, provider.IV.Length, result.Length);                    
                }
            }

            return finalResult;
        }

        /// <summary>
        /// More advanced Encryption method, should rather use this for encryption and decryption.
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <param name="encryptionKey"></param>
        /// <returns></returns>
        public static byte[] EncryptData(byte[] toEncrypt, byte[] encryptionKey)
        {
            byte[] finalResult = null;
            if (toEncrypt == null || toEncrypt.Length == 0) throw new ArgumentNullException("toEncrypt is null or empty.");
            if (encryptionKey == null || encryptionKey.Length == 0) throw new ArgumentNullException("encryptionKey is null or empty.");

            using (var provider = new AesCryptoServiceProvider())
            {
                using (HashAlgorithm hasher = new SHA256CryptoServiceProvider())
                {
                    provider.Key = hasher.ComputeHash(encryptionKey);
                }

                provider.Mode = CipherMode.CBC;
                provider.Padding = PaddingMode.PKCS7;
                using (var encryptor = provider.CreateEncryptor(provider.Key, provider.IV))
                {
                    var result = encryptor.TransformFinalBlock(toEncrypt, 0, toEncrypt.Length);

                    finalResult = new byte[provider.IV.Length + result.Length];

                    Buffer.BlockCopy(provider.IV, 0, finalResult, 0, provider.IV.Length);
                    Buffer.BlockCopy(result, 0, finalResult, provider.IV.Length, result.Length);
                }
            }

            return finalResult;
        }

        public static byte[] EncryptDataFromString(string toEncrypt, string encryptionKey)
        {
            if (String.IsNullOrWhiteSpace(toEncrypt)) throw new ArgumentNullException("toEncrypt cannot be null or empty");

            return EncryptData(Encoding.UTF8.GetBytes(toEncrypt), encryptionKey);
        }

        public static byte[] DecryptData(byte[] encryptedData, string encryptionKey)
        {
            byte[] result = null;

            if (encryptedData == null || encryptedData.Length <= 16) throw new ArgumentNullException("encryptedData cannot be null or empty");
            if (String.IsNullOrWhiteSpace(encryptionKey)) throw new ArgumentNullException("encryptionKey cannot be null or empty.");

            using (var provider = new AesCryptoServiceProvider())
            {
                using (HashAlgorithm hasher = new SHA256CryptoServiceProvider())
                {
                    provider.Key = hasher.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));
                }

                provider.Mode = CipherMode.CBC;
                provider.Padding = PaddingMode.PKCS7;

                byte[] iv = new byte[16];

                Buffer.BlockCopy(encryptedData, 0, iv, 0, 16);

                provider.IV = iv;

                using (var decryptor = provider.CreateDecryptor())
                {
                    result = decryptor.TransformFinalBlock(encryptedData, 16, encryptedData.Length - 16);
                }                
            }

            return result;
        }

        public static byte[] DecryptData(byte[] encryptedData, byte[] encryptionKey)
        {
            byte[] result = null;

            if (encryptedData == null || encryptedData.Length <= 16) throw new ArgumentNullException("encryptedData cannot be null or empty");
            if (encryptionKey == null || encryptionKey.Length == 0) throw new ArgumentNullException("encryptionKey cannot be null or empty.");

            using (var provider = new AesCryptoServiceProvider())
            {
                using (HashAlgorithm hasher = new SHA256CryptoServiceProvider())
                {
                    provider.Key = hasher.ComputeHash(encryptionKey);
                }

                provider.Mode = CipherMode.CBC;
                provider.Padding = PaddingMode.PKCS7;

                byte[] iv = new byte[16];

                Buffer.BlockCopy(encryptedData, 0, iv, 0, 16);

                provider.IV = iv;

                using (var decryptor = provider.CreateDecryptor())
                {
                    result = decryptor.TransformFinalBlock(encryptedData, 16, encryptedData.Length - 16);
                }
            }

            return result;
        }

        public static string DecryptDataToString(byte[] encryptedData, string encryptionKey)
        {
            var result = DecryptData(encryptedData, encryptionKey);

            if (result != null)
                return Encoding.UTF8.GetString(result);

            return null;
        }
    }
}