// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the JSON helper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json
{
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A JSON helper.
    /// </summary>
    internal static class JsonHelper
    {
        /// <summary>
        /// A JToken extension method that unwraps the given value.
        /// </summary>
        /// <param name="value">The value to act on.</param>
        /// <returns>
        /// An object.
        /// </returns>
        public static object Unwrap(this JToken value)
        {
            if (value is JObject jobj)
            {
                return new JObjectDictionary(jobj);
            }

            if (value is JValue jval)
            {
                return jval.Value;
            }

            if (value is JArray jarr)
            {
                return new JObjectList(jarr);
            }

            return value;
        }

        /// <summary>
        /// An object extension method that wraps the given object.
        /// </summary>
        /// <param name="obj">The obj to act on.</param>
        /// <returns>
        /// A JToken.
        /// </returns>
        public static JToken Wrap(this object obj)
        {
            return obj is JToken token ? token : JToken.FromObject(obj);
        }
    }
}
