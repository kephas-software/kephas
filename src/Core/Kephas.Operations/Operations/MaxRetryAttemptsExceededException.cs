// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MaxRetryAttemptsExceededException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Resources;

namespace Kephas.Operations;

/// <summary>
/// Exception indicating that the maximum number of attempts was exceeded.
/// </summary>
public sealed class MaxRetryAttemptsExceededException : OperationFailedException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MaxRetryAttemptsExceededException"/> class.
    /// </summary>
    /// <param name="maxRetryAttempts">The number of maximum retry attempts.</param>
    public MaxRetryAttemptsExceededException(int maxRetryAttempts)
        : this(maxRetryAttempts, string.Format(OperationsStrings.MaxRetryAttemptsExceededException_Message, maxRetryAttempts))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MaxRetryAttemptsExceededException"/> class.
    /// </summary>
    /// <param name="maxRetryAttempts">The number of maximum retry attempts.</param>
    /// <param name="message">The exception message.</param>
    public MaxRetryAttemptsExceededException(int maxRetryAttempts, string message)
        : base(message)
    {
        MaxRetryAttempts = maxRetryAttempts;
        CanRetry = false;
    }

    /// <summary>
    /// Gets the maximum number of retry attempts.
    /// </summary>
    public int MaxRetryAttempts { get; }
}
