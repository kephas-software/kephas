// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogConsumer.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Marks loggable classes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging
{
    /// <summary>
    /// Marks loggable classes.
    /// </summary>
    public interface ILogConsumer
    {
        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        ILogger Logger { get; set; }
    }
}