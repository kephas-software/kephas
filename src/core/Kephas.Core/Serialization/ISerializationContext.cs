// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializationContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for serialization contexts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Contract for serialization contexts.
    /// </summary>
    public interface ISerializationContext : IContext
    {
        /// <summary>
        /// Gets the type of the format.
        /// </summary>
        /// <value>
        /// The type of the format.
        /// </value>
        Type FormatType { get; }
    }
}