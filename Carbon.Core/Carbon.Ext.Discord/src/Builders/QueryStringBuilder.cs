using System.Collections.Generic;
using System.Text;

namespace Oxide.Ext.Discord.Builders
{
    /// <summary>
    /// Builder used to build query strings for urls
    /// </summary>
    public class QueryStringBuilder
    {
        private readonly StringBuilder _builder = new StringBuilder("?");

        /// <summary>
        /// Add a key value pair to the query string
        /// </summary>
        /// <param name="key">Key name</param>
        /// <param name="value">Key value</param>
        public void Add(string key, string value)
        {
            _builder.Append(key);
            _builder.Append('=');
            _builder.Append(value);
            _builder.Append('&');
        }

        /// <summary>
        /// Add a list of values with the specified separator
        /// </summary>
        /// <param name="key">Key name</param>
        /// <param name="list">List to be added</param>
        /// <param name="separator">Separator for the list</param>
        /// <typeparam name="T"></typeparam>
        public void AddList<T>(string key, List<T> list, string separator)
        {
            _builder.Append(key);
            _builder.Append('=');
            for (int index = 0; index < list.Count; index++)
            {
                T entry = list[index];
                _builder.Append(entry.ToString());
                if (index + 1 != list.Count)
                {
                    _builder.Append(separator);
                }
            }

            _builder.Append('&');
        }

        /// <summary>
        /// Returns the query string as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}