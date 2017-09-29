// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionMessage.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the exception message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Messages
{
    using Kephas.ExceptionHandling;

    /// <summary>
    /// An exception message.
    /// </summary>
    public class ExceptionMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public ExceptionData Exception { get; set; }
    }
}