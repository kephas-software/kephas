// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonExpando.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the JSON expando class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.ServiceStack.Text
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Dynamic;

    /// <summary>
    /// An expando object created from a JSON.
    /// </summary>
    public class JsonExpando : Expando
    {
        /// <summary>
        /// The json hash.
        /// </summary>
        private readonly IDictionary<string, object> jsonHash;

        /// <summary>
        /// The parsed.
        /// </summary>
        private readonly IDictionary<string, bool> parsed = new Dictionary<string, bool>();

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonExpando"/> class.
        /// </summary>
        /// <param name="json">The JSON.</param>
        public JsonExpando(string json)
            : this(global::ServiceStack.Text.JsonSerializer.DeserializeFromString<Dictionary<string, object>>(json))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonExpando"/> class.
        /// </summary>
        /// <param name="jsonHash">The json hash.</param>
        public JsonExpando(IDictionary<string, object> jsonHash)
            : base(jsonHash ?? (jsonHash = new Dictionary<string, object>()))
        {
            this.jsonHash = jsonHash;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return global::ServiceStack.Text.JsonSerializer.SerializeToString(this.jsonHash);
        }

        /// <summary>
        /// Tries to convert the provided object to a <see cref="JsonExpando"/>.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A <see cref="JsonExpando"/> object, a list of <see cref="JsonExpando"/> objects, or the original object, if the object could not be converted.
        /// </returns>
        protected internal static object TryConvertToJsonExpando(object obj)
        {
            return obj is IDictionary<string, object> objDictionary 
                       ? new JsonExpando(objDictionary) 
                       : obj is IList objList 
                           ? objList.Cast<object>().Select(TryConvertToJsonExpando).ToList()
                           : obj;
        }

        /// <summary>Attempts to get the dynamic value with the given key.</summary>
        /// <remarks>
        /// First of all, it is tried to get a property value from the inner object, if one is set.
        /// The next try is to retrieve the property value from the expando object itself.
        /// Lastly, if still a property by the provided name cannot be found, the inner dictionary is searched by the provided key.
        /// </remarks>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to get.</param>
        /// <returns>
        /// <c>true</c> if a value is found, <c>false</c> otherwise.
        /// </returns>
        protected override bool TryGetValue(string key, out object value)
        {
            return this.ComputeMember(key, out value);
        }

        /// <summary>
        /// Calculates the member.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="result">The result of the get operation. For example, if the method is
        ///                      called for a property, you can assign the property value to.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        private bool ComputeMember(string name, out object result)
        {
            // if value not in dictionary, return null.
            if (!this.jsonHash.TryGetValue(name, out var jsonObject))
            {
                result = null;
                return true;
            }

            if (this.parsed.TryGetValue(name))
            {
                result = jsonObject;
                return true;
            }

            // if value is not a string or was already parsed
            if (jsonObject is IDictionary<string, object> jsonDict)
            {
                result = new JsonExpando(jsonDict);
                this.jsonHash[name] = result;
                this.parsed[name] = true;
                return true;
            }

            if (jsonObject is IList jsonList)
            {
                result = jsonList.Cast<object>().Select(TryConvertToJsonExpando).ToList();
                this.jsonHash[name] = result;
                this.parsed[name] = true;
                return true;
            }

            result = jsonObject;
            this.jsonHash[name] = result;
            this.parsed[name] = true;
            return true;
        }
    }
}