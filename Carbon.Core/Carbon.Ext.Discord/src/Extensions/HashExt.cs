using System;
using System.Collections.Generic;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Extensions
{
    /// <summary>
    /// Hash extensions
    /// </summary>
    internal static class HashExt
    {
        /// <summary>
        /// Remove all records from the hash with the given predicate filter
        /// </summary>
        /// <param name="hash">Hash to have data removed from</param>
        /// <param name="predicate">Filter of which values to remove</param>
        /// <typeparam name="TKey">Key type of the hash</typeparam>
        /// <typeparam name="TValue">Value type of the hash</typeparam>
        internal static void RemoveAll<TKey, TValue>(this Hash<TKey, TValue> hash, Func<TValue, bool> predicate)
        {
            if (hash == null)
            {
                return;
            }

            List<TKey> removeKeys = new List<TKey>();
            
            foreach (KeyValuePair<TKey, TValue> key in hash)
            {
                if (predicate(key.Value))
                {
                    removeKeys.Add(key.Key);
                }
            }

            foreach (TKey key in removeKeys)
            {
                hash.Remove(key);
            }
        }

        /// <summary>
        /// Creates a copy of a hash with it's current key value pairs
        /// </summary>
        /// <param name="hash">Hash to be copied</param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns>Copied Hash</returns>
        public static Hash<TKey, TValue> Copy<TKey, TValue>(this Hash<TKey, TValue> hash)
        {
            Hash<TKey, TValue> copy = new Hash<TKey, TValue>();
            foreach (KeyValuePair<TKey, TValue> value in hash)
            {
                copy[value.Key] = value.Value;
            }

            return copy;
        }
    }
}