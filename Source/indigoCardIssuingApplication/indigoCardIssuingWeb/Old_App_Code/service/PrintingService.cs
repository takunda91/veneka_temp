using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Security.Principal;

namespace indigoCardIssuingWeb.Old_App_Code.service
{
    public class PrintingService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PrintingService));
        //private static readonly Service1SoapClient _issuanceService = new Service1SoapClient();
        private readonly SHA256CryptoServiceProvider sha256;

        private static volatile PrintingService instance;
        private static object syncRoot = new Object();

        public static PrintingService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new PrintingService();
                    }
                }

                return instance;
            }
        }

        private PrintingService()
        {
            sha256 = new SHA256CryptoServiceProvider();
        }

        /// <summary>
        /// Caches the html and returns token to use.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="html"></param>
        /// <returns></returns>
        internal string SetCardPrinting(string key, string html)
        {
            string hashKey = GetHash(key);
            if (CacheLayer.Exists(hashKey))
                CacheLayer.Clear(hashKey);
            CacheLayer.Add<string>(html, hashKey);

            return hashKey;
        }

        internal string GetCardPrinting(string key)
        {
            return CacheLayer.Get<string>(key);
        }
        internal void Clear(string key)
        {
            CacheLayer.Clear(key);
        }
        private string GetHash(string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

    }

    public class CacheLayer
    {
        static readonly ObjectCache Cache = MemoryCache.Default;
        private static CacheItemPolicy policy = null;
        private static CacheEntryRemovedCallback callback = null;
        private static readonly ILog log = LogManager.GetLogger(typeof(CacheLayer));

        /// <summary>
        /// Retrieve cached item
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Name of cached item</param>
        /// <returns>Cached item as type</returns>
        public static T Get<T>(string key) where T : class
        {
            try
            {
                return (T)Cache[key];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Insert value into the cache using
        /// appropriate name/value pairs
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="objectToCache">Item to be cached</param>
        /// <param name="key">Name of item</param>
        public static void Add<T>(T objectToCache, string key) where T : class
        {

            policy = new CacheItemPolicy();
            policy.Priority = CacheItemPriority.Default;
            policy.AbsoluteExpiration = System.DateTimeOffset.Now.AddMinutes(30.00);
            policy.RemovedCallback = MyCachedItemRemovedCallback;

            Cache.Add(key, objectToCache, policy);
        }
        private static void MyCachedItemRemovedCallback(CacheEntryRemovedArguments arguments)
        {
            // Log these values from arguments list 
            log.Debug(
                 string.Format(
                     "Cache Item Evicted (reason: {0}) - Key: {1}, Value: {2}",
                     arguments.RemovedReason,
                     arguments.CacheItem.Key,
                     arguments.CacheItem.Value));

        }


        /// <summary>
        /// Insert value into the cache using
        /// appropriate name/value pairs
        /// </summary>
        /// <param name="objectToCache">Item to be cached</param>
        /// <param name="key">Name of item</param>
        public static void Add(object objectToCache, string key)
        {
            Cache.Add(key, objectToCache, DateTime.Now.AddMinutes(30));
        }

        /// <summary>
        /// Remove item from cache
        /// </summary>
        /// <param name="key">Name of cached item</param>
        public static void Clear(string key)
        {
            Cache.Remove(key);
        }

        public static void SetValue(string key, string html)
        {
            Cache[key] = html;
        }
       

        /// <summary>
        /// Check for item in cache
        /// </summary>
        /// <param name="key">Name of cached item</param>
        /// <returns></returns>
        public static bool Exists(string key)
        {
            return Cache.Get(key) != null;
        }


        /// <summary>
        /// Gets all cached items as a list by their key.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAll()
        {
            return Cache.Select(keyValuePair => keyValuePair.Key).ToList();
        }
    }
}