using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Veneka.Indigo.Integration.Common
{
    public class PseudoGenerator
    {
        public enum PseudoType { AlphaNumeric, Numeric, Hexidecimal };

        private static byte[] exclusiveOR(byte[] arr)
        {
            //if (arr1.Length != arr2.Length)
            //    throw new ArgumentException("arr1 and arr2 are not the same length");

            int half = arr.Length / 2;
            byte[] result = new byte[half];

            // Xor the first hald with the second half of the array
            for (int i = 0; i < half; ++i)
                result[i] = (byte)(arr[i] ^ arr[half + i]);

            return result;
        }

        /// <summary>
        /// Generate
        /// </summary>
        /// <param name="pseudoLength"></param>
        /// <param name="iterations"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static string[] Generate(int pseudoLength, int iterations, char[] chars)
        {
            string[] results = new string[iterations];

            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                for (int i = 0; i < iterations; i++)
                {
                    // create an array of bytes twice the size of the needed length
                    var rndData = new byte[pseudoLength * 2];

                    crypto.GetNonZeroBytes(rndData);

                    StringBuilder result = new StringBuilder(pseudoLength);
                    foreach (byte b in exclusiveOR(rndData))
                    {
                        result.Append(chars[b % (chars.Length)]);
                    }

                    results[i] = result.ToString();
                }
            }

            return results;
        }


        /// <summary>
        /// Generate
        /// </summary>
        /// <param name="pseudoLength"></param>
        /// <param name="iterations"></param>
        /// <param name="pseudoType"></param>
        /// <returns></returns>
        public static string[] Generate(int pseudoLength, int iterations, PseudoType pseudoType)
        {
            char[] chars;

            switch(pseudoType)
            {
                case PseudoType.AlphaNumeric: 
                    chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
                    break;
                case PseudoType.Numeric:
                    chars = "1234567890".ToCharArray();
                    break;
                case PseudoType.Hexidecimal:
                    chars = "ABCDEF1234567890".ToCharArray();
                    break;
                default:
                    chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
                    break;
            }


            return Generate(pseudoLength, iterations, chars);
        }

        /// <summary>
        /// Generate pseudo string and insert it from start count and keeping the last from end count.
        /// e.g. startIndex = 6 and endIndex = 4 will keep the first 6 and last 4 characters, inserting the pseudo string between them.
        /// </summary>
        /// <param name="pseudoLength"></param>
        /// <param name="pans"></param>
        /// <param name="startCount"></param>
        /// <param name="endCount"></param>
        /// <param name="pseudoType"></param>
        /// <returns></returns>
        public static string[] Generate(int pseudoLength, string[] pans, int startCount, int endCount, PseudoType pseudoType)
        {
            char[] chars;

            switch (pseudoType)
            {
                case PseudoType.AlphaNumeric:
                    chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
                    break;
                case PseudoType.Numeric:
                    chars = "1234567890".ToCharArray();
                    break;
                case PseudoType.Hexidecimal:
                    chars = "ABCDEF1234567890".ToCharArray();
                    break;
                default:
                    chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
                    break;
            }


            var pseudos =  Generate(pseudoLength, pans.Length, chars);

            var returnPans = new string[pans.Length];

            for(int i = 0; i < pans.Length; i++)
            {
                returnPans[i] = pans[i].Substring(0, startCount) + pseudos[i] + pans[i].Substring(pans[i].Length - endCount);
            }

            return returnPans;
        }
    }
}
