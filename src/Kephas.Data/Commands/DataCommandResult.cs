﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataCommandResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data command result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;

    using Kephas.Data.Resources;
    using Kephas.Operations;

    /// <summary>
    /// Encapsulates the result of a data command.
    /// </summary>
    public class DataCommandResult : OperationResult
    {
        /// <summary>
        /// The result representing a successful operation.
        /// </summary>
        public static readonly DataCommandResult Success = new DataCommandResult(Strings.DataCommandResult_Successful_Message);

        /// <summary>
        /// Initializes a new instance of the <see cref="DataCommandResult"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">Optional. The exception.</param>
        public DataCommandResult(string? message, Exception? exception = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this.MergeMessage(message!);
            }

            if (exception != null)
            {
                this.Fail(exception);
            }
            else
            {
                this.Complete();
            }
        }
    }
}