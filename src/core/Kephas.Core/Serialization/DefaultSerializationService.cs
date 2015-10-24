// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultSerializationService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A default serialization service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using Kephas.Serialization.Formats;

    /// <summary>
    /// A default serialization service.
    /// </summary>
    public class DefaultSerializationService : ISerializationService
    {
        /// <summary>
        /// Gets a serializer for the provided context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The serializer.
        /// </returns>
        public ISerializer GetSerializer(ISerializationContext context = null)
        {
            context = context ?? new SerializationContext(typeof(JsonFormat));

            throw new System.NotImplementedException();
        }
    }
}