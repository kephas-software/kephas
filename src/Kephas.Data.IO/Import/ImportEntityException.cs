// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportEntityException.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the import entity exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Import
{
    using System;

    using Kephas.Data.IO.Resources;

    /// <summary>
    /// Exception for signalling import entity errors.
    /// </summary>
    public class ImportEntityException : DataIOException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportEntityException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="entity">The entity.</param>
        public ImportEntityException(string message, object entity)
          : base(message)
        {
            this.Entity = entity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportEntityException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="inner">The inner.</param>
        public ImportEntityException(string message, object entity, Exception inner)
          : base(message, inner)
        {
            this.Entity = entity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportEntityException"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="inner">The inner.</param>
        public ImportEntityException(object entity, Exception inner)
          : this(GetMessage(entity, inner), entity, inner)
        {
            this.Entity = entity;
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <value>
        /// The entity.
        /// </value>
        public object Entity { get; }

        /// <summary>
        /// Gets the formatted message.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="inner">The inner exception.</param>
        /// <returns>
        /// The formatted message.
        /// </returns>
        private static string GetMessage(object entity, Exception inner)
        {
            return string.Format(Strings.ImportEntityException_Message, entity.GetType(), entity, inner.Message);
        }
    }
}