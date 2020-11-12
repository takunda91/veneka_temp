using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace Veneka.Indigo.Security
{
    /// <summary>
    /// Provides secure memory cache to store values
    /// </summary>
    public class SecureCache
    {
        private static readonly RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        private readonly ObjectCache _secureSessionCache;
        private readonly TimeSpan _expiry;

        //128 bit encryption
        private readonly byte[] cacheKey = new byte[16];
        
        /// <summary>
        /// New secure cache instance, please note the cacheName must be unique.
        /// </summary>
        /// <param name="cacheName">Name of the cache store.</param>
        /// <param name="expiry">How long the cache item will be stored for.</param>
        public SecureCache(string cacheName, TimeSpan expiry)
        {
            if (String.IsNullOrWhiteSpace(cacheName)) throw new ArgumentNullException("cacheName", "Cannot be null or empty.");
            if(expiry == null) throw new ArgumentNullException("expiry", "Cannot be null.");

            _secureSessionCache = new MemoryCache(cacheName);
            _expiry = expiry;

            //Generate a Random key to encrypt values in memory cache            
            rng.GetBytes(cacheKey);
        }

        /// <summary>
        /// Set and item in the secure cache. Please note, if and item already exists in the cache with the same key it will be overwritten.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>
        public void SetCacheItem(string key, byte[] item)
        {
            if (String.IsNullOrWhiteSpace(key)) throw new ArgumentNullException("key", "Cannot be null or empty.");
            if (item == null || item.Length == 0) throw new ArgumentNullException("item", "Cannot be null or empty.");

            if (_secureSessionCache.Contains(key))
                _secureSessionCache.Remove(key);

            var encryptedItem = EncryptionManager.EncryptData(item, cacheKey);

            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now.Add(_expiry);

            _secureSessionCache.Add(key, encryptedItem, policy);
        }

        public void SetCacheItem<C>(string key, C item) where C : class
        {
            if (item == null) throw new ArgumentNullException("item", "Cannot be null.");

            IFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream serialisedObject = new MemoryStream())
            {
                binaryFormatter.Serialize(serialisedObject, item);
                serialisedObject.Position = 0;
                SetCacheItem(key, serialisedObject.ToArray());
            }
        }

        /// <summary>
        /// Returns cached item based on key. If item not found in cache the method will return null.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public byte[] GetCachedItem(string key)
        {
            if (String.IsNullOrWhiteSpace(key)) throw new ArgumentNullException("key", "Cannot be null or empty.");

            if (_secureSessionCache.Contains(key))            
                return EncryptionManager.DecryptData((byte[])_secureSessionCache.Get(key), cacheKey);

            return null;
        }

        public C GetCachedItem<C>(string key) where C : class
        {
            var serialisedObject = GetCachedItem(key);

            if (serialisedObject == null)
                return null;

            IFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream serialisedStream = new MemoryStream(serialisedObject))
            {
                serialisedStream.Position = 0;
                return (C)binaryFormatter.Deserialize(serialisedStream);
            }
        }
    }
}