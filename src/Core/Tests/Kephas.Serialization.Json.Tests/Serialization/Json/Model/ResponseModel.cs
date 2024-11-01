// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResponseModel.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Kephas.Tests.Serialization.Json.Model
{
    public interface IResponseModel
    {
        /// <summary>
        /// Gets or sets the copyright notice.
        /// </summary>
        /// <value>The copyright notice.</value>
        string? CopyrightNotice { get; set; }

        /// <summary>
        /// Gets or sets the optional message.
        /// </summary>
        /// <value>The message.</value>
        string? Message { get; set; }

        /// <summary>
        /// Gets or sets the count of elements contained in the array.
        /// </summary>
        /// <value>The count of elements contained in the array.</value>
        int NumberOfElements { get; set; }
    }

    /// <summary>
    /// The envelope around every successful response.
    /// </summary>
    /// <typeparam name="T">Defines the generic type contained in the response.</typeparam>
    [ExcludeFromCodeCoverage]
    [XmlRoot(nameof(ResponseModel<T>), Namespace = "")]
    public sealed class ResponseModel<T> : IResponseModel
    {
        /// <summary>
        /// Gets or sets the copyright notice.
        /// </summary>
        /// <value>The copyright notice.</value>
        public string? CopyrightNotice { get; set; }

        /// <summary>
        /// Gets or sets the duration of the operation in milliseconds.
        /// </summary>
        /// <value>The duration in milliseconds.</value>
        public double DurationInMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets the optional message.
        /// </summary>
        /// <value>The message.</value>
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets the count of elements contained in the array.
        /// </summary>
        /// <value>The count of elements contained in the array.</value>
        public int NumberOfElements { get; set; }

        /// <summary>
        /// Gets or sets the elements contained in the response.
        /// </summary>
        /// <value>The elements.</value>
        public T? Elements { get; set; }

        /// <summary>
        /// Gets or sets additional data. Especially useful to prevent deserialization errors in case of excess properties. 
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JsonElement>? Data { get; set; }
    }
}
