// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryStringAuthorizationParametersMessageStore.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Stores
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    using global::IdentityServer4.Extensions;
    using global::IdentityServer4.Models;
    using Kephas.Services;
    using Microsoft.AspNetCore.WebUtilities;

    /// <summary>
    /// https://github.com/IdentityServer/IdentityServer4/issues/3552.
    /// </summary>
    /// <seealso cref="IAuthorizationParametersMessageStoreService" />
    [OverridePriority(Priority.Lowest)]
    public class QueryStringAuthorizationParametersMessageStore : IAuthorizationParametersMessageStoreService
    {
        /// <summary>Writes the authorization parameters.</summary>
        /// <param name="message">The message.</param>
        /// <returns>An asynchronous result yielding the identifier for the stored message.</returns>
        public Task<string> WriteAsync(Message<IDictionary<string, string[]>> message)
        {
            var queryString = ToQueryString(FromFullDictionary(message.Data));
            return Task.FromResult(queryString);
        }

        /// <summary>Reads the authorization parameters.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>An asynchronous result yielding a dictionary of parameters.</returns>
        public Task<Message<IDictionary<string, string[]>>> ReadAsync(string id)
        {
            var values = ReadQueryStringAsNameValueCollection(id);
            var msg = new Message<IDictionary<string, string[]>>(ToFullDictionary(values), DateTime.UtcNow);
            return Task.FromResult(msg);
        }

        /// <summary>Deletes the authorization parameters.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The asynchronous result.</returns>
        public Task DeleteAsync(string id)
        {
            return Task.CompletedTask;
        }

        private static NameValueCollection FromFullDictionary(IDictionary<string, string[]> source)
        {
            var nvc = new NameValueCollection();

            foreach ((string key, string[] strings) in source)
            {
                foreach (var value in strings)
                {
                    nvc.Add(key, value);
                }
            }

            return nvc;
        }

        private static string ToQueryString(NameValueCollection collection)
        {
            if (collection.Count == 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder(128);
            var first = true;
            foreach (string name in collection)
            {
                var values = collection.GetValues(name);
                if (values == null || values.Length == 0)
                {
                    first = AppendNameValuePair(builder, first, true, name, string.Empty);
                }
                else
                {
                    foreach (var value in values)
                    {
                        first = AppendNameValuePair(builder, first, true, name, value);
                    }
                }
            }

            return builder.ToString();
        }

        private static bool AppendNameValuePair(StringBuilder builder, bool first, bool urlEncode, string name, string value)
        {
            var effectiveName = name ?? string.Empty;
            var encodedName = urlEncode ? UrlEncoder.Default.Encode(effectiveName) : effectiveName;

            var effectiveValue = value ?? string.Empty;
            var encodedValue = urlEncode ? UrlEncoder.Default.Encode(effectiveValue) : effectiveValue;
            encodedValue = ConvertFormUrlEncodedSpacesToUrlEncodedSpaces(encodedValue);

            if (first)
            {
                first = false;
            }
            else
            {
                builder.Append("&");
            }

            builder.Append(encodedName);
            if (!string.IsNullOrEmpty(encodedValue))
            {
                builder.Append("=");
                builder.Append(encodedValue);
            }

            return first;
        }

        private static string ConvertFormUrlEncodedSpacesToUrlEncodedSpaces(string? str)
        {
            if ((str != null) && (str.IndexOf('+') >= 0))
            {
                str = str.Replace("+", "%20");
            }

            return str;
        }

        private static NameValueCollection ReadQueryStringAsNameValueCollection(string? url)
        {
            if (url != null)
            {
                var idx = url.IndexOf('?');
                if (idx >= 0)
                {
                    url = url.Substring(idx + 1);
                }
                var query = QueryHelpers.ParseNullableQuery(url);
                if (query != null)
                {
                    return query.AsNameValueCollection();
                }
            }

            return new NameValueCollection();
        }

        private static IDictionary<string, string[]> ToFullDictionary(NameValueCollection source)
        {
            return source.AllKeys.ToDictionary(k => k, source.GetValues);
        }
    }
}
