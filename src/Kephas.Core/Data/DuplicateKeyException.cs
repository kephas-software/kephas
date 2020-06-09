// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DuplicateKeyException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the duplicate key exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Resources;

    /// <summary>
    /// Exception for signalling duplicate key errors.
    /// </summary>
    public class DuplicateKeyException : DataException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateKeyException"/> class.
        /// </summary>
        /// <param name="keyName">The name of the key.</param>
        public DuplicateKeyException(string keyName)
            : base(Strings.DuplicateKeyException_Message)
        {
            Requires.NotNullOrEmpty(keyName, nameof(keyName));

            this.KeyName = keyName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateKeyException"/> class.
        /// </summary>
        /// <param name="keyName">The name of the key.</param>
        /// <param name="message">The message.</param>
        public DuplicateKeyException(string keyName, string message)
            : base(message)
        {
            Requires.NotNullOrEmpty(keyName, nameof(keyName));

            this.KeyName = keyName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateKeyException"/>
        ///  class.
        /// </summary>
        /// <param name="keyName">The name of the key.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public DuplicateKeyException(string keyName, string message, Exception inner)
            : base(message, inner)
        {
            Requires.NotNullOrEmpty(keyName, nameof(keyName));

            this.KeyName = keyName;
        }

        /// <summary>
        /// Gets the name of the key.
        /// </summary>
        /// <value>
        /// The name of the key.
        /// </value>
        public string KeyName { get; }
    }
}