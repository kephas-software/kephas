// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectivityException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Connectivity;

using System;

/// <summary>
/// Exception for signalling connectivity errors.
/// </summary>
public class ConnectivityException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectivityException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public ConnectivityException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectivityException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="inner">The inner exception.</param>
    public ConnectivityException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
