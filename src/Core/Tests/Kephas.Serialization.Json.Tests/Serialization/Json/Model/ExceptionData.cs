// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionData.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Serialization.Json.Model;

/// <summary>
/// Exception class used to serialize/deserialize exception information.
/// </summary>
public sealed class ExceptionData : Exception
{
    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    public new string? Message { get; set; }
        
    /// <summary>
    /// Gets or sets the inner exception.
    /// </summary>
    public new ExceptionData? InnerException { get; set; }
}