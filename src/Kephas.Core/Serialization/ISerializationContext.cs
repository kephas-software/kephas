// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
        /// Gets or sets the media type.
        /// </summary>
        /// <value>
        /// The media type.
        /// </value>
        Type MediaType { get; set; }

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

        /// <summary>
        /// Gets or sets a value indicating whether the serialized output should be indented.
        /// </summary>
        /// <value>
        /// True if the output should be indented, false if not.
        /// </value>
        bool Indent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the type information should be included.
        /// </summary>
        /// <value>
        /// True to include type information, false otherwise.
        /// </value>
        bool IncludeTypeInfo { get; set; }
    }
}