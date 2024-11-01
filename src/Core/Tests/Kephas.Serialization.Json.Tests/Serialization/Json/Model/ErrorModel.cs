// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorModel.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kephas.Tests.Serialization.Json.Model;

/// <summary>
/// The envelope around every exception response.
/// </summary>
/// <typeparam name="T">Defines the generic type contained in the response.</typeparam>
[ExcludeFromCodeCoverage]
public sealed class ErrorModel<T> where T : Exception
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
    /// Gets or sets the errors.
    /// </summary>
    /// <value>The errors.</value>
    /// <remarks>This property will not be serialized when its value is <code>null</code>.</remarks>
    public T? Exception { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    /// <value>The message.</value>
    /// <remarks>This property will not be serialized when its value is <code>null</code>.</remarks>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the status code.
    /// </summary>
    /// <value>The status code.</value>
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    /// Gets or sets the Error Guid.
    /// </summary>
    public Guid ErrorId { get; set; }

    /// <summary>
    /// Gets or sets additional data. Especially useful to prevent deserialization errors in case of excess properties. 
    /// </summary>
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? Data { get; set; }
}