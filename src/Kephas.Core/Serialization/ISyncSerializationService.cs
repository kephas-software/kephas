// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISyncSerializationService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISyncSerializationService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using System;
    using System.IO;

    /// <summary>
    /// Interface for synchronise serialization service.
    /// </summary>
    public interface ISyncSerializationService
    {
        /// <summary>
        /// Serializes the object with the provided options.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="textWriter">The text writer where the serialized object should be written.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        void Serialize(
            object obj,
            TextWriter textWriter,
            Action<ISerializationContext> optionsConfig = null);

        /// <summary>
        /// Serializes the object with the options provided in the serialization context.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The serialized object.
        /// </returns>
        string Serialize(object obj, Action<ISerializationContext> optionsConfig = null);

        /// <summary>
        /// Deserializes the object with the options provided in the serialization context.
        /// </summary>
        /// <param name="textReader">The text reader where from the serialized object should be read.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        object Deserialize(
            TextReader textReader,
            Action<ISerializationContext> optionsConfig = null);

        /// <summary>
        /// Deserializes the object with the options provided in the serialization context.
        /// </summary>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        object Deserialize(
            string serializedObj,
            Action<ISerializationContext> optionsConfig = null);
    }
}
