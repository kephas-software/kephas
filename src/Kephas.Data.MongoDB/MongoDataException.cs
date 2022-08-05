// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoDataException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the mongo data exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.MongoDB
{
    using System;

    /// <summary>
    /// Exception for signalling MongoDB data errors.
    /// </summary>
    public class MongoDataException : DataException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDataException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MongoDataException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDataException"/>
        ///  class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public MongoDataException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}