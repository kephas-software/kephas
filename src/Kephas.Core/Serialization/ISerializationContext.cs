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
        /// Gets the serialization service.
        /// </summary>
        /// <value>
        /// The serialization service.
        /// </value>
        ISerializationService SerializationService { get; }

        /// <summary>
        /// Gets the type of the format.
        /// </summary>
        /// <value>
        /// The type of the format.
        /// </value>
        Type FormatType { get; }

        /// <summary>
        /// Gets or sets the type of the root object.
        /// </summary>
        /// <value>
        /// The type of the root object.
        /// </value>
        Type RootObjectType { get; set; }

        /// <summary>
        /// Gets or sets the root object factory.
        /// </summary>
        /// <value>
        /// The root object factory.
        /// </value>
        Func<object> RootObjectFactory { get; set; }
    }
}