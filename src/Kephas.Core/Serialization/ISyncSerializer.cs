// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISyncSerializer.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISyncSerializer interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using System.IO;

    /// <summary>
    /// Interface for a synchronous serializer.
    /// </summary>
    /// <remarks>
    /// Typically, a serializer supporting synchronous serialization
    /// will implement this interface too.
    /// </remarks>
    public interface ISyncSerializer
    {
        /// <summary>
        /// Serializes the provided object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="textWriter">The <see cref="TextWriter"/> used to write the object content.</param>
        /// <param name="context">The context.</param>
        void Serialize(
            object obj,
            TextWriter textWriter,
            ISerializationContext context = null);

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> containing the serialized object.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        object Deserialize(
            TextReader textReader,
            ISerializationContext context = null);
    }
}